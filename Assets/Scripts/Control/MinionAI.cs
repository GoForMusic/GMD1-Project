using Core;
using Interfaces.Core;
using Interfaces.Minion;
using Static;
using UnityEngine;

namespace Control
{
    
    /// <summary>
    /// Handles the behavior of a minion character.
    /// </summary>
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Fighter))]
    public class MinionAI : MonoBehaviour
    {
        // Variables to set in the Inspector
        [Header("PatrolPath")]
        public PatrolPath patrolPath;
        [Header("Minion Stats")]
        public float moveSpeed = 5f;
        public float rotationSpeed = 5f;
        public float weaponRange = 2f;
        [Header("Player")]
        public float followDistanceThreshold = 5f;
        
        private Animator _animator;
        //Other Core Elements
        private Fighter _fighter;
        private Health _health;
        
        
        //Interface
        private IMinionBehavior _minionBehavior;
        private IMovement _movement;
        
        /// <summary>
        /// Initializes references and components.
        /// </summary>
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
            
            _minionBehavior = new MinionBehavior(followDistanceThreshold,weaponRange);
            _movement = new Movement();
        }
        
        /// <summary>
        /// Updates the minion's behavior based on its current state.
        /// </summary>
        void Update()
        {
            if (_health.IsDead()) return;

            if (!_minionBehavior.SawEnemy(_fighter))
            {
                MoveToWaypoint();
            }
            else
            {
                MoveToEnemy();
            }
        }

        /// <summary>
        /// Moves the minion towards the next waypoint.
        /// </summary>
        private void MoveToWaypoint()
        {
            var targetPosition = _minionBehavior.MoveToWaypoint(patrolPath, transform.position);
            if (targetPosition != null)
            {
                RotateTowards(targetPosition.Value);
                MoveTowards(targetPosition.Value);
            }
        }

        /// <summary>
        /// Moves the minion towards the enemy if detected.
        /// </summary>
        private void MoveToEnemy()
        {
            Vector3? targetPosition = _minionBehavior.MoveToEnemy(_fighter, transform.position);
            if (targetPosition != null)
            {
                RotateTowards(_fighter.GetEnemyTarget().transform.position);
                if (targetPosition != Vector3.zero)
                {
                    MoveTowards(targetPosition.Value);
                }
                else
                {
                    _animator.SetFloat(AnimatorParameters.MovementSpeed, 0);
                    _fighter.AttackBehavior(_animator, 1f);
                }
            }
        }

        /// <summary>
        /// Rotates the minion towards the target position.
        /// </summary>
        /// <param name="targetPosition">The position to rotate towards.</param>
        private void RotateTowards(Vector3 targetPosition)
        {
            Quaternion rotation = _movement.Rotate(targetPosition - transform.position, transform);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
        
        /// <summary>
        /// Moves the minion towards the target position.
        /// </summary>
        /// <param name="targetPosition">The position to move towards.</param>
        private void MoveTowards(Vector3 targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            _animator.SetFloat(AnimatorParameters.MovementSpeed, moveSpeed);
        }
        
    }
}