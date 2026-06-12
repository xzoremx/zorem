# 01 - Instalación y Configuración de MCPs

## Requisitos Previos

- **Unity** 2022.3.39f1 LTS (ya debe tener el addon MCP for Unity)
- **Blender** 5.x (ya debe tener el addon Blender MCP)
- **Claude Code** instalado
- **Python 3.10+** con `uv` (para Blender MCP)
- El archivo **`.mcp.json`** debe estar en la raíz del proyecto

---

## Activación de Servidores

### 1. MCP for Unity

#### Paso 1: Abrir la ventana del MCP

En Unity Editor:
```
Window > MCP For Unity > Toggle MCP Window
```

O usa el atajo: `Ctrl+Shift+M`

#### Paso 2: Iniciar el servidor

En la ventana que se abre, haz clic en el botón **Start Server**.

**Resultado esperado:**
```
INFO: Uvicorn running on http://127.0.0.1:8080
INFO: Plugin registered: ZoremGame
INFO: Registered 30 tools for session ...
```

El servidor estará disponible en `http://127.0.0.1:8080/mcp`

#### Paso 3: Verificar estado

En el panel **Server**:
- Transport: `HTTPLocal` ✓
- HTTP URL: `http://127.0.0.1:8080` ✓
- Local Server: `Conectado` (punto verde) ✓

---

### 2. MCP for Blender

#### Paso 1: Abrir Blender

Abre Blender normalmente. El addon debe estar habilitado automáticamente.

#### Paso 2: Acceder al panel MCP

En el editor de 3D, abre la barra lateral derecha (presiona `N`) y busca la pestaña **Blender MCP**.

#### Paso 3: Activar integraciones (opcional)

Antes de conectar, puedes habilitar integraciones extras:
- ☑️ `Use assets from Poly Haven` - Descargar assets de https://polyhaven.com
- ☑️ `Use assets from Sketchfab` - Descargar modelos de Sketchfab
- ☑️ `Use Hyper3D Rodin 3D model generation` - Generar modelos con IA
- ☑️ `Use Tencent Hunyuan 3D model generation` - Generar modelos con Hunyuan

#### Paso 4: Reconectar/verificar servidor

En el panel **Blender MCP**, verifica que el servidor está disponible (debería mostrar status "Conectado").

El servidor estará disponible en el puerto `9876`.

---

## Configuración en Claude Code

El archivo `.mcp.json` en la raíz del proyecto ya contiene la configuración necesaria:

```json
{
  "mcpServers": {
    "unity": {
      "url": "http://localhost:8080/mcp"
    },
    "blender": {
      "command": "C:/Users/renat/.local/bin/uvx.exe",
      "args": ["blender-mcp"],
      "env": {
        "BLENDER_PORT": "9876"
      }
    }
  }
}
```

Claude Code leerá esta configuración automáticamente.

---

## Verificación de Conexión

### En Claude Code

Ejecuta el comando:
```
/mcp
```

Deberías ver:
- ✓ `unity` - Conectado
- ✓ `blender` - Conectado

Ambos deberían listar sus herramientas disponibles.

---

## Troubleshooting

### Unity no se conecta

**Problema:** Aparece "No Session" en la ventana MCP de Unity

**Solución:**
1. Verifica que el botón **Start Server** se presionó
2. Comprueba que no hay otro proceso escuchando en puerto 8080
3. Reinicia Unity y el servidor MCP

### Blender no aparece en `/mcp`

**Problema:** El comando `/mcp` no muestra el servidor de Blender

**Solución:**
1. Verifica que el addon está habilitado en Blender
2. Asegúrate de que `uvx` está instalado: `uvx --version`
3. Verifica que el puerto 9876 está disponible
4. Reinicia Claude Code después de habilitar el addon en Blender

### Pérdida de conexión

**Problema:** Claude Code pierde conexión con los servidores

**Solución:**
- Para Unity: Reinicia el servidor desde `Window > MCP For Unity > Start Server`
- Para Blender: Reconecta desde el panel **Blender MCP**

---

## Ciclo de Sesión

Cada vez que trabajes:

1. **Abre Unity** y espera a que compile completamente
2. **Inicia el servidor MCP** desde `Window > MCP For Unity`
3. **Abre Blender** (el servidor debe estar listo)
4. **Abre Claude Code** en la carpeta del proyecto
5. **Verifica conexión** con `/mcp`
6. **Comienza a trabajar**

⚠️ **Importante:** El servidor de Unity se detiene automáticamente cuando cierras Unity. Deberás reiniciarlo en la siguiente sesión.

---

**Siguiente:** [02 - Guía de Uso](02_Uso.md)
