using Core;
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
    [RequireComponent(typeof(Fighter))]
    public class PlayerController : MonoBehaviour, IHealthProvider
    {
        [Header("References")] [SerializeField]
        private Animator _animator;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private Camera _camera;
        
        [Header("PlayerUI")]
        [SerializeField]
        private Slider healthBar;

        [Header("Player starts")] 
        public float maxHealth = 100f;
        
        [Header("Movement")] 
        [SerializeField] private float _moveSpeed = 25f;
        [SerializeField] private float _rotateSpeed = 10f;
        [SerializeField] private float _weaponRange = 3f;
        //Other Core Elements
        private Fighter _fighter;
        private IHealth _health;
        
        //Interfaces
        private IMovement _movement;
        
        /// <summary>
        /// Initializes references and interfaces.
        /// </summary>
        void Start()
        {
            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
            _fighter = GetComponent<Fighter>();
            _fighter.SetWeaponRange(_weaponRange);
            _health = new HealthPlayer(healthBar, maxHealth, gameObject.tag, GetComponent<Collider>(), _animator);
            
            //Init Interface
            _movement = new Movement();
        }

        /// <summary>
        /// Handles player movement and combat every frame.
        /// </summary>
        void Update()
        {
            if (_health.IsDead()) return;

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
    }
}

