import { exec, execSync } from "child_process";
import { promisify } from "util";
import * as fs from "fs-extra";
import * as path from "path";
import * as os from "os";
import * as crypto from "crypto";

const execAsync = promisify(exec);

export class BlenderCLI {
  private blenderExecutable: string;
  private pythonScriptsPath: string;

  constructor(blenderExecutable: string, pythonScriptsPath: string) {
    this.blenderExecutable = blenderExecutable;
    this.pythonScriptsPath = pythonScriptsPath;
  }

  async runScript(
    scriptName: string,
    args: Record<string, string | number | boolean>,
    timeout: number = 120000
  ): Promise<{ stdout: string; stderr: string }> {
    const scriptPath = path.join(this.pythonScriptsPath, scriptName);
    if (!(await fs.pathExists(scriptPath))) {
      throw new Error(`Script Python no encontrado: ${scriptPath}`);
    }

    // Serializar args como JSON para pasarlos via env var
    const argsJson = JSON.stringify(args);
    const tmpArgsFile = path.join(os.tmpdir(), `blender_args_${crypto.randomUUID()}.json`);
    await fs.writeFile(tmpArgsFile, argsJson, "utf-8");

    const command = `"${this.blenderExecutable}" --background --python "${scriptPath}" --python-exit-code 1 -- "${tmpArgsFile}"`;

    try {
      const result = await execAsync(command, { timeout, maxBuffer: 50 * 1024 * 1024 });
      return result;
    } finally {
      await fs.remove(tmpArgsFile).catch(() => {});
    }
  }

  async runInlineScript(
    script: string,
    timeout: number = 120000
  ): Promise<{ stdout: string; stderr: string }> {
    const tmpScript = path.join(os.tmpdir(), `blender_script_${crypto.randomUUID()}.py`);
    await fs.writeFile(tmpScript, script, "utf-8");

    const command = `"${this.blenderExecutable}" --background --python "${tmpScript}" --python-exit-code 1`;

    try {
      const result = await execAsync(command, { timeout, maxBuffer: 50 * 1024 * 1024 });
      return result;
    } finally {
      await fs.remove(tmpScript).catch(() => {});
    }
  }

  validateInstallation(): boolean {
    try {
      const result = execSync(`"${this.blenderExecutable}" --version`, {
        timeout: 10000,
        stdio: ["pipe", "pipe", "pipe"],
      }).toString();
      return result.toLowerCase().includes("blender");
    } catch {
      return false;
    }
  }
}
