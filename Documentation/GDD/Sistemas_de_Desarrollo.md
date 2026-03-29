# ZoremGame (Indie) - Sistemas de Desarrollo

> Scope reducido para publicación indie en Steam.
> Visión completa guardada en: Documentation/Future/

---

## Prioridad 1: CORE (Sin esto no hay juego)

### 1.1 Player Controller & Movimiento
- Control de personaje en 3ª persona (caminar, correr, saltar, rotar cámara)
- **Complejidad:** Media

### 1.2 Sistema de Combate Melee
- Hit detection cuerpo a cuerpo, ataques, daño, stagger
- **Complejidad:** Alta

### 1.3 Movimiento Symbiote
- Web shooters (enganche + balanceo), bombas de impulso (limitadas pero duraderas)
- **Complejidad:** Muy Alta

### 1.4 Parkour
- Wall-running, escalada, vault, slide
- **Complejidad:** Alta

### 1.5 Cámara 3ª Persona
- Cámara orbital, collision avoidance, lock-on
- **Complejidad:** Media

---

## Prioridad 2: GAMEPLAY

### 2.1 Sistema de Skills (4 tipos)
- **Boost:** Buffs temporales (HP, daño, velocidad) por 5-6 min
- **Pause:** Debuffs a enemigos (slow, confusión)
- **Attack:** Habilidades de daño directo
- **Amplifier:** Potenciadores one-time (invisibilidad, amplificar attacks)
- **Complejidad:** Alta

### 2.2 Sistema de 4 Personajes
- 4 arquetipos con skills únicos
- Todos disponibles desde inicio, cambio entre partidas
- **Complejidad:** Media

### 2.3 Sistema de Curación
- Curación rápida, 60 cargas en stacks de 15
- Cooldown de recarga: 15 seg, interrumpible
- **Complejidad:** Media

### 2.4 Loot e Inventario
- Items/armas en el suelo, recolección, equipamiento
- **Complejidad:** Media

### 2.5 HUD
- HP, skills, cooldowns, pociones, indicadores de daño
- **Complejidad:** Media

---

## Prioridad 3: MULTIJUGADOR

### Stack Tecnológico Definido
| Componente | Tecnología | Costo |
|-----------|-----------|-------|
| **Networking Framework** | FishNet (Fish-Networking) | $0 (free, open source) |
| **Transporte** | SteamNetworkingSockets Transport | $0 (Valve SDR relay) |
| **Steam Integration** | Steamworks.NET | $0 (open source) |
| **Salas/Lobbies** | Steam Lobby API (Steamworks.NET) | $0 |
| **Fallback** | Mirror + Steam Transport (si FishNet falla) | $0 |

**Costo total de infraestructura: $0**

### ¿Por qué FishNet?
- **PredictedObject** built-in: client-side prediction + server reconciliation automático
- Soporte nativo para SteamNetworkingSockets (Valve relay, $0)
- ObserverManager con condiciones de distancia (relevancia por zonas)
- API moderna, ideal para solo dev
- Alternativa Pro ($150 one-time) para optimizaciones de bandwidth

### ¿Por qué NO otras opciones?
- **Mirror:** Viable pero sin prediction built-in (semanas de trabajo extra)
- **Unity NGO:** Empuja hacia Unity Relay (costo), prediction manual difícil
- **Photon Fusion:** Técnicamente superior pero costo recurrente ($95+/mes por CCU)

### Arquitectura de Red
```
Steam Lobby (Steamworks.NET)
    │
    ▼
Jugador crea sala privada → Amigos ven invitación via Steam overlay
    │
    ▼
Host inicia FishNet server (listen server mode)
    │
    ▼
Clientes conectan via SteamNetworkingSockets (SDR relay de Valve)
    │
    ▼
Sincronización:
  - Movimiento jugador:  PredictedObject (prediction + reconciliation built-in)
  - Jugadores remotos:   Interpolación (FishNet lo maneja)
  - Combate melee:       Server-authoritative + lag compensation (hitbox rewind)
  - Skills/Items:        Server-authoritative con SyncVars
  - Boss AI:             Corre solo en host, estado sincronizado a clientes
  - Loot:                Server-authoritative (spawn + pickup)
  - Curación:            Server-validated (prevenir trampas)
```

### 3.1 Integración FishNet + Steam
- Configurar FishNet con SteamNetworkingSockets transport
- Conectar dos jugadores a través de Steam lobbies
- Listen server mode (host = jugador + servidor)
- **Complejidad:** Alta

### 3.2 Client-Side Prediction (Movimiento)
- PredictedObject de FishNet en PlayerController
- El jugador se mueve al instante, servidor valida después
- Reconciliación automática si hay discrepancia
- **Complejidad:** Media (FishNet lo facilita)

### 3.3 Sistema de Salas (Steam Lobbies)
- CreateLobby con tipo FriendsOnly o Private
- Invitación via Steam overlay
- Metadata del lobby (mapa elegido, max jugadores, configuración)
- **Complejidad:** Media

### 3.4 Lag Compensation Melee
- Historial de hitboxes por frame (buffer de N frames)
- Al validar hit: rebobinar hitbox del objetivo según RTT del atacante
- TimeManager de FishNet para timing preciso
- **Complejidad:** Alta

### 3.5 Relevancia por Zonas
- ObserverManager de FishNet con condiciones de distancia
- Solo sincronizar jugadores y entidades cercanas
- Reduce bandwidth en mapas grandes con 4-20 jugadores
- **Complejidad:** Media

### 3.6 Sincronización de Sistemas
- Loot, inventario, curación: SyncVars + RPCs
- Boss AI: ejecuta en host, replica estado a clientes
- Skills: server-authoritative (prevenir trampas)
- **Complejidad:** Alta

---

## Prioridad 4: MAPAS Y BOSSES

### 4.1 Cuatro Mapas
- 4 mapas diseñados manualmente, cada uno con ambiente y música únicos
- **Complejidad:** Alta

### 4.2 Cuatro Bosses (1 por mapa)
- Cada boss con patrones de ataque únicos
- Ágiles, alto daño, requieren grupo
- Aparecen en horarios fijos definidos por el desarrollador
- **Complejidad:** Muy Alta

### 4.3 Drops de Bosses
- Items equipables únicos por boss
- **Complejidad:** Media

---

## Prioridad 5: PRESENTACIÓN

### 5.1 Audio
- Música ambiental única por mapa (4 tracks)
- SFX de combate, movimiento, items, UI
- **Complejidad:** Media

### 5.2 VFX y Shaders
- Efectos del Symbiote, impactos melee, skills visuales
- **Complejidad:** Alta

### 5.3 Ragdoll
- Física ragdoll al morir
- **Complejidad:** Media

---

## Prioridad 6: PUBLICACIÓN

### 6.1 Integración Steam
- Steamworks SDK, logros, overlay
- **Complejidad:** Media

### 6.2 Optimización
- Performance (60 FPS), LOD, culling
- **Complejidad:** Alta

### 6.3 QA
- Testing y bug fixing
- **Complejidad:** Alta

---

## Resumen: 17 sistemas (vs 28 de la versión completa)

| Versión | Personajes | Mapas | Bosses | Networking | Clanes | BR | Web |
|---------|-----------|-------|--------|------------|--------|-----|-----|
| **Indie** | 4 | 4 | 4 | Salas privadas | No | No | No |
| Future | 8 | 12+ editor | 12+ | Matchmaking + Multiverse | Sí | Sí | Sí |
