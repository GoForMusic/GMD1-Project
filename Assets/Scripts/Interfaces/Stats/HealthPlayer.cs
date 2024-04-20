using System.Collections;
using Static;
using UnityEngine;
using UnityEngine.UI;

namespace Interfaces.Stats
{
    public class HealthPlayer : IHealth
    {
        private Slider _healthBar;
        private float _maxHealth;
        
        private float _currentHealth;
        private bool _isDead = false;
        private string _initialTag;
        
        private Collider _collider;
        private Animator _animator;
        
        public HealthPlayer(Slider healthBar, 
            float maxHealth,
            string gameObjectTag,
            Collider collider,
            Animator animator)
        {
            _healthBar = healthBar;
            _currentHealth = maxHealth;
            _maxHealth = maxHealth;
            _initialTag = gameObjectTag;
            _collider = collider;
            _animator = animator;
            
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
            
            // Disable the collider
            if(_collider != null)
                _collider.enabled = false;
            
            _animator.SetTrigger(AnimatorParameters.Die);
            gameObjectTag = "Untagged";
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