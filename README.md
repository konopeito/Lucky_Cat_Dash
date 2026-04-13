# Lucky Cat Dash
**ラッキーキャット・ダッシュ** (Rakkī Kyatto Dasshu)

A fast-paced 2D platformer centered on luck-based movement and multiplayer racing. Players control lucky cats traversing Japanese-inspired stages, collecting charms to fill a Luck Meter and unlocking special abilities.

---

## Game Concept

Players collect charms to fill a **Luck Meter**. As luck increases, they unlock or empower abilities:

| Luck Threshold | Ability Unlocked |
|---|---|
| 25 | Double Jump |
| 50 | Dash |
| 75 | Slow Fall |

In **Co-op mode**, when all players simultaneously maintain high luck, a **Team Luck Bonus** triggers — temporary platforms spawn and hazards are briefly disabled.

---

## Game Modes

| Mode | Description |
|---|---|
| Single Player | Race to the shrine, maximising charms and best time |
| Multiplayer Co-op | Work together; shared team luck bonus rewards coordination |
| Multiplayer Competitive | Race/score — first to shrine or most charms wins |

---

## Controls

| Input | Action |
|---|---|
| A / D | Move left / right |
| Space | Jump |
| Space (air, luck ≥ 25) | Double jump |
| Left Shift | Dash (requires luck ≥ 50) |
| R | Respawn at last checkpoint |
| Esc | Pause menu |

---

## Scene Structure

| Scene Name | Purpose |
|---|---|
| `MainMenu` | Mode selection, volume settings |
| `Lobby` | Multiplayer lobby (host/join) |
| `Level1` | Shrine Rooftops |
| `Level2` | Bamboo Forest |
| `Results` | Match results, best time, total charms |

---

## Project Architecture

```
Assets/Scripts/
├── Core/
│   ├── GameManager.cs          — Singleton: match state, scene transitions, charm events
│   └── SaveSystem.cs           — JSON persistence (charms, wins/losses, best time, skins, volume)
├── Player/
│   ├── PlayerController2D.cs   — Movement, jump (coyote + buffer), respawn
│   ├── LuckSystem.cs           — Luck meter, threshold checks, spend/add API
│   ├── PlayerAbilities.cs      — Dash and slow-fall ability execution
│   └── SkinSelector.cs         — Cat skin management (unlock + apply)
├── Gameplay/
│   ├── CharmCollectible.cs     — Charm pickup; adds luck + notifies GameManager (pooled)
│   ├── ObjectPool.cs           — Generic GameObject pool for charms/platforms
│   ├── Checkpoint.cs           — Sets player respawn point on trigger
│   ├── Hazard.cs               — Sends player to respawn point on trigger
│   ├── GoalTrigger.cs          — Ends match and loads Results scene on trigger
│   └── TeamLuckBonusController.cs — Co-op team bonus: monitors all players' luck
├── UI/
│   ├── UIHudController.cs      — Luck slider, charm counter, match timer
│   ├── PauseMenuController.cs  — Pause/resume, volume slider, quit to menu
│   ├── MainMenuController.cs   — Mode selection, volume, quit
│   ├── LobbyController.cs      — Lobby start/back buttons, mode display
│   └── ResultsScreenController.cs — Match time, charms, best time, play again
├── Audio/
│   └── AudioManager.cs         — Singleton: music + SFX playback, volume control
└── Network/
    └── NetworkGameManager.cs   — Multiplayer session state, host authority, team bonus bridge
```

### Design Patterns

- **Singleton** — `GameManager`, `AudioManager`, `NetworkGameManager` persist across scenes via `DontDestroyOnLoad`.
- **Object Pool** — `ObjectPool` recycles charm collectibles and temporary platforms to avoid allocation spikes.
- **Events (Delegates)** — `LuckSystem.OnLuckChanged`, `GameManager.OnCharmCountChanged`, `GameManager.OnMatchStarted/Ended` decouple systems.

---

## Data Persistence

Saved to `Application.persistentDataPath/save.json`:

| Field | Description |
|---|---|
| `totalCharms` | Cumulative charms across all sessions |
| `wins` / `losses` | Match outcomes |
| `bestTime` | Fastest match completion time |
| `masterVolume` | Audio volume (0–1) |
| `unlockedSkins` | Comma-separated skin IDs |

---

## Setup (Unity)

1. Open the project in **Unity 2022.3 LTS** or later (2D URP template recommended).
2. Import **TextMeshPro** (Window → Package Manager → TextMeshPro).
3. Add scenes to **Build Settings** in the order: `MainMenu`, `Lobby`, `Level1`, `Level2`, `Results`.
4. Assign the `GameManager` prefab (with `DontDestroyOnLoad`) to the `MainMenu` scene.
5. Wire up `UIHudController`, `PauseMenuController`, and `AudioManager` references in each scene's Inspector.
6. Tag ground layers appropriately and assign `groundMask` on each `PlayerController2D`.