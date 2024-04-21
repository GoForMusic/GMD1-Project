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
        private CharacterController _controller;
        private float _reviveDelay;
        private MonoBehaviour _originMonoBehaviour; // Reference to a MonoBehaviour for coroutine running
        private Vector3 _respawnPosition;


        /// <summary>
        /// Constructor for initializing player health.
        /// </summary>
        /// <param name="healthBar">The slider UI element representing the health bar.</param>
        /// <param name="maxHealth">The maximum health value.</param>
        /// <param name="gameObjectTag">The tag of the player character's game object.</param>
        /// <param name="collider">The collider component of the player character.</param>
        /// <param name="animator">The animator component of the player character.</param>
        /// <param name="controller">The animator component of the player controller</param>
        /// <param name="originMonoBehaviour">A MonoBehaviour used for running coroutines.</param>
        /// <param name="reviveDelay">Delay for revive coroutine</param>
        /// <param name="respawnPosition">S</param>
        public HealthPlayer(Slider healthBar, 
            float maxHealth,
            string gameObjectTag,
            Collider collider,
            Animator animator,
            CharacterController controller,
            MonoBehaviour originMonoBehaviour,
            float reviveDelay,
            Vector3 respawnPosition)
        {
            _healthBar = healthBar;
            _currentHealth = maxHealth;
            _maxHealth = maxHealth;
            _initialTag = gameObjectTag;
            _collider = collider;
            _animator = animator;
            _controller = controller;
            _originMonoBehaviour = originMonoBehaviour;
            _reviveDelay = reviveDelay;
            _respawnPosition = respawnPosition;
            
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
                Die(ref gameObjectTag);
            }
        }

        public void Revive(ref string gameObjectTag)
        {
            if (_isDead)
            {
                _originMonoBehaviour.StartCoroutine(ReviveWithDelay());
                gameObjectTag = _initialTag;
            }
        }
        
        /// <summary>
        /// Coroutine to respawn the player back to origin.
        /// </summary>
        private IEnumerator ReviveWithDelay()
        {
            yield return new WaitForSeconds(_reviveDelay);
            
            _currentHealth = _maxHealth;
            UpdateHealthBar();
            
            // Reset position
            if (_collider != null)
            {
                _collider.enabled = true;
            }
            
            _currentHealth = _maxHealth;
            UpdateHealthBar();
            
            
            // Reset the animator
            if (_animator != null)
            {
                _animator.Rebind();
            }
            _isDead = false;

            if (_controller != null)
                _controller.enabled = false; // Disable the controller momentarily to set position
                _originMonoBehaviour.transform.position = _respawnPosition;
                _controller.enabled = true; // Re-enable the controller
        }
        
        /// <summary>
        /// Kills the player character.
        /// </summary>
        /// <param name="gameObjectTag">The tag of the player character's game object.</param>
        private void Die(ref string gameObjectTag)
        {
            if(_isDead) return; 
            _isDead = true;
            
            // Disable the collider
            if(_collider != null)
                _collider.enabled = false;
            
            _animator.SetTrigger(AnimatorParameters.Die);
            gameObjectTag = "Untagged";
            
            Revive(ref gameObjectTag);
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