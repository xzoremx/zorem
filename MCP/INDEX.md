# MCP - Índice Rápido

Documentación oficial de los MCPs para ZoremGame.

## 📚 Documentación

- **[README](docs/README.md)** - Visión general y requisitos
- **[01 - Instalación](docs/01_Instalacion.md)** - Cómo instalar y activar los MCPs
- **[02 - Guía de Uso](docs/02_Uso.md)** - Cómo usar los MCPs con Claude Code
- **[03 - Referencia](docs/03_Referencia.md)** - Listado completo de herramientas

## ⚡ Inicio Rápido

```bash
# 1. Abre Unity
Window > MCP For Unity > Toggle MCP Window (Ctrl+Shift+M)
Haz clic en "Start Server"

# 2. Abre Blender
Panel lateral (N) > Pestaña "Blender MCP" > Verificar servidor activo

# 3. En Claude Code, verifica:
/mcp

# 4. Listo para trabajar
```

## 📁 Estructura

```
MCP/
├── INDEX.md               (este archivo)
├── docs/
│   ├── README.md
│   ├── 01_Instalacion.md
│   ├── 02_Uso.md
│   └── 03_Referencia.md
├── .mcp.json             (configuración de Claude Code)
└── scripts/              (utilidades, si existen)
```

## 🎯 MCPs Disponibles

| MCP | Tipo | Puerto | Tools |
|-----|------|--------|-------|
| **MCP for Unity** | Addon | 8080 | 30+ (scripts, GameObjects, compilación) |
| **MCP for Blender** | Addon | 9876 | 20+ (modelado, rigging, exportación) |

## 📖 Documentación por Tarea

### Quiero crear un script C#
→ [02 - Guía de Uso: Crear Scripts](docs/02_Uso.md#crear-scripts-c)

### Quiero modelar un personaje
→ [02 - Guía de Uso: Trabajar con Blender](docs/02_Uso.md#trabajar-con-mcp-blender)

### Quiero saber qué herramientas hay
→ [03 - Referencia](docs/03_Referencia.md)

### Tengo un problema
→ [01 - Instalación: Troubleshooting](docs/01_Instalacion.md#troubleshooting)

---

**Última actualización:** 2026-06-11
