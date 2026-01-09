# AI_Assigment
---

## Overview
This is my AI assigment about is about a squad mechanic where the player can command the companion AI to hold, search, follow with commands and can shoot automaticly when AI sees enemy. 
The same goes for the enemy AI, that can start with different states like hold position, go between waypoint or search/follow a another AI.

---
```
Assets
├── Images                                     # Stores all image files like textures, sprites, and icons
├── Material                                   # Materials used for 3D models or sprites
├── Prefab                                     # Prefabricated objects for reuse in scenes
├── Scenes                                     # Unity scenes
├── Scripts                                    # All C# scripts controlling game logic
│   ├── GameManager.cs                         # Main game manager script
│   ├── Common                                 # Shared/common scripts
│   │   ├── SensingView.cs                     # Script for detecting objects or AI sensing
│   │   ├── AI                                 # AI behavior scripts
│   │   │   ├── AiBrain.cs                     # AI logic controller
│   │   │   └── AiWalk.cs                      # AI movement logic
│   │   └── Interfaces                         # Common interfaces for code abstraction
│   │       └── IHealth.cs                     # Health interface
│   ├── Factory                                # Factory scripts for creating objects at runtime
│   │   ├── CharacterFactory.cs
│   │   └── WeaponFactory.cs
│   ├── Player                                 # Player-related scripts
│   │   ├── PlayerBrain.cs
│   │   ├── PlayerController.cs
│   ├── Statemachine                           # AI state machine system
│   │   ├── Common                             # Shared state machine scripts
│   │   │   ├── StateMachineFactory.cs
│   │   │   ├── StateManager.cs
│   │   │   └── States                         # Individual AI states
│   │   │       ├── FollowState.cs
│   │   │       ├── HoldState.cs
│   │   │       ├── HuntState.cs
│   │   │       ├── LastKnownPosState.cs
│   │   │       ├── MediState.cs
│   │   │       ├── SearchState.cs
│   │   │       └── WaypointState.cs
│   │   ├── Enemy                               # Enemy-specific AI scripts
│   │   │   └── EnemyStateManager.cs
│   │   └── Friendly                            # Friendly NPC AI scripts
│   │       └── FriendlyStateManager.cs
│   ├── Utility                                 # Utility/helper scripts
│   │   └── Singleton.cs                        # Generic singleton class
│   └── Weapon                                  # Weapon scripts
│       ├── AttackScript.cs
│       ├── BulletScript.cs
│       ├── Pistol.cs
│       └── Rifle.cs
```

            
