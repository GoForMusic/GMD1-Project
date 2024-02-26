using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core
{
    /// <summary>
    /// Health class object
    /// </summary>
    public class Health : MonoBehaviour
    {
        /// <summary>
        /// parameter of health points
        /// </summary>
        public float maxHealth = 100f;
        private float _currentHealth;
        [SerializeField]
        private Slider healthBar;
        
        /// <summary>
        /// if gameobj is dead
        /// </summary>
        private bool _isDead = false;

        /// <summary>
        /// A method that will return if the player or AI is dead
        /// </summary>
        /// <returns>true or false</returns>
        public bool IsDead()
        {
            return _isDead;
        }
        
        private void Start()
        {
            _currentHealth = maxHealth;
            UpdateHealthBar();
        }
        
        /// <summary>
        /// Take damage, if health == 0 , than die
        /// </summary>
        /// <param name="damage">damage amount you receive</param>
        public void TakeDamage(float damage)
        {
            _currentHealth = Mathf.Max(_currentHealth-damage,0);
            UpdateHealthBar();
            if (_currentHealth == 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Die animation
        /// </summary>
        private void Die()
        {
            if(_isDead) return;
            _isDead = true;
            GetComponent<Animator>().SetTrigger("die");
        }
        
        /// <summary>
        /// A method that will update the UI element (Health BAR)
        /// </summary>
        void UpdateHealthBar()
        {
            float normalizedHealth = _currentHealth / maxHealth;
            healthBar.value = normalizedHealth;
        }
    }
}
