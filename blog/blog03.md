---
title: "Blog 3 Developer post"
author: "Adrian-Cristian Militaru"
date: "16-03-2024"
version: "0.1"
---

# Game Development Progress Report

## Introduction
In this blog post, I'll share the progress I've made so far on my Unity game project. I'll discuss various aspects, including character controllers, AI implementation, asset integration, animations and UI.

## Overview
My game is a MOBA (Multiplayer Online Battle Arena) set up for split-screen gameplay on an arcade machine. It involves character control, minion AI, combat mechanics, as well as UI elements.

## Code talking
### Character Controller
I began by implementing the character controller using Unity's new Input System. This allowed for smoother and more responsive player movement. Below is a snippet of the PlayerController script:

```csharp
namespace Control
{
    [RequireComponent(typeof(Fighter))]
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Animator _animator;

        [SerializeField] private Rigidbody _rb;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private Camera _camera;

        [Header("Movement")] 
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _roateSpeed = 5f;
        [SerializeField] private float _weaponRange = 2f;
        //Other Core Elements
        private Fighter _fighter;
        
        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();
            _playerInput = GetComponent<PlayerInput>();
            _fighter = GetComponent<Fighter>();
            _fighter.SetWeaponRange(_weaponRange);
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 movement = _playerInput.actions["Move"].ReadValue<Vector2>();
            var moveVector = MoveTowardTarget(new Vector3(movement.x, 0, movement.y));
            RotateTowardMovementVector(moveVector);
            _fighter.AttackBehavior(_animator, _playerInput.actions["Fire"].ReadValue<float>());
            //transform.Translate(targetVector * _moveSpeed * Time.deltaTime);
            _animator.SetFloat(AnimatorParameters.MovementSpeed, movement.magnitude);
        }

        private void RotateTowardMovementVector(Vector3 moveVector)
        {
            if (moveVector.magnitude == 0) return;

            var rotation = Quaternion.LookRotation(moveVector);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _roateSpeed);
        }

        private Vector3 MoveTowardTarget(Vector3 targetVector)
        {
            var speed = _moveSpeed * Time.deltaTime;

            targetVector = Quaternion.Euler(0, _camera.gameObject.transform.eulerAngles.y, 0) * targetVector;
            var targetPosition = transform.position + targetVector * speed;
            transform.position = targetPosition;
            return targetVector;
        }

    }
}
```
- References: Holds references to the player's animator, rigidbody, player input, and camera.
- Movement: Uses the new Input System to read player movement input and moves the player character accordingly. It also rotates the player to face the direction of movement.
- Combat: Allows the player to perform attacks based on input. It interfaces with the Fighter component to trigger attack animations.


### Minion AI
Creating AI for minions was crucial for the MOBA experience. I developed a robust AI system that controls minion behavior, including patrolling, attacking enemies, and following designated paths. Here's a snippet from the MinionAI script:

```csharp
using System;
using Core;
using Static;
using UnityEngine;
using UnityEngine.Serialization;

namespace Control
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Fighter))]
    public class MinionAI : MonoBehaviour
    {
        [Header("PatrolPath")]
        public PatrolPath patrolPath;
        [Header("Minion Stats")]
        public float moveSpeed = 5f;
        public float rotationSpeed = 5f;
        public float weaponRange = 2f;

        [Header("Root Minion Ref ONLY")]
        [SerializeField]
        private Transform _minionMeshTransform;
        
        private Animator _animator;
        //Other Core Elements
        private Fighter _fighter;
        private Health _health;
        private int _currentWaypointIndex = 0;
        
        void Start()
        {
            _animator = GetComponent<Animator>();
            _fighter = GetComponent<Fighter>();
            _fighter.SetWeaponRange(weaponRange);
            _health = GetComponent<Health>();
            // Make sure there is a PatrolPath assigned
            if (patrolPath == null)
            {
                Debug.LogError("PatrolPath not assigned to MinionAI!");
                enabled = false; // Disable the script
                return;
            }
            
            // Set the minion's position to the first waypoint
            transform.position = patrolPath.GetWaypoints()[0];
        }
        
        void Update()
        {
            if(_health.IsDead()) return;
            
            if (!SawEnemy())
            {
                MoveToWaypoint();
            }
            else
            {
                MoveToEnemy();
            }
        }
        private void MoveToWaypoint()
        {
            Vector3 targetPosition = patrolPath.GetWaypoints()[_currentWaypointIndex];
            _minionMeshTransform.position = Vector3.MoveTowards(_minionMeshTransform.position, targetPosition, moveSpeed * Time.deltaTime);
            _animator.SetFloat(AnimatorParameters.MovementSpeed, _minionMeshTransform.position.magnitude);

            if (Vector3.Distance(_minionMeshTransform .position, targetPosition) < 0.1f)
            {
                _currentWaypointIndex++;
                if (_currentWaypointIndex >= patrolPath.GetWaypoints().Length)
                {
                    _currentWaypointIndex = 0; // Loop back to the beginning
                }
            }

            Vector3 direction = targetPosition - transform.position;
            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }
        }
        private void MoveToEnemy()
        {
            
            bool isInrange = Vector3.Distance(_minionMeshTransform.position, _fighter.GetEnemyTarget().transform.position) <
                             weaponRange;
            if (!isInrange)
            {
                // Move towards the enemy position
                _minionMeshTransform.position = Vector3.MoveTowards(_minionMeshTransform.position, _fighter.GetEnemyTarget().transform.position, moveSpeed * Time.deltaTime);
                _animator.SetFloat(AnimatorParameters.MovementSpeed, moveSpeed);

                // Rotate towards the enemy position
                Vector3 direction = _fighter.GetEnemyTarget().transform.position - _minionMeshTransform.position;
                if (direction != Vector3.zero)
                {
                    Quaternion rotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
                }
            }
            else
            {
                _animator.SetFloat(AnimatorParameters.MovementSpeed, 0);
                _fighter.AttackBehavior(_animator,1f);
            }
        }
        
        // Set the enemy position and switch to enemy seeking mode
        public bool SawEnemy()
        {
            if (_fighter.GetEnemyTarget() != null) return true;
            else return false;
        }
    }
}
```

- References: Holds references to the minion's animator, fighter, and health components, as well as a reference to the patrol path.
- Initialization: Sets up initial parameters and ensures the existence of a patrol path.
- Update: Controls minion behavior based on whether an enemy is detected. If no enemy is seen, the minion moves along its patrol path. If an enemy is detected, the minion moves towards the enemy and attacks if within range.
- Movement: Handles movement towards waypoints or enemy targets, including rotation towards the target.
- Combat: Triggers attack animations and deals damage to enemies within range.


### PatrolPath

To visualize minion paths, I utilized the PatrolPath class, which allows for easy creation and editing of patrol routes directly in the Unity Editor.

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Control
{
    /// <summary>
    /// A class that is creating a patrol path on the editor
    /// </summary>
    public class PatrolPath : MonoBehaviour
    {
        private const float waypointGizmoRadius = 0.3f;
        [SerializeField] 
        [Range(1,2)]
        private int team=1;
        
        /// <summary>
        /// Drow on engine the waypoints and the path between
        /// </summary>
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (i == 0)
                {
                    Gizmos.DrawIcon(GetCurrentWayPointLocation(i),"sv_icon_dot9_pix16_gizmo");
                }
                else if(i==transform.childCount-1)
                {
                    Gizmos.DrawIcon(GetCurrentWayPointLocation(i),"sv_icon_dot14_pix16_gizmo");
                }
                else
                {
                    Gizmos.DrawIcon(GetCurrentWayPointLocation(i),"sv_icon_dot8_pix16_gizmo");
                }
                
                Gizmos.color = team == 1 ? Color.blue: Color.red;
                Gizmos.DrawLine(GetCurrentWayPointLocation(i),GetCurrentWayPointLocation(GetNextIndex(i)));
            }
        }

        /// <summary>
        /// Get the next Index from the list of waypoints
        /// </summary>
        /// <param name="i">The current index</param>
        /// <returns>0 if the next position is out of list or the next index</returns>
        public int GetNextIndex(int i)
        {
            if (i + 1 == transform.childCount)
            {
                return 0;
            }
            
            return i + 1;
        }
        
        /// <summary>
        /// Get current waypoint location on the map
        /// </summary>
        /// <param name="i">The current index</param>
        /// <returns>Vector3 position</returns>
        public Vector3 GetCurrentWayPointLocation(int i)
        {
            return transform.GetChild(i).transform.position;
        }
        
        public Vector3[] GetWaypoints()
        {
            Vector3[] waypoints = new Vector3[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                waypoints[i] = transform.GetChild(i).position;
            }
            return waypoints;
        }
    }
}
```

- Drawing Gizmos: Visualizes the waypoints and paths between them in the Unity Editor scene view.
- Waypoint Management: Provides methods to get the next waypoint index, get the position of the current waypoint, and get an array of all waypoints.
- Editor Script: Includes an editor script to facilitate the addition of waypoints in the Unity Editor.

Talking of the Editor script, I've come with a script that will be more easy to create a PatrolPath on the Scene:

```csharp
using UnityEngine;
using UnityEditor;
using Control;

namespace Editor
{
    /// <summary>
    /// A class for editor only
    /// </summary>
    public class SpawnWaypoint
    {
        /// <summary>
        /// Spawn the gameobj waypoint to the patrolpath gameobj shortcut ctrl+g
        /// </summary>
        [MenuItem("Tools/PatrolPath/Add Waypoint %g", false)]
        static void SpawnWaypointFromEditor()
        {
            if (Selection.activeGameObject.GetComponent<PatrolPath>() != null)
            {
                var selectObj = Selection.activeGameObject.GetComponent<PatrolPath>();
                new GameObject()
                {
                    name = $"Waypoint {selectObj.transform.childCount}",
                    transform =
                    {
                        position = selectObj.transform.position,
                        parent = selectObj.transform
                    }
                };
            }
            else if (Selection.activeGameObject.GetComponentInParent<PatrolPath>() != null)
            {
                var selectObjInParent = Selection.activeGameObject.GetComponentInParent<PatrolPath>();

                new GameObject()
                {
                    name = $"Waypoint {selectObjInParent.transform.childCount}",
                    transform =
                    {
                        position = selectObjInParent.transform.position,
                        parent = selectObjInParent.transform
                    }
                };
            }
        }

        /// <summary>
        /// Validate if the selected obj in editor contain PatrolPath script
        /// </summary>
        /// <returns>true or false</returns>
        [MenuItem("Tools/PatrolPath/Add Waypoint %g", true)]
        static bool SpawnWaypointValidation()
        {
            if (Selection.activeGameObject == null) return false;

            if (Selection.activeGameObject.GetComponent<PatrolPath>()
                || Selection.activeGameObject.GetComponentInParent<PatrolPath>()) return true;
            else return false;
        }
    }
}
```
- SpawnWaypointFromEditor(): This method is called when the user triggers the "Add Waypoint" command (Ctrl+G shortcut). It checks if the selected game object has a PatrolPath component attached directly or as a parent. If found, it creates a new waypoint game object as a child of the selected PatrolPath object.

- SpawnWaypointValidation(): This method is used to validate whether the "Add Waypoint" command should be enabled or disabled. It returns true if the selected game object (or its parent) has a PatrolPath component attached, indicating that a waypoint can be added. Otherwise, it returns false, disabling the command.


### Combat Mechanics
Combat mechanics were implemented for both players and minions. The Fighter class manages attack behaviors, including cooldowns and damage calculations. Health management ensures that entities can take damage and be defeated. Here's a snippet from the Fighter and Health classes:

```csharp
using System;
using System.Collections.Generic;
using Static;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Core
{
    /// <summary>
    /// A ckass that define a game object being a fighter
    /// </summary>
    public class Fighter : MonoBehaviour
    {
        /// <summary>
        /// A var that will set the enemy team tag.
        /// </summary>
        private string _enemyTeamTag;
        /// <summary>
        /// Deal dmg to enemy var
        /// </summary>
        [SerializeField]private float dealDmg = 0.1f;
        /// <summary>
        /// Time between attack used for animations
        /// </summary>
        [SerializeField] private float timeBetweenAttack = 0.5f;
        /// <summary>
        /// Time between attack used for animations
        /// </summary>
        [SerializeField] private int noOfAttacks = 3;
        /// <summary>
        /// Time since last attack calculate with Infinity and deltatime for animation
        /// </summary>
        private float _timer = 0f;
        private bool _hasAttacked = false;

        /// <summary>
        /// The enemy target
        /// </summary>
        private GameObject _target;
        /// <summary>
        /// Set weapon attack range from Minion or player
        /// </summary>
        private float _weaponAttackRange;
        
        private void Awake()
        {
            if (gameObject.CompareTag("Team1"))
            {
                _enemyTeamTag = "Team2";
            }else if (gameObject.CompareTag("Team2"))
            {
                _enemyTeamTag = "Team1";
            }
        }

        public void AttackBehavior(Animator animator, float attackInput)
        {
            if (!_hasAttacked && attackInput >= 1f)
            {
                _hasAttacked = true;
                _timer = 0f;
                animator.SetInteger(AnimatorParameters.AttackRandom,Random.Range(1, noOfAttacks+1));
                animator.SetTrigger(AnimatorParameters.Attack);
            }
            
            if (_hasAttacked)
            {
                _timer += Time.deltaTime;
                if (_timer >= timeBetweenAttack)
                {
                    _timer = 0;
                    _hasAttacked = false;
                }
            }
        }

        /// <summary>
        /// Hit event when hit with the animation
        /// </summary>
        void Hit()
        {
            if (_target ==null) return;


            if (Vector3.Distance(transform.position, _target.transform.position) < 2.0f)
            {
                Health enemyHealth = _target.GetComponent<Health>();
                enemyHealth.DealDamage(dealDmg);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(_enemyTeamTag))
            {
                _target=other.gameObject;
            }
        }
        
        public GameObject GetEnemyTarget()
        {
            return _target;
        }

        public void SetWeaponRange(float weaponAttackRange)
        {
            this._weaponAttackRange = weaponAttackRange;
        }
    }
}
```

- Initialization: Determines the enemy team tag based on the tag of the game object this script is attached to.
- Attack Behavior: Controls attack animations and cooldowns based on player input.
- Combat Logic: Handles dealing damage to enemies when in range and triggers hit animations.

### Health Management

The `Health.cs` script is just a generic component that will hold and manage the health of characters (players and minions). Here's what it does:

```csharp
using System;
using System.Collections;
using Static;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core
{
    /// <summary>
    /// Health class object
    /// </summary>
    public class Health : MonoBehaviour
    {
        /// <summary>
        /// parameter of health points
        /// </summary>
        public float maxHealth = 100f;
        private float _currentHealth;
        [SerializeField]
        private Slider healthBar;
        
        /// <summary>
        /// if gameobj is dead
        /// </summary>
        private bool _isDead = false;

        /// <summary>
        /// A method that will return if the player or AI is dead
        /// </summary>
        /// <returns>true or false</returns>
        public bool IsDead()
        {
            return _isDead;
        }
        
        private void Start()
        {
            _currentHealth = maxHealth;
            UpdateHealthBar();
        }
        
        /// <summary>
        /// Take damage, if health == 0 , than die
        /// </summary>
        /// <param name="damage">damage amount you receive</param>
        public void DealDamage(float damage)
        {
            Debug.Log("Get dmg :" + damage);
            _currentHealth = Mathf.Max(_currentHealth-damage,0);
            UpdateHealthBar();
            if (_currentHealth == 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Die animation
        /// </summary>
        private void Die()
        {
            if(_isDead) return;
            _isDead = true;
            GetComponent<Animator>().SetTrigger(AnimatorParameters.Die);
        }
        
        /// <summary>
        /// A method that will update the UI element (Health BAR)
        /// </summary>
        void UpdateHealthBar()
        {
            float normalizedHealth = _currentHealth / maxHealth;
            healthBar.value = normalizedHealth;
        }
    }
}
```

- Initialization: Sets up initial health values and links to a health bar UI element.
- Taking Damage: Allows characters to take damage and updates the health bar accordingly.
- Death Handling: Triggers death animations and flags the character as dead when health reaches zero.

### Spawning Minions
In this script I wrote the logic of handling the spawning of minions at regular intervals. Here's what it does:

```csharp
using System.Collections;
using System.Collections.Generic;
using Control;
using UnityEngine;

public class SpawnMinions : MonoBehaviour
{
    [Header("Minion Que for team")]
    [SerializeField]
    private List<GameObject> _minions;
    
    [Header("Spawner Properties")]
    [SerializeField] private float _spawnInterval = 30.0f;
    [SerializeField] private float _delayBetweenMinions=1f;

    [SerializeField]
    private PatrolPath _patrolPath;
    
    private void Start()
    {
        _patrolPath = GetComponent<PatrolPath>();
        if (_patrolPath == null)
        {
            Debug.LogError("PatrolPath component not found on the spawner object!");
        }
        else
        {
            StartCoroutine(SpawnMinionsAsync());
        }
    }

    private IEnumerator SpawnMinionsAsync()
    {
        while (true)
        {
            for (int i = 0; i < _minions.Count; i++)
            {
                SpawnRegularMinions(_minions[i]);
                yield return new WaitForSeconds(_delayBetweenMinions);
            }

            yield return new WaitForSeconds(_spawnInterval - _delayBetweenMinions);
        }
    }

    private void SpawnRegularMinions(GameObject minion)
    {
        GameObject objectSpawned = Instantiate(minion, transform.position, transform.rotation);
        objectSpawned.GetComponentInChildren<MinionAI>().patrolPath = _patrolPath;
    }
}
```

- Initialization: Sets up parameters for spawning, including spawn interval and delay between minions.
- Spawning Coroutine: Utilizes a coroutine to spawn minions asynchronously at defined intervals.
- Minion Initialization: Instantiates minions at the spawner's position and assigns them a patrol path.


## Asset Integration and UI
I integrated 3D assets and animations to breathe life into characters and environments. Additionally, I set up UI elements such as health bars to provide players with vital information during gameplay. For UI elements, I utilized external packages for textures and also employed the [Inkscape](https://inkscape.org/) application to edit some textures, such as creating the blue banners.

### Overview of the Scene and hierarchy

![Scene Overview](/blog/resources/blog03/scene_overview.png)
![Hierarchy Overview](/blog/resources/blog03/hierarchy_overview.png)

The scene comprises various game objects, including players, minions, environment assets, and UI elements, all meticulously organized hierarchically in the Unity Editor for improved management and accessibility.

### New Input System

![New Input System key bindings](/blog/resources/blog03/newInputSystem.png)

The new Input System in Unity offers enhanced flexibility and efficiency in managing player input. By implementing this system, player controls become more responsive and customizable, thereby enhancing the overall gaming experience. 

**Notably**, I managed to configure two Action maps for each player, accommodating different key bindings from the same keyboard or the same PS controller.

![PS controller](/blog/resources/blog03/ps_controller_keyMap.png)

### Character Animation and Minion Animation

![Character Base Layer Animation](/blog/resources/blog03/character_base_layer_animation.png)

![Character Base Layer  blend tree](/blog/resources/blog03/character_base_layer_blendtree.png)

Animations play an essential role in animating characters, make the meshes movements such as walking and attacking. Using Unity's animation system, I utilized a blend tree for both player characters and minions, allowing smooth transitions between animations. **Notably**, the MovementSpeed float variable determines the speed of the mesh, with 0 representing idle and 1 representing forward movement.

![Character Attack Layer Animation](/blog/resources/blog03/character_attack_layer_animation.png)

For attacking animations, I created a separate layer using a special avatar mask, restricting animation to the upper body of the skeleton. Using triggers and integer parameters like Attack and AttackRandom, I control the initiation and having a combat animations.

![Attack Mask](/blog/resources/blog03/avatar_mask.png)


### UI elements

#### Health Bar
![Health Bar](/blog/resources/blog03/Minion_health_bar.png)

UI elements, particularly health bars, are crucial for conveying vital information to players. In my game, I designed a health bar that displays the current health status of both characters and minions, enabling players to monitor their health. The minion UI element is represented as a world object, while the player's Health bar is represented as 2D canvas element.

#### UI HUD

![UI HUD](/blog/resources/blog03/UI_Overall.png)

The UI HUD I designed serves to represent player status, featuring indicators such as health (red bar), experience (yellow bar), and player level. Additionally, I incorporated design details such as separators for screen views and prepared the UI for a generic minimap, enhancing gameplay experience, especially for arcade machine setups.

#### Main Menu

![Main Menu UI](/blog/resources/blog03/MainMenu.png)

Regarding the MainMenu, I've just set-up somethign simple and try to use the new input system in order to selecte the elements on the canvas.

### PatrolPathUI

![Patrol Path UI](/blog/resources/blog03/patrol_path_ui.png)

The PatrolPathUI provides an intuitive interface for editing patrol paths directly in the Unity Editor. With this tool, level designers can easily create and modify patrol routes for minions, enhancing the AI behavior and overall gameplay experience.

And for a better understanding I've make the Gizmos to create some specials icons for example the blue diamon represent the start of the Path where the red one represent the end of, and white represent that bing one of many waypoints alongside.

In addition The patrol path will have a Team varaible that will work in both UI and script wise. (swoing the right color of the team)

## Conclusion for this blog
In summary, substantial strides have been made in advancing the core gameplay mechanics of my game project. With the implementation of character controllers, AI behavior, combat systems, and asset integration, the groundwork for an immersive gaming experience has been established.

Looking ahead to the next phase of development, several key objectives are gonna be implemented such as:
- Addressing AI behavior issues, such as out-of-range detection and path-following optimizations.
- Introducing new gameplay elements, including capture-the-flag mechanics to enrich the player experience.
- Incorporating a minimap feature to enhance navigation and spatial awareness within the game world.
- Implementing death animations to add realism and depth to character interactions.

These planned enhancements aim to elevate the overall quality and depth of the gameplay experience, providing players with engaging challenges and immersive immersion.

Be sure to stay tuned for future updates as I continue to refine and expand upon these features, bringing the game project closer to its full potential!