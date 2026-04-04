import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import {
  CallToolRequestSchema,
  ListToolsRequestSchema,
} from "@modelcontextprotocol/sdk/types.js";
import * as fs from "fs-extra";
import * as path from "path";
import { UnityCLI } from "./unity-cli.js";
import { UnityProjectConfig, ScriptCreateInput } from "./types.js";

const config: UnityProjectConfig = {
  projectPath: process.env.UNITY_PROJECT_PATH || "C:/Users/renat/Desktop/ZoremGame",
  unityExecutable: process.env.UNITY_EXECUTABLE || "C:/Program Files/Unity/Hub/Editor/2022.3.0f1/Editor/Unity.exe",
  assetPath: process.env.ASSET_STORE_PATH || "Assets",
  autoCompile: process.env.AUTO_COMPILE === "true",
};

const cli = new UnityCLI(config.projectPath, config.unityExecutable);

const server = new Server(
  { name: "zoremgame-unity-mcp", version: "1.0.0" },
  { capabilities: { tools: {} } }
);

// ============================================================
// LIST TOOLS
// ============================================================

server.setRequestHandler(ListToolsRequestSchema, async () => ({
  tools: [
    {
      name: "create_script",
      description: "Crea un nuevo script C# en el proyecto Unity",
      inputSchema: {
        type: "object",
        properties: {
          path: { type: "string", description: "Ruta desde Assets/ (ej: Scripts/Player/PlayerController.cs)" },
          content: { type: "string", description: "Código fuente C#" },
          namespace: { type: "string", description: "Namespace C# (default: ZoremGame)" },
          auto_compile: { type: "boolean", description: "Compilar tras crear", default: false },
          force: { type: "boolean", description: "Sobreescribir si ya existe", default: false },
        },
        required: ["path", "content"],
      },
    },
    {
      name: "modify_script",
      description: "Modifica el contenido de un script C# existente",
      inputSchema: {
        type: "object",
        properties: {
          path: { type: "string", description: "Ruta del script desde Assets/" },
          content: { type: "string", description: "Nuevo contenido completo del script" },
        },
        required: ["path", "content"],
      },
    },
    {
      name: "delete_script",
      description: "Elimina un script C# del proyecto",
      inputSchema: {
        type: "object",
        properties: {
          path: { type: "string", description: "Ruta del script desde Assets/" },
        },
        required: ["path"],
      },
    },
    {
      name: "read_script",
      description: "Lee el contenido de un script C# existente",
      inputSchema: {
        type: "object",
        properties: {
          path: { type: "string", description: "Ruta del script desde Assets/" },
        },
        required: ["path"],
      },
    },
    {
      name: "list_scripts",
      description: "Lista todos los scripts C# en una carpeta",
      inputSchema: {
        type: "object",
        properties: {
          folder: { type: "string", description: "Carpeta dentro de Assets/ (default: Scripts)" },
        },
        required: [],
      },
    },
    {
      name: "get_errors",
      description: "Obtiene errores de compilación del proyecto Unity",
      inputSchema: {
        type: "object",
        properties: {},
        required: [],
      },
    },
    {
      name: "compile_project",
      description: "Compila el proyecto Unity y reporta errores",
      inputSchema: {
        type: "object",
        properties: {
          target: {
            type: "string",
            enum: ["StandaloneWindows64", "StandaloneLinux64", "StandaloneOSX"],
          },
        },
        required: [],
      },
    },
  ],
}));

// ============================================================
// HANDLERS
// ============================================================

async function handleCreateScript(input: ScriptCreateInput) {
  if (!input.path || !input.content) throw new Error("path and content son requeridos");

  const safePath = input.path.replace(/\.\.\//g, "").replace(/\.\.\\/g, "");
  const fullPath = path.join(config.projectPath, config.assetPath, safePath);

  if (!fullPath.startsWith(path.join(config.projectPath, config.assetPath))) {
    throw new Error("Los scripts deben estar en la carpeta Assets/");
  }

  if (!input.force && (await fs.pathExists(fullPath))) {
    throw new Error(`El archivo ya existe: ${safePath}. Usa force: true para sobreescribir.`);
  }

  await fs.ensureDir(path.dirname(fullPath));
  const namespace = input.namespace || "ZoremGame";
  const wrappedContent = input.content.includes("namespace ")
    ? input.content
    : `namespace ${namespace}\n{\n${input.content}\n}\n`;

  await fs.writeFile(fullPath, wrappedContent, "utf-8");

  if (input.auto_compile || config.autoCompile) {
    await cli.compile();
  }

  return {
    content: [{ type: "text", text: `Script creado: Assets/${safePath}\nNamespace: ${namespace}` }],
  };
}

async function handleModifyScript(input: { path: string; content: string }) {
  const safePath = input.path.replace(/\.\.\//g, "").replace(/\.\.\\/g, "");
  const fullPath = path.join(config.projectPath, config.assetPath, safePath);

  if (!(await fs.pathExists(fullPath))) throw new Error(`Archivo no encontrado: ${safePath}`);

  await fs.writeFile(fullPath, input.content, "utf-8");
  return { content: [{ type: "text", text: `Script modificado: Assets/${safePath}` }] };
}

async function handleDeleteScript(input: { path: string }) {
  const safePath = input.path.replace(/\.\.\//g, "").replace(/\.\.\\/g, "");
  const fullPath = path.join(config.projectPath, config.assetPath, safePath);

  if (!(await fs.pathExists(fullPath))) throw new Error(`Archivo no encontrado: ${safePath}`);

  await fs.remove(fullPath);
  const metaPath = fullPath + ".meta";
  if (await fs.pathExists(metaPath)) await fs.remove(metaPath);

  return { content: [{ type: "text", text: `Script eliminado: Assets/${safePath}` }] };
}

async function handleReadScript(input: { path: string }) {
  const safePath = input.path.replace(/\.\.\//g, "").replace(/\.\.\\/g, "");
  const fullPath = path.join(config.projectPath, config.assetPath, safePath);

  if (!(await fs.pathExists(fullPath))) throw new Error(`Archivo no encontrado: ${safePath}`);

  const content = await fs.readFile(fullPath, "utf-8");
  return { content: [{ type: "text", text: `// Assets/${safePath}\n\n${content}` }] };
}

async function handleListScripts(input: { folder?: string }) {
  const folder = input.folder || "Scripts";
  const folderPath = path.join(config.projectPath, config.assetPath, folder);

  if (!(await fs.pathExists(folderPath))) {
    return { content: [{ type: "text", text: `Carpeta no encontrada: Assets/${folder}` }] };
  }

  const files: string[] = [];
  const walkDir = async (dir: string): Promise<void> => {
    const entries = await fs.readdir(dir, { withFileTypes: true });
    for (const entry of entries) {
      const fullPath = path.join(dir, entry.name);
      if (entry.isDirectory()) {
        await walkDir(fullPath);
      } else if (entry.name.endsWith(".cs")) {
        files.push(fullPath.replace(config.projectPath + path.sep, "").replace(/\\/g, "/"));
      }
    }
  };

  await walkDir(folderPath);
  const list = files.length > 0 ? files.join("\n") : "No se encontraron scripts .cs";
  return { content: [{ type: "text", text: `Scripts en Assets/${folder}:\n${list}` }] };
}

async function handleGetErrors() {
  const errors = await cli.getCompileErrors();
  return {
    content: [{
      type: "text",
      text: errors.length > 0 ? `Errores de compilación:\n${errors.join("\n")}` : "Sin errores de compilación",
    }],
  };
}

async function handleCompileProject() {
  const result = await cli.compile();
  const status = result.success ? "Compilación exitosa" : `Compilación fallida:\n${result.errors.join("\n")}`;
  return { content: [{ type: "text", text: status }] };
}

// ============================================================
// ROUTER
// ============================================================

server.setRequestHandler(CallToolRequestSchema, async (request) => {
  const { name, arguments: args } = request.params;

  switch (name) {
    case "create_script":
      return handleCreateScript(args as unknown as ScriptCreateInput);
    case "modify_script":
      return handleModifyScript(args as unknown as { path: string; content: string });
    case "delete_script":
      return handleDeleteScript(args as unknown as { path: string });
    case "read_script":
      return handleReadScript(args as unknown as { path: string });
    case "list_scripts":
      return handleListScripts(args as unknown as { folder?: string });
    case "get_errors":
      return handleGetErrors();
    case "compile_project":
      return handleCompileProject();
    default:
      throw new Error(`Tool desconocido: ${name}`);
  }
});

// ============================================================
// INICIAR
// ============================================================

async function main() {
  const projectExists = await fs.pathExists(path.join(config.projectPath, "ProjectSettings"));
  if (!projectExists) {
    console.error(`[Unity MCP] ADVERTENCIA: Proyecto Unity no encontrado en ${config.projectPath}`);
    console.error("[Unity MCP] El servidor iniciará de todas formas. Configura UNITY_PROJECT_PATH correctamente.");
  } else {
    console.error(`[Unity MCP] Proyecto encontrado en ${config.projectPath}`);
  }

  const transport = new StdioServerTransport();
  await server.connect(transport);
  console.error("[Unity MCP] Servidor listo.");
}

main().catch((err) => {
  console.error("Error fatal:", err);
  process.exit(1);
});
