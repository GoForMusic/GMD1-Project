using System.Collections;
using Static;
using UnityEngine;
using UnityEngine.UI;

namespace Interfaces.Core
{
    /// <summary>
    /// Class for managing the health of a player character.
    /// </summary>
    public class HealthPlayer : IHealth
    {
        private Slider _healthBar;
        private float _maxHealth;
        
        private float _currentHealth;
        private bool _isDead = false;
        private string _initialTag;
        
        private Collider _collider;
        private Animator _animator;
        
        /// <summary>
        /// Constructor for initializing player health.
        /// </summary>
        /// <param name="healthBar">The slider UI element representing the health bar.</param>
        /// <param name="maxHealth">The maximum health value.</param>
        /// <param name="gameObjectTag">The tag of the player character's game object.</param>
        /// <param name="collider">The collider component of the player character.</param>
        /// <param name="animator">The animator component of the player character.</param>
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
        
        /// <summary>
        /// Kills the player character.
        /// </summary>
        /// <param name="gameObjectTag">The tag of the player character's game object.</param>
        /// <param name="gameObject">The player character's game object.</param>
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