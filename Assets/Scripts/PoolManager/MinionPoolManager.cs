using System.Collections.Generic;
using UnityEngine;

namespace PoolManager
{
    public class MinionPoolManager : MonoBehaviour
    {
        public MinionPrefabDatabase minionPrefabDatabase;
        
        private Dictionary<string, Queue<GameObject>> _minionPools;
        private int _initialPoolSize;
        
        private void Start()
        {
            if (minionPrefabDatabase != null)
            {
                _minionPools = new Dictionary<string, Queue<GameObject>>();
                _initialPoolSize = minionPrefabDatabase.poolSize;

                InitializePools(_initialPoolSize);
            }
            else
            {
                Debug.LogWarning("MinionPrefabDatabase is not assigned to the MinionPoolManager.");
            }
        }
        
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
        
        
        public void IncreasePoolSize(string key)
        {
            if (!_minionPools.ContainsKey(key))
            {
                Debug.LogError($"Minion type '{key}' not found in minionPools.");
                return;
            }

            int newPoolSize = _initialPoolSize * 2; // Double the pool size
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
        
        public GameObject GetMinionFromPool(string minionType, Vector3 position, Quaternion rotation)
        {
            if (!_minionPools.ContainsKey(minionType))
            {
                Debug.LogError($"Minion type '{minionType}' not found in minionPrefabs.");
                return null;
            }

            Queue<GameObject> pool = _minionPools[minionType];

            if (pool.Count == 0)
            {
                Debug.LogWarning($"No more minions available in the {minionType} pool.");
                return null;
            }

            // Dequeue a minion from the pool
            GameObject minion = pool.Dequeue();
            minion.transform.position = position;
            minion.transform.rotation = rotation;
            minion.SetActive(true);
            
            return minion;
        }
        
        public void ReturnMinionToPool(GameObject minion)
        {
            // Reset minion properties if needed
            minion.SetActive(false);
            // Determine the type of minion and return it to the corresponding pool
            foreach (var kvp in _minionPools)
            {
                if (kvp.Value.Contains(minion))
                {
                    kvp.Value.Enqueue(minion);
                    break;
                }
            }
        }
        
        
    }
}