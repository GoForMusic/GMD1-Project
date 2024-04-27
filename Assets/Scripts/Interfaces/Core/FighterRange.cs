using Gameplay;
using PoolManager;
using Static;
using UnityEngine;

namespace Interfaces.Core
{
    /// <summary>
    /// Implementation of the IFighter interface for range fighters.
    /// </summary>
    public class FighterRange : IFighter
    {
        private string _enemyTeamTag;
        private float _dealDmg;
        private float _timeBetweenAttack;
        private int _noOfAttacks;
        private float _timer = 0f;
        private bool _hasAttacked = false;
        private GameObject _target;
        private float _weaponAttackRange;
        private ObjectPoolManager _poolManager; // Reference to the object pool manager
        private Transform _projectileSpawnPoint; // Reference to the projectile spawn point
        
        /// <summary>
        /// Constructor for creating a range fighter.
        /// </summary>
        /// <param name="yourTag">The tag of the fighter's team.</param>
        /// <param name="dealDmg">The amount of damage the fighter deals.</param>
        /// <param name="timeBetweenAttack">The time between each attack.</param>
        /// <param name="noOfAttacks">The number of attacks the fighter has.</param>
        /// <param name="weaponAttackRange">The attack range of the fighter's weapon.</param>
        /// <param name="poolManager">Pool manager reference, used to spawn obj from the pool</param>
        /// <param name="projectileSpawnPoint">Projectile from where to spawn</param>
        public FighterRange(string yourTag,
            float dealDmg, 
            float timeBetweenAttack, 
            int noOfAttacks,
            float weaponAttackRange,
            ObjectPoolManager poolManager,
            Transform projectileSpawnPoint)
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
            _poolManager = poolManager;
            _projectileSpawnPoint = projectileSpawnPoint;
            
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
                // Determine the projectile key based on the target's tag
                string projectileKey = "";
                if (_target.tag.Equals("Team1"))
                {
                    projectileKey = "MinionP2-Range-Projectile";
                }
                else if (_target.tag.Equals("Team2"))
                {
                    projectileKey = "MinionP1-Range-Projectile";
                }

                if(projectileKey.Equals("")) return;
                
                // Initialize and activate a projectile from the object pool
                GameObject projectile = _poolManager.GetObjectFromPool(projectileKey, _projectileSpawnPoint.position, Quaternion.identity);
                if (projectile != null)
                { 
                    // Initialize the projectile with the shooter's tag and damage
                    projectile.GetComponent<Projectile>().Initialize(_poolManager,_target,_dealDmg);
                }
            }
        }
    }
}