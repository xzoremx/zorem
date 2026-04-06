# Player Controller System

## 📦 Estructura

```
Assets/Scripts/Player/
├── PlayerMotor.cs             (Física, ground check, saltos, gravedad)
├── PlayerAnimator.cs          (Sincronización Animator)
├── PlayerController.cs        (Control de movimiento, sprint, strafe, jump)
├── PlayerInput.cs             (Manejo de input)
├── ThirdPersonCamera.cs       (Cámara orbital con collision avoidance)
├── Editor/
│   ├── QuickPlayerSetup.cs    (Menú rápido de setup)
│   ├── PlayerSetupEditor.cs   (Ventana de configuración)
│   └── AnimatorSetup.cs       (Generador de AnimatorController)
├── Animators/
│   └── DefaultPlayerController.controller (Auto-generado)
├── SETUP_GUIDE.md             (Guía de configuración)
└── README.md                  (Este archivo)
```

## 🔗 Jerarquía de Herencia

```
PlayerMotor (Física base)
    ↓
PlayerAnimator (+ Animator sync)
    ↓
PlayerController (+ Control de movimiento)
    ↓
PlayerInput (+ Manejo de input)
```

## 🎮 Como Usar

### Opción A: Setup Automatizado (Recomendado)
```
1. En Unity: ZoremGame > Quick Setup > 1. Create Animator Controller
2. En Unity: ZoremGame > Quick Setup > 2. Create Player from Scratch
3. Click Play
```

**Tiempo:** ~2 minutos

### Opción B: Con tu Personaje Existente
```
1. ZoremGame > Quick Setup > 1. Create Animator Controller
2. Selecciona tu GameObject con Animator
3. ZoremGame > Quick Setup > 3. Setup Existing Player
4. Verifica que el Animator tiene animaciones
5. Click Play
```

---

## 📋 Qué Hace Cada Script

### PlayerMotor
**Responsabilidades:**
- Ground check (raycast + spherecast)
- Física (gravedad extra, velocidad terminal)
- Saltos (duración, altura, arc)
- Slope limit (ángulo máximo para caminar)
- Rotación del personaje
- Materiales de fricción dinámicos

**Public Methods:**
- `Init()` — Inicialización (llamado automáticamente)
- `UpdateMotor()` — Actualiza física cada frame
- `SetControllerMoveSpeed()` — Ajusta velocidad (walk/run/sprint)
- `MoveCharacter()` — Mueve según dirección
- `Jump()` — Salta
- `RotateToDirection()` — Rota hacia dirección

**Parámetros Inspectables:**
- `useRootMotion` — Si usa animaciones root motion
- `locomotionType` — FreeWithStrafe, OnlyStrafe, OnlyFree
- `freeSpeed` / `strafeSpeed` — Velocidades (walk, run, sprint)
- `jumpHeight`, `jumpTimer` — Control del salto
- `extraGravity` — Gravedad adicional
- `slopeLimit` — Ángulo máximo de pendiente

---

### PlayerAnimator
**Responsabilidades:**
- Sincronizar variables físicas → Animator parameters
- Calcular velocidades relativas (vertical, horizontal)
- Mapear InputMagnitude para blending de animaciones

**Public Methods:**
- `UpdateAnimator()` — Actualiza Animator cada frame
- `SetAnimatorMoveSpeed()` — Calcula velocidades relativas

**Animator Parameters que Controla:**
```
InputMagnitude    ← magnitud de movimiento (0-1.5)
InputHorizontal   ← input lateral en strafe (-1 a 1)
InputVertical     ← input frontal (-1 a 1)
IsGrounded        ← bool si está en suelo
IsStrafing        ← bool si está en strafe
IsSprinting       ← bool si está corriendo
GroundDistance    ← distancia al suelo (para animaciones)
```

---

### PlayerController
**Responsabilidades:**
- Locomotion type (free vs strafe)
- Rotation type (hacia cámara o mundo)
- Manejo de sprint y strafe
- Root motion control
- Triggers de jump

**Public Methods:**
- `ControlLocomotionType()` — Aplica velocidad según tipo
- `ControlRotationType()` — Rota según input/cámara
- `ControlAnimatorRootMotion()` — Sincroniza root motion
- `UpdateMoveDirection()` — Calcula dirección relativa a cámara
- `Sprint(bool)` — Toggle sprint
- `Strafe()` — Toggle strafe
- `Jump()` — Inicia jump

---

### PlayerInput
**Responsabilidades:**
- Recolectar input del usuario
- Llamar a PlayerController en los momentos correctos
- Integrar cámara
- Ciclo Update/FixedUpdate/OnAnimatorMove

**Flow Cada Frame:**
```
Start
  ├─ InitializeController()    → busca PlayerController
  ├─ InitializeCamera()        → busca/crea ThirdPersonCamera
  
FixedUpdate (física)
  ├─ playerController.UpdateMotor()
  ├─ playerController.ControlLocomotionType()
  ├─ playerController.ControlRotationType()
  
Update (input)
  ├─ MoveInput()               → lee WASD
  ├─ CameraInput()             → lee Mouse
  ├─ SprintInput()             → lee Shift
  ├─ StrafeInput()             → lee Tab
  ├─ JumpInput()               → lee Space
  ├─ playerController.UpdateAnimator()
  
OnAnimatorMove() (después del Animator)
  └─ playerController.ControlAnimatorRootMotion()
```

---

### ThirdPersonCamera
**Responsabilidades:**
- Órbita alrededor del personaje
- Collision avoidance (acerca cámara si hay obstáculos)
- Culling dinámico (ajusta altura/distancia)
- Rotación suave con mouse

**Public Methods:**
- `Init()` — Inicialización
- `SetTarget()` — Cambia objetivo
- `SetMainTarget()` — Asigna objetivo principal
- `RotateCamera()` — Rota según input X/Y
- `ScreenPointToRay()` — Para raycasts desde pantalla

**Inspector:**
- `defaultDistance` — Distancia deseada de la cámara
- `height` — Altura sobre el personaje
- `smoothCameraRotation` — Suavidad de rotación
- `xMouseSensitivity` / `yMouseSensitivity` — Sensibilidad

---

## 🎯 Flujo de Ejecución (Cada Frame)

```
Frame N
│
├─ Update()
│  ├─ PlayerInput.HandleInput()
│  │  ├─ MoveInput()       → lee WASD → PlayerController.input
│  │  ├─ CameraInput()     → lee Mouse → ThirdPersonCamera.RotateCamera()
│  │  ├─ SprintInput()     → lee Shift → PlayerController.Sprint()
│  │  ├─ JumpInput()       → lee Space → PlayerController.Jump() (si condiciones)
│  │  
│  └─ PlayerController.UpdateAnimator()
│     ├─ SetAnimatorMoveSpeed()
│     └─ animator.SetFloat/SetBool()
│
├─ FixedUpdate()
│  ├─ PlayerController.UpdateMotor()
│  │  ├─ CheckGround()
│  │  ├─ ControlJumpBehaviour()
│  │  ├─ AirControl()
│  │  
│  ├─ PlayerController.ControlLocomotionType()
│  │  └─ MoveCharacter()
│  │
│  ├─ PlayerController.ControlRotationType()
│  │  └─ RotateToDirection()
│
├─ Animator Update
│  └─ OnAnimatorMove()
│     └─ PlayerController.ControlAnimatorRootMotion()
│
├─ FixedUpdate() (Camera)
│  └─ ThirdPersonCamera.CameraMovement()
│     ├─ CullingRayCast()
│     └─ Quaternion.Slerp()
│
Frame N+1
```

---

## 🔧 Integración con Sistemas Futuros

### Parkour (Próximo)
- Agrega métodos a `PlayerMotor`: `WallRun()`, `Vault()`, `Slide()`
- Extiende `PlayerInput` con nuevas teclas
- Añade estados al Animator

### Combate (Fase 2)
- Lockable `lockMovement` (PlayerMotor)
- Lockable `lockRotation` (PlayerMotor)
- Nuevos parámetros Animator: `IsAttacking`, `ComboIndex`, etc.

### Networking (Fase 3)
- Agrega `PredictedObject` (FishNet) a PlayerController
- Sincroniza input a través de red
- Lag compensation para movimiento

---

## 🐛 Debugging

### Gizmos útiles (PlayerMotor)
```csharp
// En OnDrawGizmos, puedes visualizar:
- groundHit.point (punto de suelo)
- moveDirection (dirección actual)
- isGrounded (color rojo si está en suelo)
```

### Valores a Monitorear (Inspector)
```
PlayerMotor:
  - isGrounded (debe alternar entre true/false)
  - groundDistance (debe ser ~0.25 cuando en suelo)
  - isJumping (debe ser true por jumpTimer segundos)
  - moveSpeed (debe cambiar según locomotionType)

PlayerAnimator:
  - inputMagnitude (debe ser 0-1.5 según velocidad)
  - horizontalSpeed, verticalSpeed (según input)

ThirdPersonCamera:
  - distance (debe acercarse si hay obstáculos)
  - mouseX, mouseY (debe seguir mouse input)
```

---

## 📚 Recursos

- **SETUP_GUIDE.md** — Cómo configurar paso a paso
- **PlayerMotor.cs** — Detalles de cada método
- **Editor Scripts** — Automatización

---

## 🚀 Próximo Paso

Una vez que tengas el Player moviendo:
1. Agrega animaciones reales al Animator
2. Ajusta parámetros (jumpHeight, speeds, etc.)
3. Empieza **Parkour** (wall-run, vault, slide)
4. Luego **Combate Melee** (hit detection, daño)

**Roadmap:** `Documentation/GDD/Roadmap.md`
