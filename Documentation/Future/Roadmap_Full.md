# ZoremGame - Roadmap de Desarrollo

> Sin presión de tiempo. Objetivo: Prototipo jugable para Steam.
> Desarrollador: Solo + IA (Claude, Codex, Gemini)
> Motor: Unity | Plataforma: PC (Steam)

---

## FASE 1: Fundación (El personaje se mueve y pelea)

### Objetivos
- [ ] Un personaje en 3ª persona que se mueve fluidamente
- [ ] Combate melee funcional
- [ ] Movimiento Symbiote básico
- [ ] Cámara 3ª persona con lock-on

### Tareas
| Tarea | Sistema |
|-------|---------|
| Player Controller (caminar, correr, saltar, rotar) | Core |
| Cámara orbital 3ª persona con collision avoidance | Core |
| Sistema de combate melee (ataques básicos, hit detection, daño) | Core |
| Animaciones placeholder de combate | Core |
| Web shooters básicos (enganche + balanceo) | Symbiote |
| Bombas de impulso | Symbiote |
| Parkour básico (wall-run, vault, slide) | Movimiento |

### Entregable
> Un personaje jugable en un mapa de prueba que puede moverse como Spider-Man Symbiote y pelear cuerpo a cuerpo.

---

## FASE 2: Identidad (Los personajes son únicos)

### Objetivos
- [ ] 8 arquetipos de personaje diferenciados
- [ ] Sistema de skills completo (4 tipos)
- [ ] Sistema de curación
- [ ] Loot básico e inventario

### Tareas
| Tarea | Sistema |
|-------|---------|
| ScriptableObjects para definir cada arquetipo | Personajes |
| Skills tipo Boost (buffs temporales 5-6 min) | Skills |
| Skills tipo Pause (debuffs: slow, confusión) | Skills |
| Skills tipo Attack (habilidades de daño) | Skills |
| Skills tipo Amplifier (potenciadores one-time) | Skills |
| Sistema de curación (60 cargas, stacks de 15, cooldown 15s, interrumpible) | Healing |
| Inventario básico (equipar items, gestionar pociones) | Inventario |
| Loot spawns en el mapa (armas, items) | Loot |
| HUD básico (HP, skills, cooldowns, pociones) | UI |

### Entregable
> 8 personajes jugables con habilidades únicas, sistema de curación, y loot básico en un mapa de prueba.

---

## FASE 3: Multijugador (Jugar con otros)

### Objetivos
- [ ] Netcode hybrid funcional
- [ ] 20 jugadores en un mapa grande
- [ ] Lag compensation para melee
- [ ] Matchmaking básico

### Tareas
| Tarea | Sistema |
|-------|---------|
| Elegir e integrar solución de red (Netcode/Photon/Mirror) | Networking |
| Sincronización de movimiento y combate (netcode hybrid) | Networking |
| Sistema de relevancia por zonas (solo sincronizar jugadores cercanos) | Networking |
| Lag compensation para hits melee | Networking |
| Lobby de 20 jugadores | Matchmaking |
| Matchmaking básico (conectar jugadores) | Matchmaking |
| Spawn/despawn de jugadores | Networking |
| Sincronización de loot, inventario, y curación | Networking |

### Entregable
> 20 jugadores pueden entrar a un mapa, pelear, recoger loot y curarse con latencia aceptable.

---

## FASE 4: Battle Royale (El modo principal)

### Objetivos
- [ ] Loop de BR completo (entrar, pelear, ganar/perder)
- [ ] Sistema ELO funcional
- [ ] Gold rewards
- [ ] Primer mapa BR diseñado

### Tareas
| Tarea | Sistema |
|-------|---------|
| Diseñar y construir primer mapa BR (grande, con zonas) | Mapas |
| Game loop de BR (inicio, zona, fin, ganador) | Core |
| Sistema ELO (base 500, +/- por kill/muerte) | ELO |
| Ranking global en percentil | ELO |
| Sistema de Gold (rewards por kills/victorias) | Economía |
| Matchmaking basado en ELO | Matchmaking |
| Asignación de mapa aleatorio | Matchmaking |
| Pantalla de resultados post-partida | UI |
| Música ambiental para el primer mapa | Audio |
| SFX de combate, movimiento, items, UI | Audio |

### Entregable
> Un Battle Royale de 20 jugadores funcional con ELO, Gold, y un mapa completo.

---

## FASE 5: Clanes y Persistencia (La comunidad)

### Objetivos
- [ ] Sistema de clanes (crear, unirse, roles)
- [ ] Servidores persistentes con multiverse
- [ ] Backend básico

### Tareas
| Tarea | Sistema |
|-------|---------|
| Backend: API para cuentas, ELO, clanes, inventario | Backend |
| Base de datos (PostgreSQL + Redis) | Backend |
| Sistema de clanes (crear, unirse, roles, líder) | Clanes |
| Subservidores multiverse (Alpha, Beta, Gamma) | Servidores |
| Navegación entre mapas (tu clan, otros clanes, públicos, 12 base) | Servidores |
| Zonas privadas por subservidor | Servidores |
| Persistencia de inventario y progreso | Persistencia |
| Sistema de guardado sincronizado con backend | Persistencia |

### Entregable
> Jugadores pueden crear clanes, entrar a servidores persistentes, y su progreso se guarda.

---

## FASE 6: Bosses y Conquista (El endgame)

### Objetivos
- [ ] Bosses con IA única
- [ ] Sistema de spawn por horarios
- [ ] Drops únicos
- [ ] Conquista de bosses por clanes

### Tareas
| Tarea | Sistema |
|-------|---------|
| IA del primer boss (patrones únicos, ágil, alto daño) | Bosses |
| Mapas de boss (vacíos, separados de BR) | Mapas |
| Sistema de spawn por horarios (rotación entre mapas) | Bosses |
| Drops únicos del boss (items equipables exclusivos) | Loot |
| Mecánica de conquista (clan derrota → boss en su mapa) | Clanes |
| Persistencia de conquistas | Servidores |

### Entregable
> Bosses aparecen en horarios, clanes luchan por conquistarlos, y obtienen drops exclusivos.

---

## FASE 7: Economía y Social (Retención)

### Objetivos
- [ ] Marketplace entre jugadores
- [ ] Perfiles públicos
- [ ] Página web

### Tareas
| Tarea | Sistema |
|-------|---------|
| Marketplace in-game (listar items, buscar, comprar con Gold) | Economía |
| Página web pública (marketplace, perfiles, rankings) | Web |
| Perfiles: ELO, clan, redes sociales, fecha creación, percentil | Social |
| Balance económico anti-inflación | Economía |

### Entregable
> Jugadores pueden vender items, ver perfiles y rankings en una web pública.

---

## FASE 8: Editor de Mapas y Contenido (Creatividad)

### Objetivos
- [ ] Editor de mapas para líderes de clan
- [ ] 12 mapas BR completos
- [ ] Más bosses

### Tareas
| Tarea | Sistema |
|-------|---------|
| Editor de mapas (terreno, props, zonas) para líderes de clan | MapEditor |
| Diseñar y construir los 12 mapas BR | Mapas |
| Mapas de boss correspondientes (12 vacíos) | Mapas |
| Más bosses con patrones únicos | Bosses |
| Música ambiental única por cada mapa | Audio |

### Entregable
> 12 mapas completos, editor funcional, múltiples bosses.

---

## FASE 9: Polish y Steam (Publicación)

### Objetivos
- [ ] Integración Steam (Steamworks SDK)
- [ ] Ragdoll y física avanzada
- [ ] VFX y shaders del Symbiote
- [ ] Optimización de performance
- [ ] QA y bug fixing

### Tareas
| Tarea | Sistema |
|-------|---------|
| Integración Steamworks (logros, overlay, amigos) | Steam |
| Ragdoll en muerte | Física |
| VFX del Symbiote (web, explosiones, skills) | VFX |
| Shaders realistas (Symbiote, environment) | Shaders |
| Optimización de red (bandwidth, tick rate) | Networking |
| Optimización gráfica (LOD, culling, batching) | Performance |
| Testing masivo y bug fixing | QA |
| Publicación en Steam Store | Steam |

### Entregable
> Juego pulido, optimizado, y publicado en Steam.
