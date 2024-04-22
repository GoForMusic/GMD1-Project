using System.Collections.Generic;
using Interfaces.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace PoolManager
{
    /// <summary>
    /// Manages the pooling of minion game objects.
    /// </summary>
    public class ObjectPoolManager : MonoBehaviour
    {
        public PoolManagerDatabase poolManagerDatabase;
        
        private Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();
        private int _initialPoolSize;
        
        /// <summary>
        /// Initializes the minion pools on Awake.
        /// </summary>
        private void Awake()
        {
            if (poolManagerDatabase != null)
            {
                _initialPoolSize = poolManagerDatabase.poolSize;

                InitializePools(_initialPoolSize);
            }
            else
            {
                Debug.LogWarning("PoolManagerDatabase is not assigned.");
            }
        }
        
        /// <summary>
        /// Initializes the pools with the specified size for each minion type.
        /// </summary>
        /// <param name="poolSize">The initial size of each pool.</param>
        private void InitializePools(int poolSize)
        {
            foreach (var prefabData in poolManagerDatabase.prefabDatas)
            {
                string key = prefabData.key;
                GameObject prefab = prefabData.prefab;
                Queue<GameObject> pool = new Queue<GameObject>();

                // Create initial pool objects
                for (int i = 0; i < poolSize; i++)
                {
                    GameObject minion = Instantiate(prefab, transform.position, Quaternion.identity);
                    minion.SetActive(false);
                    pool.Enqueue(minion);
                }

                // Add pool to the dictionary
                _pools.Add(key, pool);
            }
        }
        
        /// <summary>
        /// Increases the size of the pool for the specified minion type.
        /// </summary>
        /// <param name="key">The key of the minion type.</param>
        public void IncreasePoolSize(string key)
        {
            if (!_pools.ContainsKey(key))
            {
                Debug.LogError($"Minion type '{key}' not found in minionPools.");
                return;
            }

            int newPoolSize = _initialPoolSize +1; // Double the pool size
            Queue<GameObject> pool = _pools[key];
            GameObject prefab = poolManagerDatabase.GetPrefabByKey(key);

            // Instantiate additional minions to meet the new pool size
            while (pool.Count < newPoolSize)
            {
                GameObject minion = Instantiate(prefab, transform.position, Quaternion.identity);
                minion.SetActive(false);
                pool.Enqueue(minion);
            }

            // Update the initial pool size
            _initialPoolSize = newPoolSize;
        }
        
        /// <summary>
        /// Retrieves a minion from the pool with the specified type, position, and rotation.
        /// </summary>
        /// <param name="minionType">The type of minion to retrieve.</param>
        /// <param name="position">The position to spawn the minion.</param>
        /// <param name="rotation">The rotation of the minion.</param>
        /// <returns>The spawned minion GameObject.</returns>
        public GameObject GetObjectFromPool(string key, Vector3 position, Quaternion rotation)
        {
            if (!_pools.ContainsKey(key))
            {
                Debug.LogError($"Key: '{key}' not found in pool manager database.");
                return null;
            }
            
            Queue<GameObject> pool = _pools[key];
            
            // Check if there are inactive minions available in the pool
            foreach (GameObject obj in pool)
            {
                if (!obj.activeSelf)
                {
                    obj.transform.position = position;
                    obj.transform.rotation = rotation;
                    var newGameObjectTag = obj.tag;
                    IHealthProvider healthProvider = obj.GetComponent<IHealthProvider>();
                    if (healthProvider != null)
                    {
                        healthProvider.GetHealth().Revive(ref newGameObjectTag);
                    }
                    obj.SetActive(true);
                    return obj;
                }
            }
            
            // If no inactive minions are available, instantiate a new one
            GameObject prefab = poolManagerDatabase.GetPrefabByKey(key);
            GameObject newMinion = Instantiate(prefab, position, rotation);
            pool.Enqueue(newMinion);

            return newMinion;
        }
        
        /// <summary>
        /// Returns a minion to the pool.
        /// </summary>
        /// <param name="minion">The minion GameObject to return to the pool.</param>
        public void ReturnObjectToPool(GameObject obj)
        {
            // Reset minion properties if needed
            obj.SetActive(false);
            // Determine the type of minion and return it to the corresponding pool
            foreach (var kvp in _pools)
            {
                if (kvp.Value.Contains(obj))
                {
                    break;
                }
            }
        }
        
        
    }
}