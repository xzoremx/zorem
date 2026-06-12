# MCPs - ZoremGame

Documentación oficial sobre los Model Context Protocol (MCPs) utilizados en ZoremGame para integrar Claude Code directamente con Unity y Blender.

## 📚 Índice de Documentación

1. **[Instalación y Configuración](01_Instalacion.md)** - Cómo instalar y activar los MCPs
2. **[Guía de Uso](02_Uso.md)** - Cómo utilizar los MCPs con Claude Code
3. **[Referencia de Herramientas](03_Referencia.md)** - Listado de tools disponibles en cada MCP

## 🎯 Resumen Rápido

### MCPs Disponibles

| MCP | Tipo | Puerto | Estado |
|-----|------|--------|--------|
| **MCP for Unity** | Addon de Unity | 8080 | Funcional |
| **MCP for Blender** | Addon de Blender | 9876 | Funcional |

### Inicio Rápido

1. Abre Unity → `Window > MCP For Unity > Toggle MCP Window` → Haz clic en **Start Server**
2. Abre Blender → Panel lateral (N) → Pestaña **Blender MCP** → Confirma servidor activo
3. En Claude Code, ejecuta `/mcp` para verificar conexión

## 📋 Requisitos

- **Unity**: 2022.3.39f1 LTS (con addon MCP for Unity instalado)
- **Blender**: 5.x (con addon Blender MCP instalado)
- **Claude Code**: Versión actual
- **.mcp.json**: Configurado en la raíz del proyecto

## 🔗 Archivos de Configuración

- `.mcp.json` - Configuración de los MCPs para Claude Code
  - Define `unity`: http://localhost:8080/mcp
  - Define `blender`: blender-mcp vía uvx.exe

## ⚠️ Notas Importantes

- El servidor MCP de Unity **se detiene** cuando cierras Unity. Reinicia cada sesión.
- Si pierdes conexión, reinicia el servidor desde el panel del addon.
- El arquivo `.mcp.json` en la raíz del proyecto registra ambos servidores automáticamente.

---

**Última actualización:** 2026-06-11  
**Estado:** Documentación en uso
