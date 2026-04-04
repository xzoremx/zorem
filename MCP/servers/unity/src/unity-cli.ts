import { exec } from "child_process";
import { promisify } from "util";
import * as fs from "fs-extra";
import * as path from "path";

const execAsync = promisify(exec);

export class UnityCLI {
  private projectPath: string;
  private unityExecutable: string;

  constructor(projectPath: string, unityExecutable: string) {
    this.projectPath = projectPath;
    this.unityExecutable = unityExecutable;
  }

  async executeMethod(
    className: string,
    methodName: string,
    timeout: number = 60000
  ): Promise<{ stdout: string; stderr: string }> {
    const command = `"${this.unityExecutable}" -projectPath "${this.projectPath}" -batchmode -quit -executeMethod ${className}.${methodName} -logFile -`;
    return execAsync(command, { timeout, maxBuffer: 10 * 1024 * 1024 });
  }

  async compile(): Promise<{ success: boolean; errors: string[]; warnings: string[] }> {
    try {
      await this.executeMethod("UnityEditor.EditorBuildSettings", "Refresh");
      return { success: true, errors: [], warnings: [] };
    } catch (error) {
      return {
        success: false,
        errors: [error instanceof Error ? error.message : String(error)],
        warnings: [],
      };
    }
  }

  async getCompileErrors(): Promise<string[]> {
    const logPath = path.join(this.projectPath, "Logs", "AssetImportWorker0.log");
    try {
      if (!(await fs.pathExists(logPath))) return [];
      const content = await fs.readFile(logPath, "utf-8");
      return content
        .split("\n")
        .filter((line) => line.includes("error CS") || line.includes("Error"))
        .slice(0, 20);
    } catch {
      return [];
    }
  }

  async validateScript(scriptPath: string): Promise<boolean> {
    const fullPath = path.join(this.projectPath, scriptPath);
    return fs.pathExists(fullPath);
  }
}
