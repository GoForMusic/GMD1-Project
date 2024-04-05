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

        [Header("Player")]
        public float followDistanceThreshold = 5f;
        
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
            float distanceToEnemy = Vector3.Distance(_minionMeshTransform.position, _fighter.GetEnemyTarget().transform.position);

            if (distanceToEnemy > followDistanceThreshold)
            {
                MoveToWaypoint(); // If the enemy is too far, move to the waypoint
                return;
            }
            
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