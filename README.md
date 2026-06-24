# Fortune Wheel Demo

A zone-based Wheel of Fortune game built as a technical case study for Vertigo Games. Players spin a wheel each zone to collect rewards hitting the **bomb** clears everything unless they spend currency to revive.

**Stack:** Unity 2021.3 LTS · C# · DOTween · TextMeshPro · ScriptableObjects · Android

---

## Mechanics

| Feature | Detail |
|---|---|
| Wheel spin | Weighted random — appearance and win probability are independent |
| Zone progression | 50 zones; reward quality scales with zone group |
| Safe Zone | Every 5th zone — silver wheel, no bomb, player can leave |
| Super Zone | Every 30th zone — golden wheel, best rewards, player can leave |
| Bomb | Clears all collected rewards; escalating win chance per zone |
| Revive | Spend gold to restore pre-spin snapshot; cost multiplies each use |

---

## Design Patterns

### Strategy — `ISpinStrategy`
Each zone type (Bronze / Silver / Golden) is a separate class implementing `ISpinStrategy`. `GameManager.UpdateStrategy()` swaps the active strategy after every zone advance. `HudUI` reads `CanPlayerLeave` without knowing which strategy is active adding a new zone type requires zero changes to existing classes.

```
BronzeSpinStrategy  →  HasBomb: true,  CanPlayerLeave: false
SilverSpinStrategy  →  HasBomb: false, CanPlayerLeave: true
GoldenSpinStrategy  →  HasBomb: false, CanPlayerLeave: true
```

### State Machine — `GameStateController`
Six states with validated transitions. Invalid calls (e.g. double-clicking Spin) are silently rejected. UI subscribes to `OnStateChanged(from, to)` the edge, not just the state, so `BombPopupUI` can show on entry and hide on exit with a single handler.

```
Idle → Spinning → ShowResult → ZoneTransition → Idle
                       └──→ BombHit → Idle
```

### Observer — Two layers
- **C# Action events** (`OnSpinResultEvaluated`, `OnStateChanged`) — typed, compile time safe, carry data. Used for core game flow.
- **ScriptableObject Event Bus** (`GameEventSO`) zero code coupling, Inspector wired. Used for sound triggers and scene choreography.

### Command — `ReviveCommand`
Before each spin `GameManager` snapshots inventory + zone. On revive, `ReviveCommand.Execute()` restores both atomically the spin never happened. Adding a second undo level means storing a second command, no other changes.

### Factory — `SliceFactory`
`WheelController` calls `CreateSlice(data, angle, radius)` and receives a ready `SliceView`. Polar to Cartesian positioning math, rotation and initialization live only in the factory.

### Object Pool — `SoundManager`
20 `AudioSource` components created once at startup, used round robin. Wheel tick fires every ~44° of rotation (~30+ events per spin) — per-sound allocation would cause GC spikes.

### Singleton
`GameManager`, `ZoneManager`, `CurrencyManager`, `SoundManager` — single-scene, no DI container overhead. Each exposes a clean public API; UI layer never touches internals directly.

---

## ScriptableObject Architecture

```
ZoneConfigSO                    ← global rules (safe: 5, super: 30, bomb progression)
├── ZoneGroupConfigSO[]         ← zone range → WheelConfigSO mapping
├── SilverWheelConfig
└── GoldenWheelConfig

WheelConfigSO
├── SpinChanceEntry[]
│   ├── Slice: SliceDataSO
│   ├── AppearWeight            ← probability of appearing on the wheel
│   └── WinWeight               ← probability of being the outcome
└── SlicesPerSpin: 8

SliceDataSO                     ← single reward (icon, type, amount, isBomb)
```

**Two independent probability layers:** `AppearWeight` controls visual composition; `WinWeight` controls the outcome. A bomb can appear on the wheel often but be hit rarely or vice versa.

**Bomb escalation:** `ZoneConfigSO` holds `BombWinChanceMin` (5%) and `BombWinChanceMax` (35%). `ZoneManager.GetBombWinChance()` linearly interpolates across 50 zones and passes it to `SpinResultEvaluator` as a per-zone override no per zone assets needed.

---

## Manager Responsibilities

| Manager | Responsibility |
|---|---|
| `GameManager` | Central orchestrator — spin lifecycle, revive, reset |
| `GameStateController` | State machine — validates and broadcasts transitions |
| `ZoneManager` | Zone tracking, safe/super detection, bomb chance calculation |
| `CurrencyManager` | Revive cost tracking (exponential multiplier) |
| `RewardInventory` | Runtime reward dictionary, snapshot for revive |
| `SoundManager` | Audio pool, key-based playback |
| `WheelController` | Builds/clears wheel, manages Bronze/Silver/Golden roots |
| `WheelSpinHandler` | DOTween spin animation — pullback, delta-based winner landing |
| `SpinResultEvaluator` | Weighted random selection with per-zone bomb override |

---

## Project Structure

```
Assets/_Game/
├── Scripts/
│   ├── Core/          GameManager, ZoneManager, CurrencyManager, SoundManager ...
│   ├── Wheel/         WheelController, WheelSpinHandler, SliceFactory, SpinResultEvaluator
│   ├── Strategies/    BronzeSpinStrategy, SilverSpinStrategy, GoldenSpinStrategy
│   ├── ScriptableObjects/  SliceDataSO, WheelConfigSO, ZoneConfigSO, ZoneGroupConfigSO
│   ├── Events/        GameEventSO, GameEventListener
│   └── UI/            HudUI, RewardRevealUI, InventoryPanelUI, ZoneBarUI, BombPopupUI ...
└── ScriptableObjects/
    ├── Slices/        SliceData assets per reward type
    ├── Wheels/        WheelConfig assets (Bronze_Early/Mid/Late, Silver, Golden)
    └── Zones/         ZoneGroupConfig assets, ZoneConfig
```

---

---

## Design Note

Some patterns here (Command, explicit Strategy classes, two layer observer) are more granular than I'd reach for in a production codebase of this scale. In a real project the scope, team size and maintainability requirements would drive those decisions differently.

For this case study I wanted each pattern to be clearly identifiable and easy to discuss, so I applied them deliberately and explicitly rather than only where complexity truly demanded it.

---

*Vertigo Games Technical Case Study — Kutluhan Ünsal*