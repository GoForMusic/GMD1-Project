using System;
using Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Control
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Fighter))]
    public class MinionAI : MonoBehaviour
    {
        public PatrolPath patrolPath;
        public float moveSpeed = 5f;
        public float rotationSpeed = 5f;
        private Animator _animator;
        
        //Other Core Elements
        private Fighter _fighter;
        
        private int _currentWaypointIndex = 0;
        private Transform _minionMeshTransform;
        void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            _fighter = GetComponent<Fighter>();
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
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            _animator.SetFloat("MovementSpeed", transform.position.magnitude);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                _currentWaypointIndex++;
                if (_currentWaypointIndex >= patrolPath.GetWaypoints().Length)
                {
                    _currentWaypointIndex = 0; // Loop back to the beginning
                }
            }

            Vector3 direction = targetPosition - _minionMeshTransform.position;
            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                _minionMeshTransform.rotation = Quaternion.Slerp(_minionMeshTransform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }
        }
        private void MoveToEnemy()
        {
            // Move towards the enemy position
            transform.position = Vector3.MoveTowards(transform.position, _fighter.GetEnemyTarget().transform.position, moveSpeed * Time.deltaTime);
            _animator.SetFloat("MovementSpeed", moveSpeed);

            // Rotate towards the enemy position
            Vector3 direction = _fighter.GetEnemyTarget().transform.position - _minionMeshTransform.position;
            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                _minionMeshTransform.rotation = Quaternion.Slerp(_minionMeshTransform.rotation, rotation, Time.deltaTime * rotationSpeed);
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