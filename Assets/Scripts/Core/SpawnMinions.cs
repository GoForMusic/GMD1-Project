using System.Collections;
using System.Collections.Generic;
using Control;
using PoolManager;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Manages spawning of minions at defined intervals and settings.
    /// </summary>
    public class SpawnMinions : MonoBehaviour
    {
        [Header("Spawn Configuration")] [SerializeField]
        private List<string> _spawnConfigurations;

        [Header("Spawner Properties")] [SerializeField]
        private float _spawnInterval = 30.0f;

        [SerializeField] private float _delayBetweenMinions = 1f;

        [SerializeField] private PatrolPath _patrolPath;

        // Reference to the minion pool manager
        private ObjectPoolManager _objectPoolManager;

        /// <summary>
        /// Initializes the spawner and starts the minion spawning process.
        /// </summary>
        private void Start()
        {
            _patrolPath = GetComponent<PatrolPath>();
            if (_patrolPath == null)
            {
                Debug.LogError("PatrolPath component not found on the spawner object!");
            }
            else
            {
                _objectPoolManager = FindObjectOfType<ObjectPoolManager>();

                StartCoroutine(SpawnMinionsAsync());
            }
        }

        /// <summary>
        /// Asynchronously spawns minions based on the configured properties.
        /// </summary>
        private IEnumerator SpawnMinionsAsync()
        {
            while (true)
            {
                foreach (var key in _spawnConfigurations)
                {
                    Vector3 spawnLocation = _patrolPath.GetWaypoints()[0];
                    GameObject minion = _objectPoolManager.GetObjectFromPool(key, spawnLocation, transform.rotation);

                    // Check if the pool size needs to be increased
                    if (minion == null)
                    {
                        _objectPoolManager.IncreasePoolSize(key);
                        minion = _objectPoolManager.GetObjectFromPool(key, spawnLocation, transform.rotation);
                    }

                    if (minion != null)
                    {
                        SpawnRegularMinions(minion);
                    }

                    yield return new WaitForSeconds(_delayBetweenMinions);
                }

                yield return new WaitForSeconds(_spawnInterval - _delayBetweenMinions);
            }
        }

        /// <summary>
        /// Activates a minion and sets its patrol path.
        /// </summary>
        /// <param name="minion">The minion GameObject to be activated.</param>
        private void SpawnRegularMinions(GameObject minion)
        {
            minion.SetActive(true);
            minion.GetComponentInChildren<MinionAI>().patrolPath = _patrolPath;
        }
    }
}
