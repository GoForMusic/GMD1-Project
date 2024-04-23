using System;
using Interfaces.Control;
using Interfaces.Core;
using Static;
using Stats;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Control
{
    /// <summary>
    /// Controls player movement and combat.
    /// </summary>
    public class PlayerController : MonoBehaviour, IHealthProvider
    {
        [Header("References")] [SerializeField]
        private Animator _animator;
        private CharacterController _controller;
        private PlayerInput _playerInput;
        [SerializeField] private Camera _camera;
        
        [Header("PlayerUI")]
        [SerializeField]
        private UIManager uiManager;

        [Header("Config file")]
        [SerializeField]
        private StatsConfig _statsConfig;
        
        //Other Core Elements
        private IFighter _fighter;
        private IHealth _health;
        private IMovement _movement;

        private Vector3 _sp;
        
        /// <summary>
        /// Initializes references and interfaces.
        /// </summary>
        void Start()
        {
            uiManager.Init(_statsConfig.levelUpMultiplier, _statsConfig.maxLevel);
            _sp = transform.position;
            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
            switch (_statsConfig.attackType)
            {
                case AttackType.Melee:
                    _fighter = new FighterMelee(gameObject.tag,
                        _statsConfig.dealDmg,
                        _statsConfig.timeBetweenAttack,
                        _statsConfig.noOfAttacks,
                        _statsConfig.weaponRange);
                    break;
                default:
                    Debug.LogError("Unknown attack type!");
                    break;
            }
            
            _health = new HealthPlayer(uiManager,
                _statsConfig.maxHealth,
                gameObject.tag,
                GetComponent<Collider>(),
                _animator,
                _controller,
                this,
                10f,
                _sp);
            
            uiManager.UpdateDamageText(_statsConfig.dealDmg);
            //Init Interface
            _movement = new Movement();
        }

        /// <summary>
        /// Handles player movement and combat every frame.
        /// </summary>
        void Update()
        {
            if (_health.IsDead())
            {
                return;
            }
            
            if (_fighter.GetEnemyTarget() != null)
            {
                IHealth enemyHealth = _fighter.GetEnemyTarget().GetComponent<IHealthProvider>().GetHealth();
                if (enemyHealth != null && enemyHealth.IsDead())
                {
                    if (enemyHealth is HealthPlayer)
                    {
                        // Update experience UI with 200 experience points for killing a player
                        uiManager.UpdateExperience(200);
                    }
                    else if (enemyHealth is HealthMinion)
                    {
                        // Update experience UI with 50 experience points for killing a minion
                        uiManager.UpdateExperience(50);
                    }
                    
                    // Clear the target if it's dead
                    _fighter.SetEnemyTarger(null);
                }
            }
            
            Vector2 movement = _playerInput.actions["Move"].ReadValue<Vector2>();

            // Call Move and Rotate methods from the IMovement interface
            Vector3 moveVector = _movement.Move(new Vector3(movement.x, 0, movement.y), _camera.gameObject.transform);
            Quaternion rotation = _movement.Rotate(moveVector,transform);

            // Apply the movement and rotation to the player
            _controller.Move(moveVector * _statsConfig.moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _statsConfig.rotationSpeed);

            // Perform attack behavior and update animator
            _fighter.AttackBehavior(_animator, _playerInput.actions["Fire"].ReadValue<float>());
            _animator.SetFloat(AnimatorParameters.MovementSpeed, movement.magnitude);
        }
        
        public IHealth GetHealth()
        {
            return _health;
        }
        
        /// <summary>
        /// Triggered when another collider stays inside the trigger collider.
        /// </summary>
        /// <param name="other">The collider entering the trigger.</param>
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag(_fighter.GetEnemyTag()))
            {
                _fighter.SetEnemyTarger(other.gameObject);
            }
        }

        /// <summary>
        /// Triggered when the minion attacks.
        /// </summary>
        private void Hit()
        {
            _fighter.Hit(transform.position);
        }
    }
}

