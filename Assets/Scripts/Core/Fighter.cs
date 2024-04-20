using System;
using System.Collections.Generic;
using Interfaces.Core;
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
        /// Deal dmg to enemy var
        /// </summary>
        [SerializeField]private float dealDmg = 0.1f;
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
        /// <summary>
        /// Set weapon attack range from Minion or player
        /// </summary>
        private float _weaponAttackRange;
        
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
        
        void Update()
        {
            if (_target != null)
            {
                IHealth enemyHealth = _target.GetComponent<IHealth>();
                if (enemyHealth != null && enemyHealth.IsDead())
                {
                    // Clear the target if it's dead
                    _target = null;
                }
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

        /// <summary>
        /// Hit event when hit with the animation
        /// </summary>
        void Hit()
        {
            if (_target ==null) return;


            if (Vector3.Distance(transform.position, _target.transform.position) < 2.0f)
            {
                IHealth enemyHealth = _target.GetComponent<IHealthProvider>().GetHealth();
                var newTag = gameObject.tag;
                enemyHealth.DealDamage(dealDmg, ref newTag,gameObject);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag(_enemyTeamTag))
            {
                _target=other.gameObject;
            }
        }
        
        public GameObject GetEnemyTarget()
        {
            return _target;
        }

        public void SetWeaponRange(float weaponAttackRange)
        {
            this._weaponAttackRange = weaponAttackRange;
        }
    }
}