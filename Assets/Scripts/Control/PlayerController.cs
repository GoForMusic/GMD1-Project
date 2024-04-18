using Control.Interfaces;
using Control.Interfaces.Core;
using Core;
using Static;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Control
{
    /// <summary>
    /// Controls player movement and combat.
    /// </summary>
    [RequireComponent(typeof(Fighter))]
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Animator _animator;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private Camera _camera;
        
        [Header("Movement")] 
        [SerializeField] private float _moveSpeed = 25f;
        [SerializeField] private float _rotateSpeed = 10f;
        [SerializeField] private float _weaponRange = 3f;
        //Other Core Elements
        private Fighter _fighter;
        private Health _health;
        
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
            _health = GetComponent <Health>();
            
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
    }
}

