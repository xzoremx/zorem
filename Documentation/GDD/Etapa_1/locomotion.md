# Locomotion System — Etapa 1
> Referencia: Fortnite 2026 | Personaje: ZoremGame Player

---

## Sistemas a implementar

### 1. Sprint (DONE parcial)
- **Tecla:** `Shift` (manual, no auto-sprint)
- **Estado actual:** funciona con `PlayerInput.SprintInput()`
- **Pendiente:** asegurar transición suave de animación walk → run → sprint

---

### 2. Slide
- **Activación:** Sprint + `Ctrl`
- **Comportamiento:** deslizamiento plano (sin física de pendiente)
- **Duración:** fija (~1.0s) o hasta que el jugador suelte o salte
- **Hitbox:** CapsuleCollider se reduce al agacharse durante el slide
- **Animación:** transición a pose de slide
- **Scripts a crear/modificar:** `PlayerInput`, `PlayerController`, `PlayerAnimator`

---

### 3. Vault (Auto-Vault al saltar)
- **Activación:** jugador presiona `Space` cerca de un obstáculo bajo
- **Detección:** raycast hacia adelante al inicio del salto
- **Condición:** obstáculo entre ~0.3m y ~1.2m de altura
- **Comportamiento:** en lugar del salto normal, reproduce animación de vault y mueve al personaje encima del obstáculo
- **Animación:** CrossFade a estado "Vault" en el Animator
- **Scripts a crear:** `VaultDetector.cs` (componente en Player)

---

### 4. Coyote Time
- **Duración:** 0.15 segundos
- **Comportamiento:** al salir del suelo sin saltar (caída libre desde borde), el jugador puede saltar durante 0.15s como si siguiera en el suelo
- **Implementación:** timer en `PlayerMotor` — se activa cuando `isGrounded` pasa de `true` a `false` sin que `isJumping` sea `true`

---

### 5. Jump Buffering
- **Ventana:** 0.1 segundos antes de aterrizar
- **Comportamiento:** si se presiona `Space` mientras se está en el aire y quedan <0.1s para tocar el suelo, el salto se ejecuta automáticamente al aterrizar
- **Implementación:** buffer timer en `PlayerInput.JumpInput()` — guarda el intent y lo consume en el primer frame de `isGrounded`

---

### 6. Crouch
- **Tecla:** `Ctrl` (sin sprint = crouch normal)
- **Comportamiento:** personaje se agacha, velocidad reducida (~50%), hitbox baja
- **CapsuleCollider:** height reducida a ~1.0m, center ajustado
- **Cámara:** baja con el personaje (ThirdPersonCamera ya sigue el target)
- **Toggle o hold:** hold (suelta Ctrl para levantarse)
- **Verificación al levantarse:** raycast hacia arriba para evitar levantarse dentro de geometría

---

### 7. Landing Recovery
- **Activación:** al aterrizar después de una caída
- **Comportamiento proporcional a la altura:**
  | Altura de caída | Duración animación | Bloqueo de movimiento |
  |---|---|---|
  | < 2m | Sin animación | No |
  | 2m – 5m | Corta (~0.3s) | Leve |
  | 5m – 10m | Media (~0.6s) | Moderado |
  | > 10m | Larga (~1.0s) | Total |
- **Sin daño por caída**
- **Implementación:** registrar `heightReached` al despegar (ya existe en `PlayerMotor`), calcular delta al aterrizar
- **Animaciones necesarias:** `LandLight`, `LandMedium`, `LandHard` en el Animator

---

### 8. Camera Tilt
- **Activación:** movimiento lateral (strafe o dirección diagonal)
- **Efecto:** rotación en Z de la cámara ±3° con lerp suave
- **Implementación:** en `ThirdPersonCamera.CameraMovement()`, aplicar `Quaternion.Euler(mouseY, mouseX, tiltZ)`

---

### 9. FOV Boost al Sprint
- **FOV normal:** 60°
- **FOV al sprintar:** 65–68°
- **Transición:** Lerp suave (~5 unidades/s)
- **Implementación:** en `ThirdPersonCamera`, detectar `playerController.isSprinting` y ajustar `Camera.fieldOfView`

---

### 10. Step-Up Suave
- **Altura máxima de step:** ~0.35m
- **Comportamiento:** al caminar contra un escalón pequeño, el personaje sube suavemente sin saltar (interpolación de posición Y)
- **Implementación:** en `PlayerMotor.MoveCharacter()`, SphereCast hacia adelante-abajo para detectar step y aplicar offset vertical
- **Diferencia con Vault:** step-up es para escalones muy bajos y es silencioso; vault es para obstáculos más altos con animación

---

## Estado de implementación

| Sistema | Estado |
|---|---|
| Sprint (Shift) | ✅ Funciona |
| Slide | ❌ Pendiente |
| Vault | ❌ Pendiente |
| Coyote Time | ❌ Pendiente |
| Jump Buffering | ❌ Pendiente |
| Crouch | ❌ Pendiente |
| Landing Recovery | ❌ Pendiente |
| Camera Tilt | ❌ Pendiente |
| FOV Boost Sprint | ❌ Pendiente |
| Step-Up Suave | ❌ Pendiente |

---

## Archivos afectados

| Archivo | Cambios |
|---|---|
| `Assets/Scripts/Player/PlayerInput.cs` | Slide, Crouch, Jump Buffer inputs |
| `Assets/Scripts/Player/PlayerMotor.cs` | Coyote time, Step-up, Landing height tracker |
| `Assets/Scripts/Player/PlayerController.cs` | Slide state, Crouch state, Landing recovery |
| `Assets/Scripts/Player/PlayerAnimator.cs` | Nuevos parámetros: IsSliding, IsCrouching, LandType |
| `Assets/Scripts/Player/ThirdPersonCamera.cs` | Camera tilt, FOV boost |
| `Assets/Scripts/Player/VaultDetector.cs` | **Nuevo** — detección y ejecución de vault |

---

## Animaciones requeridas en el Animator Controller

| Estado | Trigger/Bool |
|---|---|
| Run Custom | Reemplaza el run existente |
| Slide | `IsSliding` (bool) |
| Crouch Idle | `IsCrouching` (bool) |
| Crouch Walk | `IsCrouching` + `InputMagnitude > 0` |
| Vault | `Vault` (trigger) |
| LandLight | `LandType = 1` (int) |
| LandMedium | `LandType = 2` (int) |
| LandHard | `LandType = 3` (int) |

---

## Pipeline de Animaciones (Blender → Unity)

### Paso 1: Exportar modelo desde Unity
- Exportar VBOT2.0 como `.fbx` desde Unity (`Assets > Export Package` o copiar el .fbx original)
- Ruta original: `Assets/Invector-3rdPersonController_LITE/3D Models/Characters/Invector@V-Bot 2.0/`

### Paso 2: Importar en Blender
- Abrir Blender (con MCP server activo)
- `File > Import > FBX` con el modelo VBOT2.0
- Verificar que el armature/skeleton esté correcto

### Paso 3: Crear animaciones en Blender
- Entrar en **Pose Mode** sobre el armature
- Crear nueva Action para cada animación (Run Custom, Slide, Crouch, Vault, LandLight/Medium/Hard)
- Insertar keyframes (`I`) en las poses clave
- Ajustar curvas en el Graph Editor para suavidad

### Paso 4: Exportar desde Blender
- Seleccionar armature + mesh
- `File > Export > FBX`
- Configuración:
  - Scale: 1.0
  - Apply Scalings: FBX All
  - Forward: -Z Forward
  - Up: Y Up
  - Armature > Primary Bone Axis: Y
  - Bake Animation: ✅ activado
  - NLA Strips / All Actions: según necesidad

### Paso 5: Importar en Unity
- Copiar `.fbx` a `Assets/Animations/Custom/`
- En el Inspector del FBX:
  - Tab **Rig** > Animation Type: **Humanoid** > Apply
  - Tab **Animation** > verificar clips, ajustar loop si necesario
- Arrastrar clips al Animator Controller (`DefaultPlayerController.controller`)
- Configurar transiciones y parámetros

### Animaciones a crear (orden de prioridad)

| Animación | Prioridad | Complejidad | Frames estimados |
|---|---|---|---|
| Run Custom | Alta | Media | 16-24 frames (loop) |
| Slide | Alta | Baja | 8-12 frames + hold |
| Crouch Idle | Media | Baja | 1 pose + breathing loop |
| Crouch Walk | Media | Media | 16 frames (loop) |
| Vault | Media | Alta | 24-36 frames |
| LandLight | Baja | Baja | 8 frames |
| LandMedium | Baja | Media | 12 frames |
| LandHard | Baja | Media | 18 frames |
