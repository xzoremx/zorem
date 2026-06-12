# 03 - Referencia de Herramientas

Listado completo de herramientas disponibles en cada MCP.

---

## MCP for Unity (30+ Herramientas)

### Gestión de Scripts

| Herramienta | Descripción | Parámetros |
|---|---|---|
| `create_script` | Crea un nuevo script C# | `path`, `content`, `namespace`, `auto_compile`, `force` |
| `modify_script` | Modifica un script existente | `path`, `content` |
| `delete_script` | Elimina un script | `path` |
| `read_script` | Lee el contenido de un script | `path` |
| `list_scripts` | Lista scripts en una carpeta | `folder` |

**Ejemplo:**
```
Crea un script en Assets/Scripts/Player/Movement.cs que implemente ILocomotionSystem
```

### Compilación y Errores

| Herramienta | Descripción | Parámetros |
|---|---|---|
| `get_errors` | Obtiene errores de compilación | (ninguno) |
| `compile_project` | Compila el proyecto | `target` (StandaloneWindows64, etc.) |

**Ejemplo:**
```
Compila el proyecto y reporta errores
```

### GameObjects y Componentes

Estas herramientas permiten:
- Crear GameObjects
- Configurar componentes (Rigidbody, Collider, Animator, etc.)
- Modificar propiedades
- Eliminar objetos

**Ejemplo:**
```
En la escena actual:
1. Crea un Cube llamado "Player"
2. Agrega un Rigidbody con mass=80
3. Agrega un BoxCollider con tamaño 1x2x1
```

### Prefabs

| Herramienta | Descripción |
|---|---|
| `create_prefab` | Crea un prefab desde un GameObject |
| `read_prefab` | Lee la estructura de un prefab |
| `write_prefab` | Modifica un prefab |
| `delete_prefab` | Elimina un prefab |

**Ejemplo:**
```
Lee el prefab Assets/Prefabs/Characters/Player.prefab y muéstrame su estructura
```

### Escenas

- Ver la jerarquía actual
- Listar GameObjects
- Acceder a propiedades de objetos

**Ejemplo:**
```
Muéstrame todos los GameObjects en la escena actual con sus componentes
```

---

## MCP for Blender (20+ Herramientas)

### Modelado

| Herramienta | Descripción | Parámetros |
|---|---|---|
| `create_model` | Crea un modelo 3D básico | `name`, `type` (humanoid, cube, sphere, etc.), `height`, `width`, `depth` |
| `edit_model` | Modifica un modelo | `blend_file`, `modifications` |
| `delete_model` | Elimina un modelo | `blend_file`, `object_name` |

**Ejemplo:**
```
Crea un modelo humanoid llamado "Character_01" con altura 1.8
```

### Rigging

| Herramienta | Descripción |
|---|---|
| `add_rig` | Agrega un rig/armadura a un objeto |

**Parámetros:** `blend_file`, `object_name`

**Ejemplo:**
```
Al modelo "Character_01", agrega un rig automático
```

### Animaciones

| Herramienta | Descripción |
|---|---|
| `create_animation` | Crea una nueva animación |
| `edit_animation` | Modifica una animación existente |

**Parámetros:** `blend_file`, `animation_name`, `keyframes`

### Exportación

| Herramienta | Descripción | Parámetros |
|---|---|---|
| `export_fbx` | Exporta a FBX | `blend_file`, `output_path`, `apply_modifiers`, `scale` |
| `export_glb` | Exporta a GLB/GLTF | `blend_file`, `output_path` |
| `batch_export` | Exporta múltiples archivos | `folder`, `format` |

**Ejemplo:**
```
Exporta el archivo Assets/Models/Characters/Character_01.blend como FBX a Assets/Models/Characters/Character_01.fbx
```

### Utilidades

| Herramienta | Descripción |
|---|---|
| `list_objects` | Lista objetos en un archivo .blend |
| `run_python` | Ejecuta script Python personalizado en Blender |
| `get_scene_info` | Obtiene información de la escena |
| `get_object_info` | Obtiene información de un objeto |
| `get_viewport_screenshot` | Captura del viewport |

**Ejemplo:**
```
Toma una captura del viewport actual de Blender
```

### Integraciones (si están habilitadas)

#### Poly Haven
- `search_polyhaven_assets` - Buscar assets
- `download_polyhaven_asset` - Descargar asset

**Ejemplo:**
```
Busca en Poly Haven un modelo de "sword" y descárgalo
```

#### Sketchfab
- `search_sketchfab_models` - Buscar modelos
- `get_sketchfab_model_preview` - Ver preview
- `download_sketchfab_model` - Descargar modelo

**Ejemplo:**
```
Busca un modelo de "goblin" en Sketchfab
```

#### Generación de IA (Hyper3D)
- `generate_hyper3d_model_via_text` - Generar desde texto
- `generate_hyper3d_model_via_images` - Generar desde imágenes
- `get_hyper3d_status` - Ver estado
- `import_generated_asset` - Importar resultado

**Ejemplo:**
```
Genera un modelo 3D de un "alien warrior" usando Hyper3D
```

#### Generación de IA (Hunyuan 3D - Tencent)
- `generate_hunyuan3d_model` - Generar modelo
- `get_hunyuan3d_status` - Ver estado
- `poll_hunyuan_job_status` - Verificar progreso
- `import_generated_asset_hunyuan` - Importar resultado

**Ejemplo:**
```
Genera un modelo de "dragon" usando Hunyuan 3D
```

---

## Patrones Comunes

### Patrón 1: Crear y Compilar Script

```
1. create_script (crea el archivo)
2. compile_project (verifica que no hay errores)
3. get_errors (muestra errores si los hay)
```

### Patrón 2: Modelar y Exportar

```
1. create_model (crea modelo en Blender)
2. add_rig (agrega esqueleto)
3. export_fbx (exporta a FBX)
4. create_prefab (crea prefab en Unity)
```

### Patrón 3: Iterar Script

```
1. read_script (lee código actual)
2. modify_script (realiza cambios)
3. compile_project (verifica compilación)
4. get_errors (muestra problemas si existen)
```

---

## Convenciones de Rutas

### Unity

```
Assets/
├── Scripts/[Category]/[FileName].cs
├── Prefabs/[Type]/[Name].prefab
├── Scenes/[Name].unity
├── Models/[Type]/[Name].fbx
└── Materials/[Name].mat
```

### Blender

```
BlenderAssets/
└── [Name].blend
```

### Exportación (Blender → Unity)

```
Blender file: BlenderAssets/Character_01.blend
Export to: Assets/Models/Characters/Character_01.fbx
```

---

## Notas Técnicas

### Timeouts
- Blender puede tardar en operaciones complejas (máximo 180 segundos)
- Unity compila rápido (máximo 60 segundos)

### Limitaciones de Tamaño
- Scripts: hasta 1MB
- Modelos FBX: hasta 100MB
- Herramientas respetan sandboxing de seguridad

### Validación
- Las rutas se validan automáticamente (no pueden salir de Assets/)
- Los parámetros se validan contra su esquema definido
- Los errores se reportan con sugerencias de corrección

---

**Volver a:** [02 - Guía de Uso](02_Uso.md) | [README](README.md)
