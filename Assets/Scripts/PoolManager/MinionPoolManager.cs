using System;
using System.Collections.Generic;
using Control;
using Core;
using Interfaces.Core;
using UnityEngine;

namespace PoolManager
{
    /// <summary>
    /// Manages the pooling of minion game objects.
    /// </summary>
    public class MinionPoolManager : MonoBehaviour
    {
        public MinionPrefabDatabase minionPrefabDatabase;
        
        private Dictionary<string, Queue<GameObject>> _minionPools = new Dictionary<string, Queue<GameObject>>();
        private int _initialPoolSize;
        
        /// <summary>
        /// Initializes the minion pools on Awake.
        /// </summary>
        private void Awake()
        {
            if (minionPrefabDatabase != null)
            {
                _initialPoolSize = minionPrefabDatabase.poolSize;

                InitializePools(_initialPoolSize);
            }
            else
            {
                Debug.LogWarning("MinionPrefabDatabase is not assigned to the MinionPoolManager.");
            }
        }
        
        /// <summary>
        /// Initializes the pools with the specified size for each minion type.
        /// </summary>
        /// <param name="poolSize">The initial size of each pool.</param>
        private void InitializePools(int poolSize)
        {
            foreach (var minionData in minionPrefabDatabase.minionPrefabs)
            {
                string key = minionData.key;
                GameObject prefab = minionData.prefab;
                Queue<GameObject> pool = new Queue<GameObject>();

                // Create initial pool objects
                for (int i = 0; i < poolSize; i++)
                {
                    GameObject minion = Instantiate(prefab, transform.position, Quaternion.identity);
                    minion.SetActive(false);
                    pool.Enqueue(minion);
                }

                // Add pool to the dictionary
                _minionPools.Add(key, pool);
            }
        }
        
        /// <summary>
        /// Increases the size of the pool for the specified minion type.
        /// </summary>
        /// <param name="key">The key of the minion type.</param>
        public void IncreasePoolSize(string key)
        {
            if (!_minionPools.ContainsKey(key))
            {
                Debug.LogError($"Minion type '{key}' not found in minionPools.");
                return;
            }

            int newPoolSize = _initialPoolSize +1; // Double the pool size
            Queue<GameObject> pool = _minionPools[key];
            GameObject prefab = minionPrefabDatabase.GetPrefabByKey(key);

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
        public GameObject GetMinionFromPool(string minionType, Vector3 position, Quaternion rotation)
        {
            
            if (!_minionPools.ContainsKey(minionType))
            {
                Debug.LogError($"Minion type '{minionType}' not found in minionPrefabs.");
                return null;
            }
            
            Queue<GameObject> pool = _minionPools[minionType];
            
            // Check if there are inactive minions available in the pool
            foreach (GameObject minion in pool)
            {
                if (!minion.activeSelf)
                {
                    minion.transform.position = position;
                    minion.transform.rotation = rotation;
                    var newGameObjectTag = minion.tag;
                    minion.GetComponent<IHealthProvider>().GetHealth().Revive(ref newGameObjectTag);
                    minion.SetActive(true);
                    return minion;
                }
            }
            
            // If no inactive minions are available, instantiate a new one
            GameObject prefab = minionPrefabDatabase.GetPrefabByKey(minionType);
            GameObject newMinion = Instantiate(prefab, position, rotation);
            pool.Enqueue(newMinion);

            return newMinion;
        }
        
        /// <summary>
        /// Returns a minion to the pool.
        /// </summary>
        /// <param name="minion">The minion GameObject to return to the pool.</param>
        public void ReturnMinionToPool(GameObject minion)
        {
            // Reset minion properties if needed
            minion.SetActive(false);
            // Determine the type of minion and return it to the corresponding pool
            foreach (var kvp in _minionPools)
            {
                if (kvp.Value.Contains(minion))
                {
                    break;
                }
            }
        }
        
        
    }
}