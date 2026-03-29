# ZoremGame (Indie) - Roadmap de Desarrollo

> Scope indie para Steam. Sin presión de tiempo.
> Solo developer + IA | Unity | PC (Steam)
> Visión completa guardada en: Documentation/Future/

---

## FASE 1: Fundación
> El personaje se mueve y pelea.

### Tareas
| Tarea | Sistema |
|-------|---------|
| Player Controller (caminar, correr, saltar) | Core |
| Cámara 3ª persona con collision avoidance y lock-on | Core |
| Combate melee (ataques, hit detection, daño) | Combat |
| Web shooters (enganche + balanceo) | Symbiote |
| Bombas de impulso | Symbiote |
| Parkour básico (wall-run, vault, slide) | Movement |

### Entregable
> Un personaje jugable en un mapa de prueba que pelea y se mueve como Spider-Man Symbiote.

---

## FASE 2: Identidad
> Los 4 personajes son únicos.

### Tareas
| Tarea | Sistema |
|-------|---------|
| Definir 4 arquetipos de personaje | Characters |
| Skills Boost (buffs temporales 5-6 min) | Skills |
| Skills Pause (debuffs: slow, confusión) | Skills |
| Skills Attack (habilidades de daño) | Skills |
| Skills Amplifier (potenciadores one-time) | Skills |
| Sistema de curación (60 cargas, stacks 15, cooldown 15s) | Healing |
| Inventario básico (equipar items, pociones) | Inventory |
| Loot del suelo (armas, items) | Loot |
| HUD (HP, skills, cooldowns, pociones) | UI |

### Entregable
> 4 personajes jugables con skills únicos, curación, loot e inventario en un mapa de prueba.

---

## FASE 3: Multijugador
> Jugar con amigos. Stack: FishNet + SteamNetworkingSockets + Steamworks.NET ($0 infra)

### Paso 1: Conexión básica
| Tarea | Sistema |
|-------|---------|
| Instalar FishNet (free) desde Asset Store / GitHub | Networking |
| Instalar Steamworks.NET | Steam |
| Configurar SteamNetworkingSockets Transport en FishNet | Networking |
| Conectar 2 jugadores via Steam (listen server) | Networking |

### Paso 2: Movimiento en red
| Tarea | Sistema |
|-------|---------|
| Agregar PredictedObject al PlayerController (prediction built-in) | Networking |
| Interpolación de jugadores remotos (FishNet lo maneja) | Networking |
| Verificar movimiento Symbiote sincronizado | Networking |

### Paso 3: Salas y combate en red
| Tarea | Sistema |
|-------|---------|
| Steam Lobby API: crear sala FriendsOnly/Private | Rooms |
| Invitaciones via Steam overlay | Rooms |
| Combate melee server-authoritative (host valida hits) | Networking |
| Lag compensation: historial de hitboxes + rewind por RTT | Networking |

### Paso 4: Sincronización completa
| Tarea | Sistema |
|-------|---------|
| ObserverManager: relevancia por distancia (solo sync cercanos) | Networking |
| Sync loot, inventario, curación con SyncVars + RPCs | Networking |
| Skills server-authoritative (prevenir trampas) | Networking |
| Escalar test a 4-20 jugadores | Networking |

### Entregable
> Jugadores crean sala via Steam, invitan amigos, pelean con combate melee fluido (0ms input delay local, lag compensado) y recogen loot sincronizado.

---

## FASE 4: Mundo
> Los 4 mapas y sus bosses.

### Tareas
| Tarea | Sistema |
|-------|---------|
| Diseñar y construir Mapa 01 | Maps |
| Diseñar y construir Mapa 02 | Maps |
| Diseñar y construir Mapa 03 | Maps |
| Diseñar y construir Mapa 04 | Maps |
| Boss 01 (patrones únicos, drops exclusivos) | Bosses |
| Boss 02 (patrones únicos, drops exclusivos) | Bosses |
| Boss 03 (patrones únicos, drops exclusivos) | Bosses |
| Boss 04 (patrones únicos, drops exclusivos) | Bosses |
| Sistema de spawn por horarios fijos | Bosses |
| Música ambiental por mapa (4 tracks) | Audio |
| SFX de combate, movimiento, items, UI | Audio |
| Menú de selección de mapa | UI |

### Entregable
> 4 mapas completos con música, 4 bosses con horarios fijos y drops únicos.

---

## FASE 5: Polish y Steam
> Pulir y publicar.

### Tareas
| Tarea | Sistema |
|-------|---------|
| Ragdoll al morir | Physics |
| VFX del Symbiote (web, explosiones, skills) | VFX |
| Shaders realistas | Shaders |
| Main Menu y pantallas de UI finales | UI |
| Integración Steamworks (logros, overlay) | Steam |
| Optimización (60 FPS, LOD, culling) | Performance |
| Testing y bug fixing | QA |
| Publicación en Steam Store | Steam |

### Entregable
> Juego pulido y publicado en Steam.

---

## Resumen Visual

```
FASE 1          FASE 2          FASE 3                    FASE 4          FASE 5
Fundación  ──►  Identidad  ──►  Multijugador         ──►  Mundo  ──►  Steam

Movimiento      4 Personajes    FishNet + Steam           4 Mapas         VFX
Combate         4×Skills        PredictedObject           4 Bosses        Ragdoll
Symbiote        Curación        Steam Lobbies             Audio           Steamworks
Parkour         Inventario      Lag Comp (hitbox rewind)   Boss Spawns     Optimización
Cámara          HUD             ObserverManager (zonas)   Drops           Publicación
```

## Stack Tecnológico

| Componente | Tecnología | Costo |
|-----------|-----------|-------|
| Motor | Unity | $0 (personal) |
| Networking | FishNet (Fish-Networking) | $0 |
| Transporte | SteamNetworkingSockets (Valve SDR) | $0 |
| Steam API | Steamworks.NET | $0 |
| Salas | Steam Lobby API | $0 |
| **Total infraestructura** | | **$0** |
