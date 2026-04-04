# ZoremGame

Juego de acción 3D multijugador indie con mecánicas de movimiento por simbionte. Desarrollado en solitario con asistencia de IA vía Claude Code + MCPs.

---

## Concepto

- **Género:** Acción 3D, melee, multijugador cooperativo/competitivo
- **Jugadores:** Salas privadas (hasta 4 jugadores)
- **Personajes:** 4 personajes jugables con habilidades únicas
- **Mapas:** 4 mapas con distintos biomas y layouts
- **Bosses:** 4 jefes con patrones de ataque únicos
- **Mecánica central:** Sistema Simbionte — el personaje fusiona habilidades con una entidad simbionte que afecta movimiento, combate y habilidades especiales

---

## Stack Técnico

| Componente | Tecnología |
|---|---|
| Motor | Unity 2022.3.39f1 LTS |
| Lenguaje | C# |
| Networking | FishNet + SteamNetworkingSockets |
| Plataforma | Steamworks.NET |
| Modelado 3D | Blender 5.1 |
| Infraestructura | $0 (P2P via Steam) |
| Asistente IA | Claude Code + MCP Unity + MCP Blender |

---

## Estructura del Proyecto

```
ZoremGame/
├── Assets/
│   ├── Animations/         # Animaciones de personajes, bosses, simbionte
│   ├── Audio/              # Música y SFX
│   │   ├── Music/          # Tracks por mapa + boss fight + menú
│   │   └── SFX/            # Combat, movement, UI, items, simbionte
│   ├── Materials/          # Materiales de personajes, entorno, VFX
│   ├── Models/             # Modelos 3D (FBX)
│   │   ├── Characters/
│   │   ├── Bosses/
│   │   ├── Environment/
│   │   └── Weapons/
│   ├── Prefabs/            # GameObjects reutilizables
│   │   ├── Characters/
│   │   ├── Bosses/
│   │   ├── Items/
│   │   └── Systems/
│   ├── Scenes/
│   │   ├── MainMenu/
│   │   ├── Maps/           # Map_01 → Map_04
│   │   ├── BossFight/
│   │   └── DevScenes/
│   └── Scripts/
│       ├── AI/
│       ├── Boss/
│       ├── Combat/
│       ├── Core/
│       ├── Movement/
│       ├── Networking/
│       ├── Player/
│       ├── Symbiote/
│       └── UI/
├── MCP/                    # Model Context Protocol servers
│   ├── servers/
│   │   ├── unity/          # MCP Unity básico (file I/O)
│   │   └── blender/        # MCP Blender (modelado y exportación)
│   └── docs/               # Documentación de implementación MCP
├── BlenderAssets/          # Archivos .blend de trabajo
├── Packages/               # Unity Package Manager
└── ProjectSettings/
```

---

## MCPs (Model Context Protocol)

El proyecto usa MCPs para que Claude Code interactúe directamente con Unity y Blender sin copiar-pegar código manualmente.

### MCP Unity (MCP for Unity — CoplayDev)
Conecta Claude directamente con el Unity Editor en tiempo real. 30+ tools:
- Crear/modificar/eliminar scripts C#
- Gestionar GameObjects, prefabs, escenas
- Modificar materiales, componentes, física
- Leer la consola, compilar, ejecutar tests

### MCP Blender
Servidor Node.js propio que controla Blender via CLI:
- Crear modelos 3D básicos
- Agregar rigs/armaduras
- Exportar a FBX/GLB

---

## Cómo conectar Claude + Unity para trabajar

Cada vez que quieras avanzar en el proyecto con Claude, sigue estos pasos:

### 1. Abrir Unity
Abre Unity Hub y carga el proyecto `ZoremGame`. Espera a que compile completamente.

### 2. Iniciar el servidor MCP
En Unity: `Window > MCP For Unity > Toggle MCP Window` (o `Ctrl+Shift+M`)

Haz clic en **Start Server**. Verás una ventana de terminal abrirse con FastMCP corriendo:
```
INFO: Uvicorn running on http://127.0.0.1:8080
INFO: Plugin registered: ZoremGame
INFO: Registered 30 tools for session ...
```

### 3. Abrir Claude Code
Abre Claude Code en la carpeta del proyecto (`C:\Users\renat\Desktop\ZoremGame`).

### 4. Verificar conexión
Ejecuta `/mcp` en Claude Code. Debes ver `UnityMCP` con estado `connected`.

### 5. Trabajar
Ya puedes pedirle a Claude acciones directas sobre Unity:

```
"Crea un script PlayerController con movimiento WASD en Assets/Scripts/Player/"
"Muestra la jerarquía de la escena activa"
"Crea un material rojo y asígnalo al GameObject Player"
"Lee los errores de la consola"
```

### Notas importantes
- El servidor MCP **se detiene** cuando cierras Unity. Debes iniciarlo de nuevo cada sesión.
- Si Claude pierde conexión con Unity, vuelve al paso 2 y reinicia el servidor.
- El MCP Blender requiere que el servidor Node.js esté corriendo: el `.mcp.json` lo inicia automáticamente al abrir Claude Code.

---

## Requisitos para Colaboradores

```bash
# Instalar dependencias de los MCP servers
cd MCP/servers/unity && npm install && npm run build
cd MCP/servers/blender && npm install && npm run build
```

Actualizar las rutas en `.mcp.json` según tu instalación local de Unity y Blender.

- Unity: `2022.3.39f1` LTS
- Blender: `5.x`
- Node.js: `v18+`
- Python: `3.10+` + `uv` (para MCP for Unity)
