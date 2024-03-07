using System;
using Static;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Core
{
    /// <summary>
    /// A ckass that define a game object being a fighter
    /// </summary>
    public class Fighter : MonoBehaviour
    {
        /// <summary>
        /// A var that will set the enemy team tag.
        /// </summary>
        private string _enemyTeamTag;
        /// <summary>
        /// Time between attack used for animations
        /// </summary>
        [SerializeField] private float timeBetweenAttack = 0.5f;
        /// <summary>
        /// Time between attack used for animations
        /// </summary>
        [SerializeField] private int noOfAttacks = 3;
        /// <summary>
        /// Time since last attack calculate with Infinity and deltatime for animation
        /// </summary>
        private float _timer = 0f;
        private bool _hasAttacked = false;

        /// <summary>
        /// The enemy target
        /// </summary>
        private GameObject _target;
        private void Awake()
        {
            if (gameObject.CompareTag("Team1"))
            {
                _enemyTeamTag = "Team2";
            }else if (gameObject.CompareTag("Team2"))
            {
                _enemyTeamTag = "Team1";
            }
        }

        public void AttackBehavior(Animator animator, float attackInput)
        {
            if (!_hasAttacked && attackInput >= 1f)
            {
                _hasAttacked = true;
                _timer = 0f;
                animator.SetInteger(AnimatorParameters.AttackRandom,Random.Range(1, noOfAttacks+1));
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
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(_enemyTeamTag))
            {
                _target = other.gameObject;
            }
        }

        public GameObject GetEnemyTarget()
        {
            return _target;
        }
    }
}