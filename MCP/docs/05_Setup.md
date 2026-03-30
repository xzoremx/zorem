# 05 - Setup & Configuración Final

---

## Requisitos Previos

```bash
# Node.js y npm
node --version  # v18+
npm --version   # v9+

# Unity (ya instalado)
# Blender (ya instalado)

# TypeScript
npm install -g typescript ts-node
```

---

## Instalación Paso a Paso

### Paso 1: Crear y compilar MCP Servers

```bash
# En MCP/servers/unity/
cd MCP/servers/unity
npm install
npm run build

# En MCP/servers/blender/
cd ../blender
npm install
npm run build

# Verificar que los builds existen
ls build/index.js  # Debe existir en ambas carpetas
```

### Paso 2: Crear archivo de configuración

En tu carpeta de Claude Code (típicamente ~/.claude/), crea o edita `settings.json`:

```json
{
  "mcpServers": {
    "unity": {
      "command": "node",
      "args": ["/absolute/path/to/MCP/servers/unity/build/index.js"],
      "env": {
        "UNITY_PROJECT_PATH": "/absolute/path/to/ZoremGame",
        "UNITY_EXECUTABLE": "/path/to/Unity/Hub/Editors/VERSION/Unity.app/Contents/MacOS/Unity",
        "ASSET_STORE_PATH": "Assets",
        "AUTO_COMPILE": "true"
      }
    },
    "blender": {
      "command": "node",
      "args": ["/absolute/path/to/MCP/servers/blender/build/index.js"],
      "env": {
        "BLENDER_EXECUTABLE": "/Applications/Blender/blender",
        "OUTPUT_PATH": "/absolute/path/to/ZoremGame/Assets/Models",
        "BLENDER_SCRIPTS": "/absolute/path/to/MCP/servers/blender/python",
        "EXPORT_FORMAT": "fbx"
      }
    }
  }
}
```

### Paso 3: Verificar ruta de Unity

```bash
# En macOS
/Applications/Unity/Hub/Editors/*/Unity.app/Contents/MacOS/Unity --version

# En Windows
"C:\Program Files\Unity\Hub\Editor\*\Editor\Unity.exe" -version

# Copiar ruta exacta a settings.json
```

### Paso 4: Verificar instalación de Blender

```bash
blender --version
# Debe mostrar "Blender X.X.X"

which blender
# Copiar ruta exacta a settings.json (env.BLENDER_EXECUTABLE)
```

### Paso 5: Reiniciar Claude Code

Una vez que actualizaste `settings.json`:
1. Cierra Claude Code completamente
2. Abre de nuevo
3. Verifica que los MCPs aparecen en el panel lateral

---

## Verificación de Funcionamiento

### Test 1: MCP Unity

En Claude Code, ejecuta:

```
Prompt: "Crea un script test simple en Assets/Scripts/Test.cs que imprima 'Hello World'"
```

**Resultado esperado:**
- Archivo creado en `Assets/Scripts/Test.cs`
- Sin errores de compilación
- Claude reporta: "Script created successfully"

### Test 2: MCP Blender

En Claude Code, ejecuta:

```
Prompt: "Crea un modelo humanoid simple llamado TestHero y exportalo como FBX a Assets/Models/TestHero.fbx"
```

**Resultado esperado:**
- Archivo FBX creado en `Assets/Models/TestHero.fbx`
- Claude reporta: "Exported successfully"

---

## Troubleshooting

### "Unknown MCP"

**Problema:** Claude Code no reconoce el MCP

**Soluciones:**
1. Verificar ruta en settings.json (usar rutas absolutas)
2. Compilar nuevamente: `npm run build`
3. Reiniciar Claude Code
4. Ver logs: `cat ~/.claude/logs/mcp*.log`

### "Unity not found"

**Problema:** MCP no encuentra Unity

**Soluciones:**
```bash
# Verificar ruta exacta
which unity
# o
find /Applications -name "Unity" -type d

# Actualizar en settings.json con ruta correcta
```

### "Blender execution timeout"

**Problema:** Blender tarda demasiado o timeout

**Soluciones:**
1. Aumentar timeout en handler (cambiar 120000 a 180000 ms)
2. Asegurar que no hay otro Blender process corriendo: `pkill blender`
3. Verificar permisos: `chmod +x /path/to/blender`

---

## Configuración Avanzada

### Auto-compile en Unity

Para desactivar compilación automática:

```json
{
  "env": {
    "AUTO_COMPILE": "false"
  }
}
```

### Blender en modo headless (sin interfaz gráfica)

Ya está configurado en el MCP (--background flag).

### Cambiar formato de exportación Blender

```json
{
  "env": {
    "EXPORT_FORMAT": "glb"  // o "fbx", "usdz"
  }
}
```

---

## Estructura Final

```
MCP/
├── docs/
│   ├── README.md
│   ├── 01_Vision.md
│   ├── 02_Arquitectura.md
│   ├── 03_Unity_MCP.md
│   ├── 04_Blender_MCP.md
│   ├── 05_Setup.md
│   ├── 06_Testing.md
│   └── 07_Referencia_Tecnica.md
├── servers/
│   ├── unity/
│   │   ├── build/
│   │   │   └── index.js (compilado)
│   │   └── src/
│   └── blender/
│       ├── build/
│       │   └── index.js (compilado)
│       ├── python/
│       │   ├── create_model.py
│       │   ├── add_rig.py
│       │   ├── export_fbx.py
│       │   └── utils.py
│       └── src/
└── scripts/
    └── (scripts de testing y setup)
```

---

## Próximo: Testing

Ve a **[06_Testing.md](06_Testing.md)** para estrategia completa de testing.
