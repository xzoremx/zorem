# Player Controller Setup Guide

## Automated Setup (Recommended)

En Unity, abre el menú **ZoremGame > Quick Setup** y sigue estos pasos en orden:

### 1. Create Animator Controller
`ZoremGame > Quick Setup > 1. Create Animator Controller`

Esto genera el AnimatorController con:
- Todos los parámetros necesarios (InputMagnitude, IsGrounded, etc.)
- Estados básicos (Idle, Walk, Run, Sprint, Jump, Fall)
- Transiciones automáticas

**Guardado en:** `Assets/Scripts/Player/Animators/DefaultPlayerController.controller`

---

### 2. Create Player from Scratch
`ZoremGame > Quick Setup > 2. Create Player from Scratch`

Esto crea automáticamente:
- **Player** (GameObject raíz con todos los componentes)
  - **Body** (visual, una cápsula por ahora)
  - Rigidbody (con rotación congelada)
  - CapsuleCollider (2m alto, 0.4m radio)
  - Animator (con DefaultPlayerController asignado)
  - PlayerInput (script de input)
- **MainCamera** con ThirdPersonCamera
- **Ground** (plano para testing)

---

### 3. Setup Existing Player
`ZoremGame > Quick Setup > 3. Setup Existing Player`

Si ya tienes un personaje en la escena:
1. Selecciona el GameObject del personaje
2. Ejecuta este comando
3. Se añaden automáticamente todos los componentes necesarios

---

### 4. Test Scene Setup
`ZoremGame > Quick Setup > 4. Test Scene Setup`

Verifica que todo está listo. Luego presiona **Play** para probar.

---

## Manual Setup (Si prefieres)

Si quieres configurar todo manualmente:

### Estructura de GameObject
```
Player
├── Body (visual mesh/model)
├── Rigidbody (constraints: Freeze Rotation)
├── CapsuleCollider (2m x 0.4m)
├── Animator (controller: DefaultPlayerController)
└── PlayerInput (script)

MainCamera
├── Camera
└── ThirdPersonCamera (target: Player transform)
```

### Componentes Requeridos
| Componente | Configuración |
|-----------|--------------|
| **Rigidbody** | Mass: 1, Linear Damping: 0, Angular Damping: 0.05, Freeze Rotation: XYZ |
| **CapsuleCollider** | Height: 2, Radius: 0.4, Center: (0, 0, 0) |
| **Animator** | Update Mode: Normal, Controller: DefaultPlayerController |
| **PlayerInput** | (sin configuración especial) |
| **ThirdPersonCamera** | Target: Player transform |

---

## Input Mapping

Por defecto:
- **WASD** — Movimiento
- **Mouse** — Rotación de cámara
- **Shift** — Sprint (toggle)
- **Tab** — Strafe (toggle)
- **Space** — Saltar

Editable en `PlayerInput.cs` (Inspector).

---

## Animator Parameters

El controller define automáticamente estos parámetros:

| Parámetro | Tipo | Propósito |
|-----------|------|----------|
| `InputMagnitude` | Float | Velocidad de movimiento normalizada (0-1.5) |
| `InputHorizontal` | Float | Input lateral en strafe |
| `InputVertical` | Float | Input de movimiento adelante/atrás |
| `IsGrounded` | Bool | ¿Está el jugador en el suelo? |
| `IsStrafing` | Bool | ¿Está en modo strafe? |
| `IsSprinting` | Bool | ¿Está corriendo? |
| `GroundDistance` | Float | Distancia al suelo (para culling) |

---

## Animaciones Esperadas

El controller espera estas animaciones en tu Animator:

- **Idle** — Posición neutra
- **Walk** — Caminar (blendable por InputMagnitude)
- **Run** — Correr (blendable por InputMagnitude)
- **Sprint** — Sprint máximo
- **Jump** — Salto
- **JumpMove** — Salto con movimiento
- **Fall** — Caída en el aire

---

## Próximos Pasos

1. **Anima los estados** — Reemplaza las transiciones genéricas con animaciones reales
2. **Configura el Animator Controller** en el Inspector:
   - Motion blend trees si quieres transiciones suaves
   - Ajusta exit times si es necesario
3. **Prueba el movimiento** — Play y verifica que todo funciona
4. **Integración Symbiote** — Luego agregarás web shooters y parkour

---

## Troubleshooting

| Problema | Solución |
|----------|----------|
| Player no se mueve | Verifica que `PlayerInput` esté en el GameObject |
| Cámara no sigue | Asigna el transform del Player en `ThirdPersonCamera.target` |
| Animaciones no cambian | Verifica que todos los parámetros existan en el AnimatorController |
| Player se cae infinitamente | Asegúrate que hay un Ground con BoxCollider en layer "Default" |

---

**¿Preguntas?** Revisa los scripts en `Assets/Scripts/Player/`
