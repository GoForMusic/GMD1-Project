using Static;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    /// <summary>
    /// A ckass that define a game object being a fighter
    /// </summary>
    public class Fighter : MonoBehaviour
    {
        /// <summary>
        /// Time between attack used for animations
        /// </summary>
        [SerializeField] private float timeBetweenAttack = 0.5f;
        /// <summary>
        /// Time since last attack calculate with Infinity and deltatime for animation
        /// </summary>
        private float _timer = 0f;
        private bool _hasAttacked = false;
        public void AttackBehavior(Animator animator, float attackInput)
        {
            if (!_hasAttacked && attackInput >= 1f)
            {
                _hasAttacked = true;
                _timer = 0f;
                animator.SetInteger(AnimatorParameters.AttackRandom,Random.Range(1, 4));
                animator.SetTrigger(AnimatorParameters.Attack);
            }
            
            if (_hasAttacked)
            {
                _timer += Time.deltaTime;
                if (_timer >= timeBetweenAttack)
                {
                    _timer = 0;
                    _hasAttacked = false;
                }
            }
        }
    }
}