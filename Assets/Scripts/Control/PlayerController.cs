using System;
using Interfaces.Control;
using Interfaces.Core;
using Static;
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
        private Slider healthBar;
        
        [Header("Player Stats")] 
        [SerializeField] private float _moveSpeed = 25f;
        [SerializeField] private float _rotateSpeed = 10f;
        [SerializeField] private float _weaponRange = 2f;
        public float maxHealth = 100f;
        public float dealDmg = 50f;
        public float timeBetweenAttack = 0.5f;
        public int noOfAttacks = 3;
        [SerializeField]
        private AttackType attackType;
        
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
            _sp = transform.position;
            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
            switch (attackType)
            {
                case AttackType.Melee:
                    _fighter = new FighterMelee(gameObject.tag,dealDmg,timeBetweenAttack,noOfAttacks,_weaponRange);
                    break;
                case AttackType.Range:
                    _fighter = new FighterRange(gameObject.tag,dealDmg,timeBetweenAttack,noOfAttacks,_weaponRange);
                    break;
                default:
                    Debug.LogError("Unknown attack type!");
                    break;
            }
            
            _health = new HealthPlayer(healthBar,
                maxHealth,
                gameObject.tag,
                GetComponent<Collider>(),
                _animator,
                _controller,
                this,
                10f,
                _sp);
            
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
                    // Clear the target if it's dead
                    _fighter.SetEnemyTarger(null);
                }
            }
            
            Vector2 movement = _playerInput.actions["Move"].ReadValue<Vector2>();

            // Call Move and Rotate methods from the IMovement interface
            Vector3 moveVector = _movement.Move(new Vector3(movement.x, 0, movement.y), _camera.gameObject.transform);
            Quaternion rotation = _movement.Rotate(moveVector,transform);

            // Apply the movement and rotation to the player
            _controller.Move(moveVector * _moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed);

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

