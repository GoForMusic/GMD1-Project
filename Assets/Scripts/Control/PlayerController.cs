using Core;
using Static;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Control
{
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
        [SerializeField] private float _roateSpeed = 5f;
        [SerializeField] private float _weaponRange = 2f;
        //Other Core Elements
        private Fighter _fighter;
        
        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
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
            // Calculate movement direction
            targetVector = Quaternion.Euler(0, _camera.gameObject.transform.eulerAngles.y, 0) * targetVector;
            targetVector.Normalize(); // Normalize the vector to ensure consistent movement speed

            // Move the player
            _controller.Move(targetVector * _moveSpeed * Time.deltaTime);
            
            return targetVector;
        }

    }
}

