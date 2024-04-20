using System.Collections;
using PoolManager;
using Static;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Interfaces.Stats
{
    public class HealthMinion : IHealth
    {
        
        private Slider _healthBar;
        private float _maxHealth;
        
        private float _currentHealth;
        private bool _isDead = false;
        private string _initialTag;

        private NavMeshAgent _navMeshAgent;
        private Collider _collider;
        private Animator _animator;


        private MonoBehaviour _coroutineRunner; // Reference to a MonoBehaviour for coroutine running

        //UNIQ for minion
        private MinionPoolManager _minionPoolManager;
        
        public HealthMinion(Slider healthBar, 
            float maxHealth,
            string gameObjectTag,
            NavMeshAgent navMeshAgent, 
            Collider collider,
            Animator animator,
            MinionPoolManager minionPoolManager,MonoBehaviour coroutineRunner)
        {
            _healthBar = healthBar;
            _currentHealth = maxHealth;
            _maxHealth = maxHealth;
            _initialTag = gameObjectTag;
            _navMeshAgent = navMeshAgent;
            _collider = collider;
            _animator = animator;
            _minionPoolManager = minionPoolManager;
            _coroutineRunner = coroutineRunner;
            
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

            _coroutineRunner.StartCoroutine(DieWithDelay(10.0f,gameObject)); 
        }
        
        private IEnumerator DieWithDelay(float delay, GameObject gameObject)
        {
            yield return new WaitForSeconds(delay);
            
            // Return the minion to the pool
            if (_minionPoolManager != null)
            {
                _minionPoolManager.ReturnMinionToPool(gameObject);
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