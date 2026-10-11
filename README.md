# ZoremGame

Juego de acción 3D multijugador indie con mecánicas de movimiento por simbionte. Desarrollado en solitario con asistencia de IA vía Claude Code, Codex y MCPs.

---

## Concepto

- **Género:** Acción 3D, melee, multijugador cooperativo/competitivo
- **Jugadores:** Salas privadas (hasta 4 jugadores)
- **Personajes:** 4 personajes jugables con habilidades únicas
- **Mapas:** 4 mapas con distintos biomas y layouts
- **Bosses:** 4 jefes con patrones de ataque únicos
- **Mecánica central:** Sistema Simbionte — el personaje fusiona habilidades con una entidad simbionte que afecta movimiento, combate y habilidades especiales

---

## Estado actual (FASE 1 — Fundación)

Lo que existe hoy en el repo, no lo planificado:

| Sistema | Estado | Dónde |
|---|---|---|
| Locomoción base (caminar, correr, sprint, salto, strafe) | Funcional | `Assets/Scripts/Player/` |
| Slide | Lógica lista, sin animación | `PlayerMotor.cs` |
| Crouch | Lógica lista (con check de espacio libre al levantarse) | `PlayerMotor.cs` |
| Cámara 3ª persona | Básica (sin tilt, sin FOV dinámico, sin lock-on) | `ThirdPersonCamera.cs` |
| Combate melee | No iniciado | — |
| Sistema Simbionte | No iniciado | — |
| Networking | No iniciado (FishNet/Steamworks aún no instalados) | — |

El controlador de jugador es un port del **Invector 3rd Person Controller LITE** al namespace `ZoremGame.Player`, con Slide y Crouch añadidos encima. Jerarquía: `PlayerMotor` → `PlayerAnimator` → `PlayerController`, más `PlayerInput` y `ThirdPersonCamera`.

### Bloqueantes conocidos

- El parámetro `IsSliding` no existe en el Animator Controller en uso (`Invector@BasicLocomotion.controller`), pero `PlayerAnimator` lo escribe cada frame → warnings en consola y slide sin animación.
- `Assets/Animations/Characters/DefaultPlayerController.controller` está vacío (0 estados) y no lo usa ninguna escena.
- No hay prefab de Player: el jugador vive suelto en `Assets/Scenes/PlayerTestScene.unity`. Requisito previo para la FASE 3 (multijugador).

---

## Stack Técnico

| Componente | Tecnología |
|---|---|
| Motor | Unity 6 (`6000.5.0f1`) |
| Render Pipeline | URP (`com.unity.render-pipelines.universal` 17.0.3) |
| Lenguaje | C# |
| Input | Legacy Input Manager (`Input.GetAxis` / `Input.GetKey`) |
| Networking *(planificado)* | FishNet + SteamNetworkingSockets |
| Plataforma *(planificado)* | Steamworks.NET |
| Modelado 3D | Blender 5.x |
| Infraestructura | $0 (P2P via Steam) |
| Asistente IA | Claude Code o Codex + MCP for Unity + MCP Blender |

> Las filas marcadas *(planificado)* todavía no aparecen en `Packages/manifest.json`.

---

## Estructura del Proyecto

```
ZoremGame/
├── Assets/
│   ├── Animations/
│   │   └── Characters/     # DefaultPlayerController.controller
│   ├── Environment/        # Geometría generada por las herramientas de editor
│   ├── Scenes/
│   │   └── PlayerTestScene.unity   # Escena de pruebas de locomoción
│   ├── Settings/           # Assets de configuración de URP
│   ├── ThirdParty/
│   │   └── Invector-3rdPersonController_LITE/   # Asset base: modelo VBOT, animator
│   └── Scripts/
│       ├── Camera/         # ThirdPersonCamera
│       ├── Editor/         # TerrainLandscapeBuilder, TestPlaygroundBuilder, UrpMaterialFixer
│       └── Player/         # PlayerMotor, PlayerAnimator, PlayerController, PlayerInput
├── MCP/                    # Documentación de uso de los MCPs
│   ├── INDEX.md
│   └── docs/
├── BlenderAssets/          # Archivos .blend de trabajo (Git LFS)
├── Documentation/
│   ├── GDD/                # Roadmap, sistemas de desarrollo, specs por etapa
│   ├── Future/             # Visión completa a largo plazo
│   ├── Lore/
│   └── Screenshots/        # Capturas del editor (fuera de Assets para que Unity no las importe)
├── Packages/               # Unity Package Manager (incluye com.coplaydev.unity-mcp embebido)
├── .mcp.json               # Configuración de MCPs para Claude Code / Codex
└── ProjectSettings/
```

`Assets/` solo contiene carpetas con contenido real. Las de cada sistema (Combat, Symbiote, Audio, Prefabs…) se crean al empezar la fase que las necesita, siguiendo los nombres del GDD.

---

## MCPs (Model Context Protocol)

El proyecto usa MCPs para que Claude Code y Codex interactúen directamente con Unity y Blender sin copiar-pegar código manualmente. La configuración vive en `.mcp.json` en la raíz del proyecto.

### MCP for Unity (CoplayDev)
Paquete de Unity embebido en `Packages/com.coplaydev.unity-mcp/`, servido por FastMCP (Python). Conecta el asistente directamente con el Unity Editor en tiempo real. 30+ tools:
- Crear/modificar/eliminar scripts C#
- Gestionar GameObjects, prefabs, escenas
- Modificar materiales, componentes, física
- Leer la consola, compilar, ejecutar tests

Se expone por HTTP en `http://localhost:8080/mcp`. El servidor lo arranca Unity, no Claude Code.

### MCP Blender
Servidor `blender-mcp` lanzado vía `uvx`, que habla con el addon Blender MCP por el puerto `9876`:
- Inspeccionar y modificar la escena
- Crear modelos 3D y rigs
- Exportar a FBX/GLB
- Capturas del viewport

---

## Cómo conectar Claude Code o Codex + los MCPs para trabajar

Cada vez que quieras avanzar en el proyecto con Claude Code o Codex, sigue estos pasos.

### Flujo Unity

#### 1. Abrir Unity
Abre Unity Hub y carga el proyecto `ZoremGame`. Espera a que compile completamente.

#### 2. Iniciar el servidor MCP
En Unity: `Window > MCP For Unity > Toggle MCP Window` (o `Ctrl+Shift+M`)

Haz clic en **Start Server**. Verás una ventana de terminal abrirse con FastMCP corriendo:
```
INFO: Uvicorn running on http://127.0.0.1:8080
INFO: Plugin registered: ZoremGame
INFO: Registered 30 tools for session ...
```

#### 3. Abrir el asistente
Abre Claude Code o Codex en la carpeta del proyecto (`C:\Users\renat\Desktop\ZoremGame`).

#### 4. Verificar conexión
Verifica que el servidor `unity` aparece disponible.

- En Claude Code: ejecuta `/mcp` y confirma que `unity` o `UnityMCP` aparece conectado.
- En Codex: confirma que el servidor `unity` aparece listado con sus tools/resources y responde llamadas de lectura del editor o la escena.

#### 5. Trabajar
Ya puedes pedirle a Claude Code o Codex acciones directas sobre Unity:

```
"Crea un script PlayerController con movimiento WASD en Assets/Scripts/Player/"
"Muestra la jerarquía de la escena activa"
"Crea un material rojo y asígnalo al GameObject Player"
"Lee los errores de la consola"
```

### Flujo Blender

#### 1. Abrir Blender
Abre Blender con el addon de Blender MCP habilitado.

#### 2. Iniciar o reconectar el servidor MCP
En la barra lateral (`N`) abre la pestaña **Blender MCP** y confirma que el servidor está corriendo.

Si quieres habilitar integraciones extra, marca las opciones disponibles en el panel antes de reconectar:
- `Use assets from Poly Haven`
- `Use assets from Sketchfab`
- `Use Hyper3D Rodin 3D model generation`
- `Use Tencent Hunyuan 3D model generation`

#### 3. Abrir el asistente
Abre Claude Code o Codex en la carpeta del proyecto (`C:\Users\renat\Desktop\ZoremGame`).

#### 4. Verificar conexión
Verifica que el servidor de Blender aparece disponible y responde.

- En Claude Code: ejecuta `/mcp` y confirma que `blender` aparece conectado.
- En Codex: confirma que el MCP de Blender responde a lecturas de escena, objetos o viewport.

#### 5. Trabajar
Ya puedes pedirle a Claude Code o Codex acciones directas sobre Blender:

```
"Muéstrame la escena actual de Blender"
"Inspecciona el objeto Cube"
"Genera un modelo base simple para un prop"
"Haz una captura del viewport"
```

### Notas importantes
- El servidor MCP de Unity **se detiene** cuando cierras Unity. Debes iniciarlo de nuevo cada sesión.
- Si Claude Code o Codex pierde conexión con Unity, vuelve al paso 2 y reinicia el servidor.
- Si Claude Code o Codex pierde conexión con Blender, reconecta el servidor desde el panel **Blender MCP**.
- El `.mcp.json` del proyecto registra ambos servidores para que Claude Code y Codex puedan conectarse a Unity y Blender desde esta carpeta.

---

## Herramientas de Editor

Disponibles desde el menú `ZoremGame` en Unity:

| Herramienta | Qué hace |
|---|---|
| `Fix URP Materials` | Reemplaza shaders Built-in rotos (magenta tras migrar a URP) por `Universal Render Pipeline/Lit`, conservando color y textura principal |
| `TerrainLandscapeBuilder` | Genera terreno de pruebas |
| `TestPlaygroundBuilder` | Genera el playground de locomoción (rampas, escalones, obstáculos) |

---

## Requisitos para Colaboradores

- Unity: `6000.5.0f1` (Unity 6) con URP
- Blender: `5.x` con el addon Blender MCP habilitado
- Python: `3.10+` + `uv` / `uvx` (para MCP for Unity y `blender-mcp`)
- Git LFS (los `.blend` de `BlenderAssets/` se gestionan con LFS)

El paquete `com.coplaydev.unity-mcp` viene embebido en `Packages/`, así que no requiere instalación aparte. Solo hay que ajustar la ruta de `uvx` en `.mcp.json` según tu instalación local.
