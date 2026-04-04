export interface BlenderProjectConfig {
  blenderExecutable: string;
  outputPath: string;
  pythonScriptsPath: string;
  defaultFormat: "fbx" | "glb";
}

export interface ModelSpec {
  type: "humanoid" | "cube" | "sphere" | "cylinder" | "plane";
  name: string;
  height?: number;
  width?: number;
  depth?: number;
}

export interface ExportOptions {
  blend_file: string;
  output_path: string;
  format?: "fbx" | "glb";
  apply_modifiers?: boolean;
  scale?: number;
}

export interface AnimationSpec {
  blend_file: string;
  name: string;
  frame_start: number;
  frame_end: number;
  fps?: number;
}

export interface RigInput {
  blend_file: string;
  object_name: string;
}
