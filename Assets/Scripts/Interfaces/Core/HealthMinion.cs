using System;
using System.Collections;
using PoolManager;
using Static;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Interfaces.Core
{
    /// <summary>
    /// Class for managing the health of a minion character.
    /// </summary>
    public class HealthMinion : IHealth
    {
        public event Action<IHealth> OnDeathHandle; // Event to notify when the minion dies
        
        private Slider _healthBar;
        private float _maxHealth;
        
        private float _currentHealth;
        private bool _isDead = false;
        private string _initialTag;

        private NavMeshAgent _navMeshAgent;
        private Collider _collider;
        private Animator _animator;
        private float _reviveDelay;
        

        private MonoBehaviour _originMonoBehaviour; // Reference to a MonoBehaviour for coroutine running

        //UNIQ for minion
        private ObjectPoolManager _objectPoolManager;

        /// <summary>
        /// Constructor for initializing minion health.
        /// </summary>
        /// <param name="healthBar">The health bar UI slider.</param>
        /// <param name="maxHealth">The maximum health of the minion.</param>
        /// <param name="gameObjectTag">The initial tag of the minion's game object.</param>
        /// <param name="navMeshAgent">The NavMeshAgent component of the minion.</param>
        /// <param name="collider">The Collider component of the minion's game object.</param>
        /// <param name="animator">The Animator component of the minion's game object.</param>
        /// <param name="objectPoolManager">The MinionPoolManager for managing the minion's pool.</param>
        /// <param name="originMonoBehaviour">A MonoBehaviour used for running coroutines.</param>
        /// <param name="reviveDelay">Delay for revive coroutine</param>
        public HealthMinion(Slider healthBar, 
            float maxHealth,
            string gameObjectTag,
            NavMeshAgent navMeshAgent, 
            Collider collider,
            Animator animator,
            ObjectPoolManager objectPoolManager,
            MonoBehaviour originMonoBehaviour,
            float reviveDelay)
        {
            _healthBar = healthBar;
            _currentHealth = maxHealth;
            _maxHealth = maxHealth;
            _initialTag = gameObjectTag;
            _navMeshAgent = navMeshAgent;
            _collider = collider;
            _animator = animator;
            _objectPoolManager = objectPoolManager;
            _originMonoBehaviour = originMonoBehaviour;
            _reviveDelay = reviveDelay;
            
            UpdateHealthBar();
        }
        
        public bool IsDead()
        {
            return _isDead;
        }

        public void DealDamage(float damage,ref string gameObjectTag, GameObject gameObject)
        {
            _currentHealth = Mathf.Max(_currentHealth-damage,0);
            UpdateHealthBar();
            if (_currentHealth == 0)
            {
                Die(ref gameObjectTag,gameObject);
            }
        }

        public void Revive(ref string gameObjectTag)
        {
            if (_isDead)
            {
                _currentHealth = _maxHealth;
                UpdateHealthBar();
                _isDead = false;
                
                // Enable NavMeshAgent if it exists
                if (_navMeshAgent != null && !_navMeshAgent.enabled)
                {
                    _navMeshAgent.enabled = true;
                }
                
                // Enable the collider
                if(_collider != null)
                    _collider.enabled = true;
                
                // Reset the animator
                if (_animator != null)
                {
                    _animator.Rebind();
                }
                
                gameObjectTag = _initialTag; // Adjust tag as needed
            }
        }

        /// <summary>
        /// Kills the player character.
        /// </summary>
        /// <param name="gameObjectTag">The tag of the player character's game object.</param>
        /// <param name="gameObject">The player character's game object.</param>
        private void Die(ref string gameObjectTag, GameObject gameObject)
        {
            if(_isDead) return; 
            _isDead = true;
            
            // Disable NavMeshAgent if it exists
            if (_navMeshAgent != null)
            {
                _navMeshAgent.enabled = false;
            }
            
            // Disable the collider
            if(_collider != null)
                _collider.enabled = false;
            
            _animator.SetTrigger(AnimatorParameters.Die);
            gameObjectTag = "Untagged";

            // Trigger the minion death event
            OnDeathHandle?.Invoke(this);
            
            _originMonoBehaviour.StartCoroutine(DieWithDelay(gameObject)); 
        }
        
        /// <summary>
        /// Coroutine to return the minion to the pool after a delay.
        /// </summary>
        private IEnumerator DieWithDelay(GameObject gameObject)
        {
            yield return new WaitForSeconds(_reviveDelay);
            
            // Return the minion to the pool
            if (_objectPoolManager != null)
            {
                _objectPoolManager.ReturnObjectToPool(gameObject);
            }
        }
        
        /// <summary>
        /// A method that will update the UI element (Health BAR)
        /// </summary>
        void UpdateHealthBar()
        {
            float normalizedHealth = _currentHealth / _maxHealth;
            _healthBar.value = normalizedHealth;
        }
    }
}