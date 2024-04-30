---
title: "Blog 4 Developer post 2"
author: "Adrian-Cristian Militaru"
date: "30-04-2024"
version: "0.1"
---

# Game Development Progress Report 2

## Introduction

I'll give an update on the latest goings-on with my Unity game project in this blog article. I'll go into detail about how SOLID principles were used to the IFighter, IMovement, and IHealth interfaces, as well as how an object pool system was integrated.

In additional the new flag system was create as well an enhanced update to the UI.

## Implementation of SOLID Principles

### IHealth

I created the IHealth interface in accordance with the Single Responsibility Principle (SRP). The methods for handling health-related features, such as calculating damage and managing death events, are defined in this interface. By doing so, I've ensured that the Health class focuses solely on health management. This separation of health-related activities into a distinct interface promotes code readability and maintainability.

![Ihealth](/blog/resources/blog04/class-diagram-IHealth.png)

For example, the player needs to handle its own actions regarding health management, while minions, upon dying, need to be restored to the object pooling system.

### IMovement
I designed the IMovement interface to encapsulate functionality related to movement, adhering to the Interface Segregation Principle (ISP). This interface defines methods for common movement behaviors such as walking, running, and rotating. By implementing ISP, I've avoided forcing classes to include unnecessary methods, resulting in more logical and modular code.

![IMovement](/blog/resources/blog04/class-diagram-IMovement.png)

For example, the player class will only handle movement-related functionalities, while the minion class will implement both the IMovement interface for movement behaviors and the IMinionBehavior interface for additional minion-specific actions.

### IFighter

I introduced the IFighter interface to manage combat-related behaviors, aiming for greater extensibility and flexibility. This interface encompasses methods for initiating attacks, managing cooldowns, and identifying opponent targets. By adhering to the Open/Closed Principle (OCP), I've designed the IFighter interface to be open for extension but closed for modification. This approach simplifies the integration of new fighting mechanics without necessitating changes to existing code.

![IFighter](/blog/resources/blog04/class-diagram-IFighter.png)

For example, the IFighter interface can be utilized to distinguish between melee and ranged minions. In the case of ranged minions, additional logic such as projectile spawning may be required.

## Object Pooling

In the Unity game project, I implemented an object pooling system to efficiently manage game objects, particularly minions and projectiles. The Object Pool Manager and the Pool Manager Database work together to achieve this.

![Object Pool](/blog/resources/blog04/class-diagram-ObjectPooling.png)

### Object Pool Manager

The Object Pool Manager script is responsible for managing the pools of game objects. Here's how it works:

- Initialization: Upon Awake, the Object Pool Manager initializes the pools based on the data stored in the Pool Manager Database.
- Pool Management: It dynamically manages the size of each pool and provides methods for retrieving and returning minion objects to the pool.
- Efficient Object Usage: Minion objects are retrieved from the pool when needed and returned to the pool when no longer in use. This avoids the overhead of instantiating and destroying objects during gameplay, leading to improved performance.

### Pool Manager Database

The Pool Manager Database script serves as a centralized storage for minion prefabs and associated data. Key functionalities include:

- Prefab Retrieval: It retrieves the prefab associated with a specific key, facilitating the instantiation of minion objects/projectile.
- Pool Size Retrieval: It retrieves the initial pool size for each type of prefab, ensuring that an adequate number of objects are pre-instantiated for efficient pooling.
- Team Association: Additionally, it provides information about the team affiliation of each minion type, which can be useful for gameplay mechanics such as team-based combat.

### Integration with Minions
In the project, I seamlessly integrated the object pooling system with SpawnMinions class as well when a range minion need to spawn a projectile . Object are retrieved from the pool when spawned and returned to the pool when defeated or no longer needed. This approach optimizes memory usage and performance, contributing to a smoother and more responsive gameplay experience.

## Flags (gameplay)

The Flag class is responsible for managing the capture mechanics of flags placed within the game world. Here's how it works:

- Objective Definition: Each flag represents a strategic location that players must capture and defend to gain a competitive advantage.
- Capture Mechanics: When players or minions from the opposing teams enter the capture zone of a flag, a capture process is initiated. The capture progress is halted if members from both teams are present in the zone.

Let's dive into the technical aspects of the Flag class:

- Capture Time: Flags have a predefined capture time, during which they gradually change ownership based on the presence of allied or enemy units in the capture zone.
- Team Association: Flags are associated with specific teams, visually represented by distinct materials and minimap icons.
- Event Handling: The Flag class subscribes to the OnDeathHandle event of player and minion objects within its capture zone. When a subscribing object trigger the death event, it will be removed from the list of units contributing to the capture progress.

Here's how the Flag class can be utilized in gameplay scenarios:

- Objective-Based: Players gain points by capturing flags. For example, if there are six flags in total, capturing all six flags will result in the player's score increasing by a factor of six every ten seconds. Initially, the player earns ten points every ten seconds, and this value is multiplied by the number of flags under their control.

By incorporating dynamic objectives like flags into the game, I aim to create immersive and compelling gameplay experiences that encourage players to outscore their opponents.

```csharp
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Control;
using Interfaces.Core;
using UnityEngine;

namespace Gameplay
{
    public class Flag : MonoBehaviour
    {
        [Header("Flag properties")]
        [SerializeField]
        private float captureTime = 2f;
        [SerializeField]
        [Range(1,2)]
        private int _teamID; // Team ID (1 for Team1, 2 for Team2)
        
        public Material[] teamMaterial;
        public Material[] minimapMaterialMark;
        
        private bool _capturing = false;
        private IEnumerator _captureCoroutine;
        [SerializeField]
        private string _currentCapturingTeam;
        
        private Renderer _flagRenderer;
        private Renderer _flagMiniMapRenderer;
        
        [SerializeField]
        private List<GameObject> _minionsInCaptureZone;
        
        private void Start()
        {
            _flagRenderer = GetComponentsInChildren<Renderer>()[1];
            _flagMiniMapRenderer = GetComponentsInChildren<Renderer>()[0];
            _minionsInCaptureZone = new List<GameObject>();
            SetupFlag();
        }
        
        private void SetupFlag()
        {
            // Convert team ID to team name
            string teamName = _teamID == 1 ? "Team1Flag" : "Team2Flag";

            // Change flag material based on the specified team
            switch (teamName)
            {
                case "Team1Flag":
                    _flagRenderer.material = teamMaterial[0];
                    _flagMiniMapRenderer.material = minimapMaterialMark[0];
                    _currentCapturingTeam = "Team2";
                    break;
                case "Team2Flag":
                    _flagRenderer.material = teamMaterial[1];
                    _flagMiniMapRenderer.material = minimapMaterialMark[1];
                    _currentCapturingTeam = "Team1";
                    break;
            }
            
            // Set the tag based on the specified team
            gameObject.tag = teamName;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // Check if the entering GameObject is a player or minion
            if (other.CompareTag("Team1") || other.CompareTag("Team2"))
            {
                other.gameObject.GetComponent<IHealthProvider>().GetHealth().OnDeathHandle += DeathHandler;
                _minionsInCaptureZone.Add(other.gameObject);
                // Check if the entering GameObject belongs to one of the allowed teams
                if (!_capturing && AreAllMembersSameTeam())
                {
                    if (!gameObject.CompareTag(other.tag + "Flag"))
                    {
                        _currentCapturingTeam = other.tag;
                        _captureCoroutine = CaptureCoroutine();
                        StartCoroutine(_captureCoroutine);
                    }
                }
            }
        }

        private bool AreAllMembersSameTeam()
        {
            foreach (var minion in _minionsInCaptureZone)
            {
                if (!minion.CompareTag(_currentCapturingTeam))
                {
                    return false;
                }
            }

            return true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            // If a player or minion exits the collider, stop capturing
            if (other.CompareTag("Team1") || other.CompareTag("Team2"))
            {
                other.gameObject.GetComponent<IHealthProvider>().GetHealth().OnDeathHandle -= DeathHandler;
                _minionsInCaptureZone.Remove(other.gameObject);
                // Check if the exiting GameObject is from the same team as the current capturing team
                if (_capturing && AreAllMembersSameTeam())
                {
                    StopCoroutine(_captureCoroutine);
                    _capturing = false;
                    _currentCapturingTeam = gameObject.tag; // Reset current capturing team
                }
            }
        }
        
        
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Team1") || other.CompareTag("Team2"))
            {
                if (AreAllMembersSameTeam() && !gameObject.CompareTag(other.tag + "Flag"))
                {
                    if (!_capturing)
                    {
                        _captureCoroutine = CaptureCoroutine();
                        StartCoroutine(_captureCoroutine);
                    }
                }
               
            }
        }

        private void DeathHandler(IHealth obj)
        {
            GameObject gameobject = _minionsInCaptureZone.Find(g =>
                g.gameObject.GetComponent<IHealthProvider>().GetHealth() == obj);
            if (gameobject != null && _minionsInCaptureZone.Contains(gameobject))
            {
                _minionsInCaptureZone.Remove(gameobject);
                // Additional logic if needed
            }
        }
        
        private IEnumerator CaptureCoroutine()
        {
            _capturing = true;
            Debug.Log("Start capturing!");
            yield return new WaitForSeconds(captureTime);
            Debug.Log("Capture completed!");
            // Capture completed
            // Change flag material based on the capturing team
            if (_currentCapturingTeam == "Team1")
            {
                gameObject.tag = "Team1Flag";
                _flagRenderer.material = teamMaterial[0];
                _flagMiniMapRenderer.material = minimapMaterialMark[0];
                _currentCapturingTeam = "Team2";
            }
            else if (_currentCapturingTeam == "Team2")
            {
                gameObject.tag = "Team2Flag";
                _flagRenderer.material = teamMaterial[1];
                _flagMiniMapRenderer.material = minimapMaterialMark[1];
                _currentCapturingTeam = "Team1";
            }
            
            
            _capturing = false;
        }
    }
}
```

## new UI

### In-Game HUD Update

![UI HUD Game](/blog/resources/blog04/ingame-ui.png)

- Health and Experience Display: The in-game HUD now includes visual indicators for the player's health and experience points. This allows players to quickly assess their current status during gameplay.
- Damage: The UI will also display the damage that the player can deal. For example, this value will vary based on the player's level.
- Flag Counter: A new element has been added to the HUD to show how many flags the player currently controls. This information is crucial for objective-based gameplay, providing players with a clear overview of their progress.
- Level Indicator: Additionally, the HUD now includes a level indicator, showing the player's current level. This helps players track their progression throughout the game.
- Minimap: At the top of the screen is the minimap view, which displays both the players and the flags using team colors.

### MainMenu Update

![MainMenu UI PlayerInput](/blog/resources/blog04/mainmenu-inputname.png)

- Player Name Input: The main menu now features an updated method for inputting player names. Inspired by classic arcade games, this method offers a nostalgic and intuitive way for players to enter their names. This adds a fun and interactive element to the menu experience.

Later on, those names will be used to populate a scoreboard using a file-based system.

## Conclusion for this blog

In this blog post, I've provided an update on the progress of the game project, highlighting key developments in both gameplay mechanics and user interface enhancements. I've discussed the implementation of SOLID principles, which have guided the design of the interfaces for health, movement, and combat behaviors. By adhering to these principles, I've ensured a modular and extensible codebase that promotes maintainability and scalability.

Looking ahead, I am excited to continue refining and expanding upon these features as I progress further in the development journey. Stay tuned for more updates and insights into my game development process in future blog posts.
