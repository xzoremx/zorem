# ZoremGame - Sistemas de Desarrollo (Prioridad)

## Prioridad 1: CORE (Sin esto no hay juego)

### 1.1 Player Controller & Movimiento
- **Descripción:** Control del personaje en 3ª persona, movimiento básico (caminar, correr, saltar, rotar cámara)
- **Complejidad:** Media
- **Dependencias:** Ninguna (es la base de todo)

### 1.2 Sistema de Combate Melee
- **Descripción:** Hit detection cuerpo a cuerpo, animaciones de ataque, daño, stagger
- **Complejidad:** Alta
- **Dependencias:** Player Controller

### 1.3 Sistema de Movimiento Symbiote
- **Descripción:** Web shooters (engancharse a superficies, balanceo), bombas de impulso, traversal aéreo
- **Complejidad:** Muy Alta
- **Dependencias:** Player Controller, Sistema de Física

### 1.4 Sistema de Parkour
- **Descripción:** Wall-running, escalada, vault, slide, ledge grab
- **Complejidad:** Alta
- **Dependencias:** Player Controller, Movimiento Symbiote

### 1.5 Sistema de Cámara 3ª Persona
- **Descripción:** Cámara orbital, collision avoidance, lock-on a enemigos, transiciones suaves
- **Complejidad:** Media
- **Dependencias:** Player Controller

---

## Prioridad 2: GAMEPLAY (Lo que hace el juego divertido)

### 2.1 Sistema de Skills (4 tipos)
- **Descripción:** Implementar los 4 tipos de skills:
  - **Boost:** Buffs temporales (HP, daño, velocidad) con duración (5-6 min)
  - **Pause:** Debuffs a enemigos (slow, confusión)
  - **Attack:** Habilidades de daño directo
  - **Amplifier:** Potenciadores one-time (invisibilidad, amplificar attacks)
- **Complejidad:** Muy Alta
- **Dependencias:** Combate Melee, Player Controller

### 2.2 Sistema de Personajes (8 Arquetipos)
- **Descripción:** 2 familias × 4 tipos, cada uno con skills únicos
  - Familia 1: Fuerza/Agilidad bruta
  - Familia 2: Magia
- **Complejidad:** Alta
- **Dependencias:** Sistema de Skills

### 2.3 Sistema de Curación
- **Descripción:** Curación instantánea, 60 cargas en stacks de 15, cooldown de recarga 15 seg, interrumpible por daño
- **Complejidad:** Media
- **Dependencias:** Combate Melee, UI HUD

### 2.4 Sistema de Loot
- **Descripción:** Spawn de items/armas en el mapa, recolección del suelo, rareza de items
- **Complejidad:** Media
- **Dependencias:** Inventario

### 2.5 Sistema de Inventario
- **Descripción:** Gestión de items equipados, pociones, loot recogido
- **Complejidad:** Media
- **Dependencias:** UI

---

## Prioridad 3: NETWORKING (Lo que hace el juego online)

### 3.1 Arquitectura de Red (Netcode Hybrid)
- **Descripción:** Cliente predice, servidor valida. Relevancia por zonas para optimizar sincronización
- **Complejidad:** Muy Alta
- **Dependencias:** Player Controller, Combate Melee
- **Tecnología sugerida:** Unity Netcode for GameObjects / Photon Fusion / Mirror

### 3.2 Matchmaking BR
- **Descripción:** Matchmaking automático basado en ELO, asignación de mapa aleatorio, lobby de 20 jugadores
- **Complejidad:** Alta
- **Dependencias:** Arquitectura de Red, Sistema ELO

### 3.3 Servidores Persistentes de Clan
- **Descripción:** Sistema multiverse (subservidores Alpha, Beta, Gamma), zonas privadas/públicas, persistencia de estado
- **Complejidad:** Muy Alta
- **Dependencias:** Arquitectura de Red, Sistema de Clanes

### 3.4 Lag Compensation (Combate Melee)
- **Descripción:** Compensación de latencia específica para hits melee, server reconciliation, rollback
- **Complejidad:** Muy Alta
- **Dependencias:** Arquitectura de Red, Combate Melee

---

## Prioridad 4: SISTEMAS SOCIALES (Lo que retiene jugadores)

### 4.1 Sistema ELO
- **Descripción:** Rating dinámico estilo chess.com. Inicio en 500, +/- por kill/muerte. Ranking global en percentil
- **Complejidad:** Media
- **Dependencias:** Backend/Base de datos

### 4.2 Sistema de Clanes
- **Descripción:** Crear/unirse a clanes, roles (líder, miembros), gestión de subservidores, permisos
- **Complejidad:** Alta
- **Dependencias:** Backend, Networking

### 4.3 Marketplace / Tienda de Jugadores
- **Descripción:** Jugadores listan items para vender, búsqueda, compra con Gold. Exposición en web pública
- **Complejidad:** Alta
- **Dependencias:** Inventario, Economía, Backend, Web

### 4.4 Sistema de Economía (Gold)
- **Descripción:** Obtención de Gold (BR kills, loot), gasto (compra items de otros jugadores), balance anti-inflación
- **Complejidad:** Media
- **Dependencias:** Backend

### 4.5 Perfiles Públicos (Web)
- **Descripción:** Perfil con ELO, clan, redes sociales, fecha creación, ranking percentil. Visible en web
- **Complejidad:** Media
- **Dependencias:** Backend, Web

---

## Prioridad 5: BOSSES (Contenido endgame)

### 5.1 IA de Bosses
- **Descripción:** Patrones de ataque únicos por boss, ágiles, alto daño, requieren grupo para derrotar
- **Complejidad:** Muy Alta
- **Dependencias:** Combate Melee, AI System

### 5.2 Sistema de Spawn de Bosses
- **Descripción:** Aparición en horarios específicos, en mapas vacíos separados (no en BR activo), rotación entre 12 mapas
- **Complejidad:** Alta
- **Dependencias:** Servidores Persistentes, Sistema de Tiempo

### 5.3 Sistema de Drops Únicos
- **Descripción:** Loot table exclusiva de bosses, items equipables únicos (ej: anillo +50 HP), distribución al clan ganador
- **Complejidad:** Media
- **Dependencias:** Inventario, Sistema de Loot

### 5.4 Conquista de Bosses por Clanes
- **Descripción:** Clan que derrota boss → boss aparece en mapa del clan la próxima vez. Persistencia de conquistas
- **Complejidad:** Alta
- **Dependencias:** Sistema de Clanes, Spawn de Bosses, Servidores Persistentes

---

## Prioridad 6: EDITOR & HERRAMIENTAS

### 6.1 Editor de Mapas para Clanes
- **Descripción:** Herramienta para que líderes de clan diseñen su mapa (terreno, props, zonas). Los mapas son públicos
- **Complejidad:** Muy Alta
- **Dependencias:** Sistema de Clanes, Servidores Persistentes

---

## Prioridad 7: PRESENTACIÓN (Polish)

### 7.1 UI / HUD
- **Descripción:** HUD de combate (HP, skills, cooldowns, pociones), menús (lobby, inventario, tienda, perfil, clan)
- **Complejidad:** Alta
- **Dependencias:** Todos los sistemas de gameplay

### 7.2 Sistema de Audio
- **Descripción:** Música ambiental única por mapa, SFX de combate/movimiento/items/UI, audio 3D espacial
- **Complejidad:** Media
- **Dependencias:** Escenas de mapas

### 7.3 VFX y Shaders
- **Descripción:** Efectos del Symbiote, impactos melee, skills visuales, shaders realistas
- **Complejidad:** Alta
- **Dependencias:** Combate, Skills, Symbiote

### 7.4 Física Avanzada (Ragdoll)
- **Descripción:** Ragdoll en muerte, física de objetos, destrucción menor
- **Complejidad:** Media
- **Dependencias:** Combate Melee

---

## Prioridad 8: INFRAESTRUCTURA EXTERNA

### 8.1 Backend / API
- **Descripción:** Servidor backend para: cuentas, ELO, clanes, marketplace, perfiles, persistencia
- **Complejidad:** Muy Alta
- **Tecnología sugerida:** Node.js / Go + PostgreSQL + Redis

### 8.2 Página Web Pública
- **Descripción:** Web con marketplace, perfiles de jugadores, rankings globales
- **Complejidad:** Alta
- **Tecnología sugerida:** Next.js / React

### 8.3 Integración Steam
- **Descripción:** Steamworks SDK, logros, amigos, overlay, publicación en Steam Store
- **Complejidad:** Media
- **Dependencias:** Juego funcional

### 8.4 Sistema de Guardado / Persistencia
- **Descripción:** Guardar progreso del jugador (inventario, ELO, clan, items), sincronización con backend
- **Complejidad:** Alta
- **Dependencias:** Backend
