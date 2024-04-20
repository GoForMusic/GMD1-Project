using Static;
using UnityEngine;

namespace Interfaces.Core
{
    /// <summary>
    /// Implementation of the IFighter interface for melee fighters.
    /// </summary>
    public class FighterMelee : IFighter
    {
        private string _enemyTeamTag;
        private float _dealDmg;
        private float _timeBetweenAttack;
        private int _noOfAttacks;
        private float _timer = 0f;
        private bool _hasAttacked = false;
        private GameObject _target;
        private float _weaponAttackRange;
        
        /// <summary>
        /// Constructor for creating a melee fighter.
        /// </summary>
        /// <param name="yourTag">The tag of the fighter's team.</param>
        /// <param name="dealDmg">The amount of damage the fighter deals.</param>
        /// <param name="timeBetweenAttack">The time between each attack.</param>
        /// <param name="noOfAttacks">The number of attacks the fighter has.</param>
        /// <param name="weaponAttackRange">The attack range of the fighter's weapon.</param>
        public FighterMelee(string yourTag,float dealDmg, float timeBetweenAttack, int noOfAttacks, float weaponAttackRange)
        {
            if (yourTag.Equals("Team1"))
            {
                _enemyTeamTag = "Team2";
            }else if (yourTag.Equals("Team2"))
            {
                _enemyTeamTag = "Team1";
            }
            _dealDmg = dealDmg;
            _timeBetweenAttack = timeBetweenAttack;
            _noOfAttacks = noOfAttacks;
            _weaponAttackRange = weaponAttackRange;
        }
        
        public void AttackBehavior(Animator animator, float attackInput)
        {
            if (!_hasAttacked && attackInput >= 1f)
            {
                _hasAttacked = true;
                _timer = 0f;
                animator.SetInteger(AnimatorParameters.AttackRandom,Random.Range(1, _noOfAttacks+1));
                animator.SetTrigger(AnimatorParameters.Attack);
            }
            
            if (_hasAttacked)
            {
                _timer += Time.deltaTime;
                if (_timer >= _timeBetweenAttack)
                {
                    _timer = 0;
                    _hasAttacked = false;
                }
            }
        }
        public void SetEnemyTarger(GameObject target)
        {
            _target=target;
        }

        public string GetEnemyTag()
        {
            return _enemyTeamTag;
        }

        public GameObject GetEnemyTarget()
        {
            return _target;
        }

        public void Hit(Vector3 position)
        {
            if (_target ==null) return;

            if (Vector3.Distance(position, _target.transform.position) < _weaponAttackRange)
            {
                IHealth enemyHealth = _target.GetComponent<IHealthProvider>().GetHealth();
                var newTag = _target.tag;
                enemyHealth.DealDamage(_dealDmg, ref newTag,_target);
                _target.gameObject.tag = newTag;
            }
        }
    }
}