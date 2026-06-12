# 02 - Guía de Uso: MCPs con Claude Code

## Flujo de Trabajo Estándar

### Paso 1: Verificar Servidores Activos

En Claude Code, ejecuta:
```
/mcp
```

Deberías ver ambos servidores conectados:
```
✓ unity (30+ herramientas disponibles)
✓ blender (20+ herramientas disponibles)
```

---

## Trabajar con MCP Unity

### Verificar Escena Actual

```
Lee la jerarquía de la escena activa en Unity
```

Claude accederá al editor en tiempo real y mostrará todos los GameObjects activos.

### Crear Scripts C#

```
Crea un script PlayerController en Assets/Scripts/Player/ que implemente:
- Movimiento WASD
- Salto con Space
- Sprint con Shift
```

El script se creará automáticamente en el proyecto.

### Modificar Objetos

```
En la escena actual:
1. Crea un Cube
2. Asígnale el material Red
3. Configura su velocidad inicial a 5 m/s
```

### Gestionar Prefabs

```
Lee el prefab Assets/Prefabs/Characters/Player.prefab y muéstrame su estructura
```

### Compilar y Verificar Errores

```
Compila el proyecto y dime si hay errores de compilación
```

---

## Trabajar con MCP Blender

### Ver la Escena Actual

```
Muéstrame los objetos en la escena actual de Blender
```

### Crear Modelos 3D

```
Crea un modelo humanoid básico en Blender con las siguientes proporciones:
- Altura: 1.8 metros
- Complexión: atlética
```

### Inspeccionar Objetos

```
Inspecciona el objeto "Character_01" en Blender. Muéstrame:
- Vertices y faces
- Materiales asignados
- Bones si tiene rig
```

### Agregar Rigs

```
Al modelo "Character_01", agrega un rig humano con estructura Rigify
```

### Exportar Modelos

```
Exporta el modelo "Character_01" como FBX a Assets/Models/Characters/Character_01.fbx
```

### Captura de Viewport

```
Toma una captura de pantalla del viewport actual de Blender
```

### Usar Generadores de Modelos (si están habilitados)

Si habilitaste las integraciones extra:

**Poly Haven (Assets gratuitos):**
```
Busca en Poly Haven un modelo de "sword" y descárgalo
```

**Sketchfab:**
```
Busca un modelo de "goblin" en Sketchfab y muéstrame opciones
```

**Generación de IA:**
```
Genera un modelo 3D de un "alien warrior" usando Hyper3D
```

---

## Flujos Comunes

### Crear un Personaje Completo

**Paso 1: Modelado en Blender**
```
En Blender, crea un modelo humanoide básico con nombre "Hero_01"
```

**Paso 2: Agregar Rig**
```
Al modelo "Hero_01", agrega un rig Rigify automático
```

**Paso 3: Exportar**
```
Exporta "Hero_01" como FBX a Assets/Models/Characters/Hero_01.fbx
```

**Paso 4: Crear Prefab en Unity**
```
En Unity:
1. Importa el FBX en Assets/Models/Characters/
2. Crea un prefab en Assets/Prefabs/Characters/Hero_01.prefab
3. Configura el Animator controller
```

### Crear un Script de Comportamiento

**Paso 1: Generar Script**
```
Crea un script "BossAI.cs" en Assets/Scripts/Boss/ que implemente:
- Patrón de patrulla
- Detección del jugador
- Ataque básico
- Sistema de salud
```

**Paso 2: Compilar**
```
Compila y reporta errores
```

**Paso 3: Crear Prefab**
```
Crea un GameObject con el script y guárdalo como prefab Assets/Prefabs/Bosses/Boss_01.prefab
```

---

## Mejores Prácticas

### 1. Estructura de Carpetas
Siempre crea recursos siguiendo la estructura del proyecto:
- Scripts: `Assets/Scripts/[Category]/[Name].cs`
- Modelos: `Assets/Models/[Type]/[Name].fbx`
- Prefabs: `Assets/Prefabs/[Type]/[Name].prefab`

### 2. Nombrado
Usa convenciones consistentes:
- Scripts: PascalCase (`PlayerController.cs`)
- GameObjects: PascalCase (`Enemy_01`)
- Archivos: PascalCase (`Character_Hero.fbx`)

### 3. Verificación
Siempre verifica que no hay errores después de:
- Crear scripts nuevos
- Importar modelos
- Modificar componentes

```
Compila y muéstrame si hay errores
```

### 4. Iteración
El flujo recomendado es:
1. **Describe** lo que quieres
2. **Verifica** que se creó correctamente
3. **Ajusta** si es necesario
4. **Repite**

```
Crea [cosa]
↓
¿Se ve bien?
↓
Sí → Siguiente tarea
No → Ajusta parámetros → Repite
```

---

## Limitaciones Conocidas

- Los MCPs no pueden hacer decisiones artísticas (qué se vea bien es subjetivo)
- La generación de IA (Hyper3D, Hunyuan) requiere tiempo extra
- Los modelos generados pueden necesitar refinamiento manual
- Los scripts grandes pueden requerir múltiples iteraciones

---

## Solución de Problemas

### "Tool not found"
El MCP perdió conexión. Ejecuta `/mcp` y reinicia los servidores.

### "Unity editor not responding"
Unity está compilando o procesando. Espera un momento y reinteneta.

### "Blender timed out"
La operación tardó mucho. Intenta con una tarea más simple primero.

### Claude sugiere algo que los MCPs no pueden hacer
Los MCPs tienen herramientas limitadas. Algunas cosas aún requieren trabajo manual en los editores.

---

**Siguiente:** [03 - Referencia de Herramientas](03_Referencia.md)
