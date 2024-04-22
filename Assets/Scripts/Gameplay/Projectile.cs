using System;
using Interfaces.Core;
using PoolManager;
using UnityEngine;

namespace Gameplay
{
    public class Projectile : MonoBehaviour
    {
        private ObjectPoolManager _poolManager; // Reference to the pool manager
        private GameObject _target;
        private float _dealDmg;
        
        public void Initialize(ObjectPoolManager poolManager,GameObject target, float dealDmg)
        {
            _poolManager = poolManager;
            _target= target;
            _dealDmg = dealDmg;
        }

        private void Update()
        {
            // Calculate the direction to the target
            Vector3 directionToTarget = (_target.transform.position - transform.position).normalized;
            // Move the projectile in the direction of the target
            transform.Translate(directionToTarget * 20f * Time.deltaTime);

            // Check if the projectile is close enough to the target
            float distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);
            float threshold = 0.1f; // Adjust this value based on your needs
            if (distanceToTarget <= threshold)
            {
                _poolManager.ReturnObjectToPool(gameObject);
                // Apply damage to the target (if needed)
                IHealth enemyHealth = _target.GetComponent<IHealthProvider>().GetHealth();
                var newTag = _target.tag;
                enemyHealth.DealDamage(_dealDmg, ref newTag, _target);
                _target.gameObject.tag = newTag;
            }
        }
    }
}