import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import {
  CallToolRequestSchema,
  ListToolsRequestSchema,
} from "@modelcontextprotocol/sdk/types.js";
import * as fs from "fs-extra";
import * as path from "path";
import { BlenderCLI } from "./blender-cli.js";
import { BlenderProjectConfig, ModelSpec, ExportOptions, AnimationSpec, RigInput } from "./types.js";

const config: BlenderProjectConfig = {
  blenderExecutable: process.env.BLENDER_EXECUTABLE || "C:/Program Files/Blender Foundation/Blender 5.1/blender.exe",
  outputPath: process.env.OUTPUT_PATH || "C:/Users/renat/Desktop/ZoremGame/BlenderAssets",
  pythonScriptsPath: process.env.BLENDER_SCRIPTS || path.join(__dirname, "../python"),
  defaultFormat: (process.env.EXPORT_FORMAT as "fbx" | "glb") || "fbx",
};

const cli = new BlenderCLI(config.blenderExecutable, config.pythonScriptsPath);

const server = new Server(
  { name: "zoremgame-blender-mcp", version: "1.0.0" },
  { capabilities: { tools: {} } }
);

// ============================================================
// LIST TOOLS
// ============================================================

server.setRequestHandler(ListToolsRequestSchema, async () => ({
  tools: [
    {
      name: "create_model",
      description: "Crea un modelo 3D básico en Blender y lo guarda como .blend",
      inputSchema: {
        type: "object",
        properties: {
          name: { type: "string", description: "Nombre del modelo" },
          type: {
            type: "string",
            enum: ["humanoid", "cube", "sphere", "cylinder", "plane"],
            description: "Tipo de mesh base",
          },
          height: { type: "number", description: "Altura del modelo (para humanoid)" },
          width: { type: "number", description: "Ancho del modelo" },
          depth: { type: "number", description: "Profundidad del modelo" },
        },
        required: ["name", "type"],
      },
    },
    {
      name: "add_rig",
      description: "Agrega una armadura (rig) a un objeto en un archivo .blend",
      inputSchema: {
        type: "object",
        properties: {
          blend_file: { type: "string", description: "Ruta al archivo .blend" },
          object_name: { type: "string", description: "Nombre del objeto a rígger" },
        },
        required: ["blend_file", "object_name"],
      },
    },
    {
      name: "export_fbx",
      description: "Exporta un archivo .blend a FBX",
      inputSchema: {
        type: "object",
        properties: {
          blend_file: { type: "string", description: "Ruta al archivo .blend" },
          output_path: { type: "string", description: "Ruta de salida del FBX" },
          apply_modifiers: { type: "boolean", description: "Aplicar modifiers antes de exportar", default: true },
          scale: { type: "number", description: "Escala de exportación", default: 1.0 },
        },
        required: ["blend_file", "output_path"],
      },
    },
    {
      name: "export_glb",
      description: "Exporta un archivo .blend a GLB/GLTF",
      inputSchema: {
        type: "object",
        properties: {
          blend_file: { type: "string", description: "Ruta al archivo .blend" },
          output_path: { type: "string", description: "Ruta de salida del GLB" },
        },
        required: ["blend_file", "output_path"],
      },
    },
    {
      name: "list_objects",
      description: "Lista los objetos en un archivo .blend",
      inputSchema: {
        type: "object",
        properties: {
          blend_file: { type: "string", description: "Ruta al archivo .blend" },
        },
        required: ["blend_file"],
      },
    },
    {
      name: "run_python",
      description: "Ejecuta un script Python personalizado en Blender",
      inputSchema: {
        type: "object",
        properties: {
          script: { type: "string", description: "Código Python para ejecutar en Blender (usa bpy)" },
        },
        required: ["script"],
      },
    },
  ],
}));

// ============================================================
// HANDLERS
// ============================================================

async function handleCreateModel(input: ModelSpec): Promise<any> {
  if (!input.name) throw new Error("name es requerido");

  await fs.ensureDir(config.outputPath);
  const blendFile = path.join(config.outputPath, `${input.name}.blend`).replace(/\\/g, "/");

  const meshOps: Record<string, string> = {
    humanoid: `bpy.ops.mesh.primitive_uv_sphere_add(radius=0.5, location=(0, 0, 1.7))`,
    cube: `bpy.ops.mesh.primitive_cube_add(size=1)`,
    sphere: `bpy.ops.mesh.primitive_uv_sphere_add(radius=1)`,
    cylinder: `bpy.ops.mesh.primitive_cylinder_add(radius=0.5, depth=${input.height || 2})`,
    plane: `bpy.ops.mesh.primitive_plane_add(size=2)`,
  };

  const script = `
import bpy

# Limpiar escena default
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete()

# Crear mesh
${meshOps[input.type]}
obj = bpy.context.active_object
obj.name = '${input.name}'

# Guardar
bpy.ops.wm.save_as_mainfile(filepath='${blendFile}')
print("MODEL_CREATED:${blendFile}")
`;

  const result = await cli.runInlineScript(script);
  const success = result.stdout.includes("MODEL_CREATED") || result.stderr.includes("MODEL_CREATED");

  return {
    content: [{
      type: "text",
      text: success
        ? `Modelo creado: ${input.name}\nTipo: ${input.type}\nArchivo: ${blendFile}`
        : `Advertencia - revisa salida:\n${result.stderr.slice(-500)}`,
    }],
  };
}

async function handleAddRig(input: RigInput): Promise<any> {
  if (!(await fs.pathExists(input.blend_file))) {
    throw new Error(`Archivo .blend no encontrado: ${input.blend_file}`);
  }

  const blendFile = input.blend_file.replace(/\\/g, "/");
  const script = `
import bpy

bpy.ops.wm.open_mainfile(filepath='${blendFile}')

obj = bpy.data.objects.get('${input.object_name}')
if not obj:
    raise Exception('Objeto no encontrado: ${input.object_name}')

bpy.context.view_layer.objects.active = obj
bpy.ops.object.armature_add(enter_editmode=False)
armature = bpy.context.active_object
armature.name = '${input.object_name}_Rig'

bpy.ops.wm.save_mainfile()
print("RIG_ADDED:${input.object_name}_Rig")
`;

  const result = await cli.runInlineScript(script);
  return {
    content: [{
      type: "text",
      text: `Rig agregado a ${input.object_name}\nGuardado en: ${input.blend_file}`,
    }],
  };
}

async function handleExportFBX(input: ExportOptions): Promise<any> {
  if (!(await fs.pathExists(input.blend_file))) {
    throw new Error(`Archivo .blend no encontrado: ${input.blend_file}`);
  }

  await fs.ensureDir(path.dirname(input.output_path));
  const blendFile = input.blend_file.replace(/\\/g, "/");
  const outputPath = input.output_path.replace(/\\/g, "/");
  const scale = input.scale || 1.0;

  const script = `
import bpy

bpy.ops.wm.open_mainfile(filepath='${blendFile}')

bpy.ops.export_scene.fbx(
    filepath='${outputPath}',
    use_selection=False,
    object_types={'ARMATURE', 'MESH', 'EMPTY'},
    use_mesh_modifiers=${input.apply_modifiers !== false ? "True" : "False"},
    global_scale=${scale},
    axis_forward='-Y',
    axis_up='Z'
)
print("FBX_EXPORTED:${outputPath}")
`;

  await cli.runInlineScript(script);
  return {
    content: [{ type: "text", text: `FBX exportado: ${input.output_path}` }],
  };
}

async function handleExportGLB(input: { blend_file: string; output_path: string }): Promise<any> {
  if (!(await fs.pathExists(input.blend_file))) {
    throw new Error(`Archivo .blend no encontrado: ${input.blend_file}`);
  }

  await fs.ensureDir(path.dirname(input.output_path));
  const blendFile = input.blend_file.replace(/\\/g, "/");
  const outputPath = input.output_path.replace(/\\/g, "/");

  const script = `
import bpy

bpy.ops.wm.open_mainfile(filepath='${blendFile}')

bpy.ops.export_scene.gltf(
    filepath='${outputPath}',
    export_format='GLB',
    use_selection=False
)
print("GLB_EXPORTED:${outputPath}")
`;

  await cli.runInlineScript(script);
  return {
    content: [{ type: "text", text: `GLB exportado: ${input.output_path}` }],
  };
}

async function handleListObjects(input: { blend_file: string }): Promise<any> {
  if (!(await fs.pathExists(input.blend_file))) {
    throw new Error(`Archivo .blend no encontrado: ${input.blend_file}`);
  }

  const blendFile = input.blend_file.replace(/\\/g, "/");
  const script = `
import bpy

bpy.ops.wm.open_mainfile(filepath='${blendFile}')

for obj in bpy.data.objects:
    print(f"OBJECT:{obj.name}:{obj.type}")
`;

  const result = await cli.runInlineScript(script);
  const objects = (result.stdout + result.stderr)
    .split("\n")
    .filter((l) => l.startsWith("OBJECT:"))
    .map((l) => l.replace("OBJECT:", "").trim());

  return {
    content: [{
      type: "text",
      text: objects.length > 0
        ? `Objetos en ${input.blend_file}:\n${objects.join("\n")}`
        : "No se encontraron objetos",
    }],
  };
}

async function handleRunPython(input: { script: string }): Promise<any> {
  const result = await cli.runInlineScript(input.script);
  return {
    content: [{
      type: "text",
      text: `Salida:\n${result.stdout}\n${result.stderr}`.trim(),
    }],
  };
}

// ============================================================
// ROUTER
// ============================================================

server.setRequestHandler(CallToolRequestSchema, async (request) => {
  const { name, arguments: args } = request.params;

  switch (name) {
    case "create_model":
      return handleCreateModel(args as unknown as ModelSpec);
    case "add_rig":
      return handleAddRig(args as unknown as RigInput);
    case "export_fbx":
      return handleExportFBX(args as unknown as ExportOptions);
    case "export_glb":
      return handleExportGLB(args as unknown as { blend_file: string; output_path: string });
    case "list_objects":
      return handleListObjects(args as unknown as { blend_file: string });
    case "run_python":
      return handleRunPython(args as unknown as { script: string });
    default:
      throw new Error(`Tool desconocido: ${name}`);
  }
});

// ============================================================
// INICIAR
// ============================================================

async function main() {
  const isValid = cli.validateInstallation();
  if (!isValid) {
    console.error(`[Blender MCP] ADVERTENCIA: Blender no encontrado en ${config.blenderExecutable}`);
    console.error("[Blender MCP] Configura BLENDER_EXECUTABLE en settings.json");
  } else {
    console.error(`[Blender MCP] Blender encontrado. Listo.`);
  }

  const transport = new StdioServerTransport();
  await server.connect(transport);
  console.error("[Blender MCP] Servidor listo.");
}

main().catch((err) => {
  console.error("Error fatal:", err);
  process.exit(1);
});
