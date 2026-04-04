export interface UnityProjectConfig {
  projectPath: string;
  unityExecutable: string;
  assetPath: string;
  autoCompile: boolean;
}

export interface ScriptCreateInput {
  path: string;
  content: string;
  namespace?: string;
  auto_compile?: boolean;
  force?: boolean;
}

export interface ScriptDeleteInput {
  path: string;
}

export interface CompileInput {
  target?: "StandaloneWindows64" | "StandaloneLinux64" | "StandaloneOSX";
}

export interface ToolResult {
  content: Array<{
    type: "text" | "image" | "resource";
    text?: string;
  }>;
}
