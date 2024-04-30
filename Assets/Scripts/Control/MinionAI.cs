using Core;
using Interfaces.Control;
using Interfaces.Core;
using PoolManager;
using Static;
using Stats;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Control
{
    
    /// <summary>
    /// Handles the behavior of a minion character.
    /// </summary>
    public class MinionAI : MonoBehaviour, IHealthProvider
    {
        // Variables to set in the Inspector
        [Header("PatrolPath")]
        public PatrolPath patrolPath;
        
        [Header("Config file")]
        [SerializeField]
        private StatsConfig _statsConfig;
        
        [Header("Only used for range minions")]
        [SerializeField] 
        private Transform projectileSpawnPoint;
        
        [Header("MinionUI")]
        [SerializeField]
        private Slider healthBar;
        
        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        
        //Other Core Elements
        private IFighter _fighter;
        private IHealth _health;
        private IMinionBehavior _minionBehavior;
        private IMovement _movement;
        
        /// <summary>
        /// Initializes references and components.
        /// </summary>
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            // Determine which attack strategy to use based on the selected attack type
            switch (_statsConfig.attackType)
            {
                case AttackType.Melee:
                    _fighter = new FighterMelee(gameObject.tag,
                        _statsConfig.dealDmg,
                        _statsConfig.timeBetweenAttack,
                        _statsConfig.noOfAttacks,
                        _statsConfig.weaponRange);
                    break;
                case AttackType.Range:
                    _fighter = new FighterRange(gameObject.tag,
                        _statsConfig.dealDmg,
                        _statsConfig.timeBetweenAttack,
                        _statsConfig.noOfAttacks,
                        _statsConfig.weaponRange,
                        FindObjectOfType<ObjectPoolManager>(),
                        projectileSpawnPoint);
                    break;
                default:
                    Debug.LogError("Unknown attack type!");
                    break;
            }
            
            // Initialize health component with Minion-specific parameters
            _health = new HealthMinion(healthBar,
                _statsConfig.maxHealth,
                gameObject.tag,
                _navMeshAgent,
                GetComponent<Collider>(),
                _animator,
                FindObjectOfType<ObjectPoolManager>(),
                this,
                10f);
            
        }

        private void Start()
        {
            // Make sure there is a PatrolPath assigned
            if (patrolPath == null)
            {
                Debug.LogError("PatrolPath not assigned to MinionAI!");
                enabled = false; // Disable the script
                return;
            }
            
            // Set the minion's position to the first waypoint
            transform.position = patrolPath.GetWaypoints()[0];
            // Initialize other interfaces
            _movement = new Movement();
            _minionBehavior = new MinionBehavior(_statsConfig.followDistanceThreshold,_statsConfig.weaponRange);
        }

        /// <summary>
        /// Updates the minion's behavior based on its current state.
        /// </summary>
        private void Update()
        {
            if (_health.IsDead())
            {
                _minionBehavior.SetCurrentWaypointIndex(0);
                return;
            };

            if (_fighter.GetEnemyTarget() != null)
            {
                IHealth enemyHealth = _fighter.GetEnemyTarget().GetComponent<IHealthProvider>().GetHealth();
                if (enemyHealth != null && enemyHealth.IsDead())
                {
                    // Clear the target if it's dead
                    _fighter.SetEnemyTarger(null);
                }
            }
            
            if (!_minionBehavior.SawEnemy(_fighter.GetEnemyTarget()))
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
            Vector3? targetPosition = _minionBehavior.MoveToEnemy(_fighter.GetEnemyTarget(), transform.position);
            if (targetPosition != null)
            {
                RotateTowards(_fighter.GetEnemyTarget().transform.position);
                if (targetPosition != Vector3.zero)
                {
                    MoveTowards(targetPosition.Value);
                }
                else
                {
                    _navMeshAgent.isStopped = true;
                    _animator.SetFloat(AnimatorParameters.MovementSpeed, 0);
                    _fighter.AttackBehavior(_animator, 1f);
                }
            }
            else
            {
                _fighter.SetEnemyTarger(null);
            }
        }

        /// <summary>
        /// Rotates the minion towards the target position.
        /// </summary>
        /// <param name="targetPosition">The position to rotate towards.</param>
        private void RotateTowards(Vector3 targetPosition)
        {
            Quaternion rotation = _movement.Rotate(targetPosition - transform.position, transform);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _statsConfig.rotationSpeed * Time.deltaTime);
        }
        
        /// <summary>
        /// Moves the minion towards the target position.
        /// </summary>
        /// <param name="targetPosition">The position to move towards.</param>
        private void MoveTowards(Vector3 targetPosition)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(targetPosition);
            _animator.SetFloat(AnimatorParameters.MovementSpeed, _statsConfig.moveSpeed);
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
                // Set the enemy target first one is on the view
                if(_fighter.GetEnemyTarget() == null)
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