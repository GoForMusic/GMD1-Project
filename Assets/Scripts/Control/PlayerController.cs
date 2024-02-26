using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]private Animator _animator;
    [SerializeField]private Rigidbody _rb;
    [SerializeField]private PlayerInput _playerInput;
    [SerializeField] private Camera _camera;
    
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 10f;

    [SerializeField] private float _roateSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = _playerInput.actions["Move"].ReadValue<Vector2>();
        var moveVector = MoveTowardTarget(new Vector3(movement.x, 0, movement.y));
        RotateTowardMovementVector(moveVector);
        _animator.SetBool("Attack",_playerInput.actions["Fire"].ReadValue<float>()==1?true:false);
        //transform.Translate(targetVector * _moveSpeed * Time.deltaTime);
        _animator.SetFloat("MovementSpeed",movement.magnitude);
    }

    private void RotateTowardMovementVector(Vector3 moveVector)
    {
        if(moveVector.magnitude==0) return;
        
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

