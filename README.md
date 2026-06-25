# Fortune Wheel Demo

A zone based Wheel of Fortune mobile game. Players spin a wheel each zone to collect rewards hitting the **bomb** clears everything unless they spend currency to revive.

**Stack:** Unity 2021.3 LTS · C# · DOTween · TextMeshPro · ScriptableObjects · Android
---

## Mechanics

| Feature | Detail |
|---|---|
| Wheel spin | Weighted random appearance and win probability are independent |
| Zone progression | 50 zones; reward quality scales with zone group |
| Safe Zone | Every 5th zone is silver wheel, no bomb, player can leave |
| Super Zone | Every 30th zone golden wheel, best rewards, player can leave |
| Revive | Spend gold to restore pre-spin snapshot; cost multiplies each use |
| Bomb | Clears all collected rewards; escalating win chance per zone |

---

## Design Patterns

**Strategy — `ISpinStrategy`**  
Each zone type is a separate class. `GameManager.UpdateStrategy()` swaps the active strategy; `HudUI` reads `CanPlayerLeave` without knowing which is active.

```
BronzeSpinStrategy  →  HasBomb: true,  CanPlayerLeave: false
SilverSpinStrategy  →  HasBomb: false, CanPlayerLeave: true
GoldenSpinStrategy  →  HasBomb: false, CanPlayerLeave: true
```

**State Machine — `GameStateController`**  
Six states with validated transitions. Invalid calls are silently rejected. UI subscribes to `OnStateChanged(from, to)` so it can react to the exact edge, not just the new state.

```
Idle → Spinning → ShowResult → ZoneTransition → Idle
                      └──→ BombHit → Idle
```

**Observer — Two layers**
- **C# Action events** typed, compile-time safe, carry data. Used for core game flow.
- **ScriptableObject Event Bus** (`GameEventSO`) — zero code coupling, Inspector wired. Used for audio and scene choreography.

**Command — `ReviveCommand`**  
Before each spin, `GameManager` snapshots inventory and zone. `ReviveCommand.Execute()` restores both atomically on revive.

**Factory — `SliceFactory`**  
Encapsulates polar to Cartesian positioning, rotation, and `SliceView` initialization. `WheelController` calls `CreateSlice()` and receives a ready view.

**Object Pool — `SoundManager`**  
20 `AudioSource` components, round-robin. Wheel tick fires every ~44° (~30+ events per spin) prevents GC spikes from per sound allocation.

**Singleton** — `GameManager`, `ZoneManager`, `CurrencyManager`, `SoundManager`. Single scene, clean public API, no UI references in logic.

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

`AppearWeight` controls visual composition; `WinWeight` controls the outcome two independent probability layers. Bomb escalation is a linear interpolation across zones (5% → 35%), computed at runtime with no per-zone assets.

---

## Manager Responsibilities

| Manager | Responsibility |
|---|---|
| `GameManager` | Central orchestrator spin lifecycle, revive, reset |
| `GameStateController` | State machine validates and broadcasts transitions |
| `ZoneManager` | Zone tracking, safe/super detection, bomb chance calculation |
| `CurrencyManager` | Revive cost tracking (exponential multiplier) |
| `RewardInventory` | Runtime reward dictionary, snapshot for revive |
| `SoundManager` | Audio pool, key-based playback |
| `WheelController` | Builds/clears wheel, manages Bronze/Silver/Golden roots |
| `WheelSpinHandler` | DOTween spin animation pullback, delta-based winner landing |
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

## Design Note

The patterns here are more explicit than a production codebase of this scale would demand. The goal was for each pattern to be clearly identifiable and discussable scope, team size, and maintainability requirements would drive those decisions differently in a real project.

---

*Kutluhan Ünsal*
