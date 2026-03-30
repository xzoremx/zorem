# MCP Project - Índice Rápido

## 📚 Documentación Completa

### Para Empezar
1. **[README.md](docs/README.md)** - Visión general y estructura del proyecto (START HERE)

### Entender el Concepto
2. **[01_Vision.md](docs/01_Vision.md)** - ¿Qué son MCPs y por qué los necesitamos?
3. **[02_Arquitectura.md](docs/02_Arquitectura.md)** - Cómo funciona la arquitectura completa

### Implementación
4. **[03_Unity_MCP.md](docs/03_Unity_MCP.md)** - Pasos para construir MCP Unity (5 Etapas)
5. **[04_Blender_MCP.md](docs/04_Blender_MCP.md)** - Pasos para construir MCP Blender (5 Etapas)

### Deployment
6. **[05_Setup.md](docs/05_Setup.md)** - Configuración final y troubleshooting
7. **[06_Testing.md](docs/06_Testing.md)** - Estrategia de testing y validación

### Referencia
8. **[07_Referencia_Tecnica.md](docs/07_Referencia_Tecnica.md)** - APIs, ejemplos, y cheat sheets

---

## 🎯 Roadmap de Implementación (Esta Semana)

```
Día 1-2: Setup
  ├─ Instalar Node.js, TypeScript, dependencias
  ├─ Crear estructura de carpetas
  └─ Verificar instalaciones de Unity y Blender

Día 2-3: MCP Unity Fase 1
  ├─ Crear servidor Node.js base
  ├─ Implementar CreateScript
  └─ Testing básico

Día 4-5: MCP Unity Fase 2-4
  ├─ Implementar DeleteScript, GetErrors
  ├─ Implementar GameObjects y Prefabs
  └─ Implementar Compilación y Build

Día 5-6: MCP Blender Fase 1-2
  ├─ Crear servidor Blender
  ├─ Implementar CreateModel
  └─ Implementar ExportFBX

Día 6-7: Integración y Testing
  ├─ Registrar en settings.json
  ├─ Testing end-to-end
  └─ Documentar bugs encontrados
```

---

## 📂 Estructura de Carpetas

```
MCP/
├── INDEX.md                       ← TÚ ESTÁS AQUÍ
├── docs/
│   ├── README.md
│   ├── 01_Vision.md
│   ├── 02_Arquitectura.md
│   ├── 03_Unity_MCP.md
│   ├── 04_Blender_MCP.md
│   ├── 05_Setup.md
│   ├── 06_Testing.md
│   └── 07_Referencia_Tecnica.md
├── servers/
│   ├── unity/
│   │   ├── src/
│   │   ├── build/
│   │   ├── package.json
│   │   └── tsconfig.json
│   └── blender/
│       ├── src/
│       ├── python/
│       ├── build/
│       ├── package.json
│       └── tsconfig.json
├── scripts/
│   ├── setup.sh
│   ├── test-unity.sh
│   └── test-blender.sh
└── config/
    └── settings-example.json
```

---

## ⚡ Cheat Sheet - Comandos Rápidos

### Compilar servidores
```bash
cd MCP/servers/unity && npm install && npm run build
cd MCP/servers/blender && npm install && npm run build
```

### Iniciar servidores (testing)
```bash
# Terminal 1
cd MCP/servers/unity && AUTO_COMPILE=false npm start

# Terminal 2
cd MCP/servers/blender && npm start
```

### Verificar instalaciones
```bash
node --version          # v18+
npm --version           # v9+
unity --version         # Cualquier versión reciente
blender --version       # 3.0+
```

---

## ✅ Checklist de Implementación

### Setup (Día 1-2)
- [ ] Node.js 18+ instalado
- [ ] npm 9+ instalado
- [ ] TypeScript instalado globalmente
- [ ] Unity 2022+ verificado
- [ ] Blender 3.0+ verificado
- [ ] Carpetas MCP creadas

### Unity MCP (Día 2-5)
- [ ] package.json creado
- [ ] tsconfig.json configurado
- [ ] types.ts definido
- [ ] unity-cli.ts implementado
- [ ] CreateScript funciona
- [ ] DeleteScript funciona
- [ ] GetErrors funciona
- [ ] CreateGameObject funciona
- [ ] ConfigureComponent funciona
- [ ] CreatePrefab funciona
- [ ] CompileProject funciona

### Blender MCP (Día 5-6)
- [ ] package.json creado
- [ ] tsconfig.json configurado
- [ ] types.ts definido
- [ ] blender-cli.ts implementado
- [ ] CreateModel funciona
- [ ] ExportFBX funciona
- [ ] AddRig funciona
- [ ] CreateAnimation funciona
- [ ] BatchExport funciona

### Integración (Día 6-7)
- [ ] Ambos servidores compilan sin errores
- [ ] settings.json configurado
- [ ] MCPs aparecen en Claude Code
- [ ] Test: Crear script en Unity
- [ ] Test: Crear modelo en Blender
- [ ] Test: Exportar FBX a Unity
- [ ] End-to-End test completo

---

## 🔗 Links Útiles

- **MCP Protocol:** https://modelcontextprotocol.io/
- **Unity Scripting:** https://docs.unity3d.com/ScriptReference/
- **Blender Python:** https://docs.blender.org/api/current/
- **Node.js Child Process:** https://nodejs.org/api/child_process.html

---

## 📞 Troubleshooting Rápido

| Problema | Solución |
|----------|----------|
| "Unknown MCP" | Compilar: `npm run build`, reiniciar Claude Code |
| "Unity not found" | Verificar UNITY_EXECUTABLE en settings.json |
| "Blender not found" | Verificar BLENDER_EXECUTABLE en settings.json |
| Timeout en operación | Aumentar timeout en handler (cambiar ms) |
| Permission denied | `chmod +x /path/to/unity` o `chmod +x /path/to/blender` |
| Port already in use | MCP usa stdio, no puertos. Ver otro proceso corriendo |

---

## 🚀 Próximos Pasos

1. **Lee primero:** [docs/README.md](docs/README.md)
2. **Entiende:** [docs/01_Vision.md](docs/01_Vision.md) y [docs/02_Arquitectura.md](docs/02_Arquitectura.md)
3. **Implementa:** Sigue [docs/03_Unity_MCP.md](docs/03_Unity_MCP.md) paso a paso
4. **Luego:** [docs/04_Blender_MCP.md](docs/04_Blender_MCP.md)
5. **Configura:** [docs/05_Setup.md](docs/05_Setup.md)
6. **Testea:** [docs/06_Testing.md](docs/06_Testing.md)
7. **Consulta:** [docs/07_Referencia_Tecnica.md](docs/07_Referencia_Tecnica.md) cuando necesites

---

**Documentación completa y lista para implementar. ¡Adelante!**
