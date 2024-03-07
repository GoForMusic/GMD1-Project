using System;
using Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Control
{
    [RequireComponent(typeof(Health))]
    public class MinionAI : MonoBehaviour
    {
        public PatrolPath patrolPath;
        public float moveSpeed = 5f;
        public float rotationSpeed = 5f;
        private Animator _animator;
        
        private int _currentWaypointIndex = 0;
        private Transform _minionMeshTransform;
        void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            //Set minion mesh transform
            _minionMeshTransform = _animator.transform;
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
            // Move towards the current waypoint
            Vector3 targetPosition = patrolPath.GetWaypoints()[_currentWaypointIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            _animator.SetFloat("MovementSpeed",transform.position.magnitude);
            
            // If the minion reaches the current waypoint, move to the next one
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                _currentWaypointIndex++;
                if (_currentWaypointIndex >= patrolPath.GetWaypoints().Length)
                {
                    _currentWaypointIndex = 0; // Loop back to the beginning
                }
            }
            
            // Rotate the minion to look at the next waypoint
            Vector3 direction = targetPosition - _minionMeshTransform.position;
            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                _minionMeshTransform.rotation = Quaternion.Slerp(_minionMeshTransform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}