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

### 3.1 Netcode Hybrid
- Cliente predice, servidor valida
- Relevancia por zonas (solo sincronizar jugadores cercanos)
- **Complejidad:** Muy Alta

### 3.2 Sistema de Salas (Rooms)
- Jugadores crean salas privadas e invitan amigos
- Sin matchmaking automático
- **Complejidad:** Media

### 3.3 Lag Compensation Melee
- Compensación de latencia para hits cuerpo a cuerpo
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
