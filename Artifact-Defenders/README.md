# ⚔️ Ngo Quyen: Battle of Bach Dang

A 2D top-down action strategy game based on the legendary Vietnamese historical battle. Command Ngo Quyen through the iconic Bach Dang River strategy to defeat the Southern Han forces.

> *"In 938 CE, Ngo Quyen's tactical genius changed the course of Vietnamese history..."*

---

## 📸 Screenshots

![Main Gameplay](https://via.placeholder.com/800x600?text=Main+Gameplay+Screenshot)
![Boss+Battle](https://via.placeholder.com/800x600?text=Boss+Battle+Screenshot)
![Strategic+View](https://via.placeholder.com/800x600?text=Strategic+River+View)
![Pixel+Art+Battles](https://via.placeholder.com/800x600?text=Pixel+Art+Combat)

---

## 🎮 Gameplay Overview

**Ngo Quyen: Battle of Bach Dang** is an immersive 2D action-strategy game where you take control of the legendary General Ngo Quyen during his pivotal victory against the Southern Han Dynasty. 

The game combines:
- **Real-time combat** with precision-based tactics
- **Strategic movement** across the river environment with wooden stakes
- **Boss encounters** with challenging enemy generals
- **Pixel art aesthetics** inspired by ancient Vietnamese culture

Master the environment by strategically positioning yourself around wooden stakes, manage your resources wisely, defeat waves of enemy soldiers, and ultimately face the enemy general boss in an epic final confrontation.

---

## 🕹️ Controls

| Action | Input | Description |
|--------|-------|-------------|
| **Move Up** | W | Move upward on the map |
| **Move Down** | S | Move downward on the map |
| **Move Left** | A | Move leftward on the map |
| **Move Right** | D | Move rightward on the map |
| **Basic Attack** | Left Mouse Click | Quick melee/projectile attack |
| **Throwing Blades** | Q | 360° spinning blade attack (costs energy) |
| **Dash/Evade** | Space | Quick dodge to avoid attacks (short cooldown) |
| **Special Skill** | P | Area damage attack, high impact (cooldown: 15s) |
| **Pause Menu** | ESC | Pause/resume gameplay or access settings |

---

## ⚙️ Features

### Combat & Gameplay
- ✅ **Real-time action combat** with responsive controls
- ✅ **Multiple attack patterns** - basic, ranged, and area attacks
- ✅ **Dynamic AI enemies** with pathfinding and varied combat tactics
- ✅ **Epic boss encounters** with multiple phases and unique attack patterns
- ✅ **Environmental strategy** - use wooden stakes for tactical advantages

### Visual & Audio
- ✅ **Pixel art aesthetic** with earthy Vietnamese color palette
- ✅ **Smooth animations** for all characters and attacks
- ✅ **Atmospheric sound design** - period-appropriate music and sound effects
- ✅ **Visual feedback** - particle effects, screen shake, UI notifications

### Game Flow
- ✅ **Main menu** with difficulty selection
- ✅ **Level progression** through multiple waves
- ✅ **Health/Energy management** system
- ✅ **Enemy variety** - soldiers, boat pilots, tactical generals
- ✅ **Win/Lose conditions** with replay functionality
- ✅ **Settings panel** for audio/graphics preferences

### Technical
- ✅ **Optimized performance** for smooth 60 FPS gameplay
- ✅ **Scalable difficulty levels** (Easy, Normal, Hard)
- ✅ **Save/Load system** for progression
- ✅ **Comprehensive input handling** for keyboard and mouse
- ✅ **VFX and particle systems** for visual impact

---

## 🧠 Game Mechanics

### Combat System
Players engage in fast-paced, skill-based combat with multiple attack options:

- **Basic Attack (Left Click)**: Quick strikes with moderate cooldown
- **Throwing Blades (Q)**: Energy-intensive attack that hits all directions
- **Dash (Space)**: Short invincibility frames for tactical evasion
- **Special Skill (P)**: Ultimate move dealing massive area damage

### Enemy Types
1. **Soldier (Common)** - Fast, low health, basic attacks
2. **Archer (Common)** - Ranged attacks, medium health
3. **Boat Pilot (Uncommon)** - High health, uses wooden stakes for cover
4. **General (Boss)** - Final encounter, multiple attack phases, strategic AI

### Environmental Mechanics
- **Wooden Stakes**: Can be used as cover or obstacles for tactical positioning
- **River Dynamics**: Water affects movement and certain attack patterns
- **Wave System**: Enemies spawn in increasing difficulty waves
- **Health Pickups**: Occasional item drops from defeated enemies

### Progression
- Defeat all enemy waves (10 waves total)
- Face the enemy general in final battle
- Victory grants achievements and unlocks harder difficulties

---

## 🏗️ Project Structure

```
Ngo-Quyen-Bach-Dang/
├── Assets/
│   ├── Scripts/
│   │   ├── Player/
│   │   │   ├── PlayerController.cs         # Main player movement & input
│   │   │   ├── PlayerCombat.cs            # Attack and skill system
│   │   │   └── PlayerStats.cs             # Health and energy management
│   │   ├── Enemy/
│   │   │   ├── EnemyBase.cs               # Base enemy class
│   │   │   ├── SoldierAI.cs               # Melee enemy AI
│   │   │   ├── ArcherAI.cs                # Ranged enemy AI
│   │   │   ├── BossAI.cs                  # Boss fight AI
│   │   │   └── EnemySpawner.cs            # Wave system
│   │   ├── Manager/
│   │   │   ├── GameManager.cs             # Overall game flow
│   │   │   ├── UIManager.cs               # UI system and HUD
│   │   │   ├── AudioManager.cs            # Sound management
│   │   │   └── SettingsManager.cs         # Game settings
│   │   ├── Environment/
│   │   │   ├── WoodenStake.cs             # Obstacle interaction
│   │   │   ├── RiverTile.cs               # Water mechanics
│   │   │   └── ObstacleManager.cs         # Environmental setup
│   │   ├── Utils/
│   │   │   ├── ObjectPooling.cs           # Performance optimization
│   │   │   └── EventSystem.cs             # Game event broadcasting
│   │   └── Input/
│   │       └── InputHandler.cs            # Centralized input management
│   ├── Scenes/
│   │   ├── MainMenu.unity
│   │   ├── Level_1_Bach_Dang.unity
│   │   ├── BossFight.unity
│   │   ├── GameOver.unity
│   │   └── Victory.unity
│   ├── Sprites/
│   │   ├── Character/                     # Ngo Quyen animations
│   │   ├── Enemies/                       # Enemy character sprites
│   │   ├── Environment/                   # Backgrounds, stakes, river
│   │   ├── UI/                            # Buttons, panels, HUD elements
│   │   └── VFX/                           # Particle effects sprites
│   ├── Animations/
│   │   ├── Player/
│   │   ├── Enemies/
│   │   └── Objects/
│   ├── Prefabs/
│   │   ├── Characters/
│   │   ├── Enemies/
│   │   ├── Items/
│   │   └── UI/
│   ├── Audio/
│   │   ├── BGM/                           # Background music
│   │   ├── SFX/                           # Sound effects
│   │   └── Voice/                         # Character voices
│   ├── Materials/
│   │   ├── Pixel.mat                      # Pixel art shader
│   │   └── Effects/
│   └── Resources/
│       ├── Config/
│       └── Data/
├── ProjectSettings/
├── Packages/
├── .gitignore
├── LICENSE
└── README.md
```

---

## 🚀 Installation & Setup

### Prerequisites
- **Unity 2020.3 LTS** or later
- **Windows 7 SP1** or later
- **4GB RAM** minimum
- **2GB disk space** for project

### Step 1: Clone the Repository
```bash
git clone https://github.com/yourusername/Ngo-Quyen-Bach-Dang.git
cd Ngo-Quyen-Bach-Dang
```

### Step 2: Open Project in Unity
1. Open Unity Hub
2. Click **Add** and select the project folder
3. Select **Unity 2020.3 LTS** (or compatible version)
4. Wait for project to load and assets to import

### Step 3: Verify Scene Setup
```
In the Editor:
1. Navigate to Assets/Scenes/
2. Open MainMenu.unity
3. Check File → Build Settings (ensure correct scene order)
```

### Step 4: Run the Game
- **Play in Editor**: Press `Ctrl + P` or click the Play button
- **Build Executable**: 
  ```
  File → Build Settings
  - Select PC, Mac & Linux Standalone
  - Choose Windows as target platform
  - Click Build and Run
  ```

---

## 🧪 How to Play

### Game Modes
1. **Main Menu** - Select difficulty and start game
2. **Gameplay** - Combat across 10 waves of enemies
3. **Boss Battle** - Final encounter with enemy general
4. **Victory/Defeat** - End screen with results and replay option

### Strategy Tips
- **Master Timing**: Use Dash (Space) to avoid incoming attacks at the last moment
- **Kite Enemies**: Keep moving and attack from range when possible
- **Use Environment**: Hide behind wooden stakes to block enemy attacks
- **Energy Management**: Q and P skills consume energy, manage cooldowns
- **Weak Spots**: Observe boss pattern and attack during recovery frames
- **Wave Preparation**: Each wave is stronger; position yourself strategically

### Difficulty Levels
| Difficulty | Enemy HP | Damage | Attack Speed | Rewards |
|-----------|----------|--------|--------------|---------|
| Easy | -30% | -25% | 0.8x | 1x |
| Normal | 100% | 100% | 1.0x | 1x |
| Hard | +40% | +40% | 1.3x | 1.5x |

---

## 🛠️ Tech Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| **Engine** | Unity | 2020.3 LTS |
| **Language** | C# | 9.0 |
| **Graphics API** | DirectX 11 / OpenGL | Latest |
| **Input System** | Unity.InputSystem | 1.3+ |
| **Animation** | Mecanim | Built-in |
| **Physics** | 2D Physics Engine | Built-in |
| **UI Framework** | uGUI | Built-in |
| **Audio** | Unity Audio | Built-in |
| **Version Control** | Git | 2.31+ |
| **IDE** | Visual Studio | 2019+ |

---

## 📦 Assets & Resources

### Free Assets Used
- **Pixel Art Font**: m5x7 by Daniel Linssen
- **Sound Effects**: Freesound.org community contributions
- **Background Music**: Copyright-free Vietnamese-inspired compositions
- **UI Framework**: TextMesh Pro

### Custom Assets
- **Character Sprites**: Original pixel art (24x32 resolution)
- **Enemy Designs**: Hand-crafted pixel art animations
- **Environmental Art**: Custom river and stake designs
- **Effect Particles**: In-house VFX design

### Asset Licensing
All custom assets are original and included in the project license.
Third-party assets follow their respective licenses (see CREDITS.md).

---

## 📌 Roadmap

### Version 1.0 ✅ (Current Release)
- ✅ Core combat mechanics
- ✅ 10-wave enemy progression
- ✅ Boss battle system
- ✅ Three difficulty levels
- ✅ Pixel art aesthetics
- ✅ Audio system

### Version 1.1 (Planned Q2 2026)
- 🔄 Additional enemy types (Elite guards, cavalry)
- 🔄 New special skills (Fire attack, Ice shards)
- 🔄 Leaderboard system
- 🔄 Accessibility improvements

### Version 1.2+ (Future)
- 🔄 Story mode with voice acting
- 🔄 Multiplayer survival mode
- 🔄 Cosmetic character skins
- 🔄 Additional historical campaigns
- 🔄 Mobile version (Android/iOS)
- 🔄 Mod support via Steam

---

## 🤝 Contributing

We welcome contributions! Please follow these guidelines:

1. **Fork** the repository
2. **Create** a feature branch: `git checkout -b feature/amazing-feature`
3. **Commit** changes: `git commit -m 'Add amazing feature'`
4. **Push** to branch: `git push origin feature/amazing-feature`
5. **Submit** a Pull Request with detailed description

### Contribution Areas
- Bug fixes and performance improvements
- New enemy types or boss phases
- UI/UX enhancements
- Documentation improvements
- Art asset contributions
- Audio/music contributions

See [CONTRIBUTING.md](CONTRIBUTING.md) for detailed guidelines.

---

## 📜 License

This project is licensed under the **MIT License** - see [LICENSE](LICENSE) file for full details.

### Summary
- ✅ You can use this project freely (personal, commercial)
- ✅ You must include a copy of the license
- ✅ You can modify and distribute the code
- ❌ No warranty is provided

---

## 👤 Author

**PRU212HungLD**
- GitHub: [@PRU212hungld](https://github.com/PRU212hungld)
- Email: hungld@example.com
- Portfolio: [your-portfolio.com](https://your-portfolio.com)

### Collaborators & Credits
- **Pixel Art**: Game Development Team
- **Sound Design**: Audio Specialists
- **Historical Consultation**: Vietnamese History Research

---

## 📞 Support & Feedback

Found a bug or have a suggestion?
- **Issues**: [GitHub Issues](https://github.com/PRU212hungld/Ngo-Quyen-Bach-Dang/issues)
- **Discussions**: [GitHub Discussions](https://github.com/PRU212hungld/Ngo-Quyen-Bach-Dang/discussions)
- **Email**: hungld@example.com

---

## 🔗 Additional Resources

- [Unity Documentation](https://docs.unity3d.com/)
- [Vietnamese History: Battle of Bach Dang](https://en.wikipedia.org/wiki/Battle_of_B%C3%A1ch_%C4%90%C4%83ng)
- [Game Design Document](https://github.com/PRU212hungld/Ngo-Quyen-Bach-Dang/wiki/GDD)
- [Development Blog](https://github.com/PRU212hungld/Ngo-Quyen-Bach-Dang/wiki)

---

<div align="center">

**Made with ❤️ by the Game Development Community**

*"History is written by those brave enough to change it."*

</div>
