using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace PoolManager
{
    /// <summary>
    /// Database for storing minion prefabs and their associated data.
    /// </summary>
    [CreateAssetMenu(fileName = "PoolManagerDatabase", menuName = "ObjectPool/Pool Manager Database")]
    public class PoolManagerDatabase : ScriptableObject
    {
        [System.Serializable]
        public struct PrefabData
        {
            public int poolSize;
            public string key;
            public GameObject prefab;
            [Range(1,2)]
            public int team;
        }
        
        [Header("Pools")]
        [FormerlySerializedAs("PrefabDatas")] [SerializeField]
        public List<PrefabData> prefabDatas = new List<PrefabData>();
        
        /// <summary>
        /// Retrieves the prefab associated with the specified key.
        /// </summary>
        /// <param name="key">The key associated with the desired prefab.</param>
        /// <returns>The GameObject prefab associated with the key.</returns>
        public GameObject GetPrefabByKey(string key)
        {
            return prefabDatas.Find(minionPrefabs => minionPrefabs.key == key).prefab;
        }
        
        /// <summary>
        /// Retrieves the poolsize associated with the specified key.
        /// </summary>
        /// <param name="key">The key associated with the desired pool size.</param>
        /// <returns>The pool size associated with the key.</returns>
        public int GetPoolSizeByKey(string key)
        {
            return prefabDatas.Find(minionPrefabs => minionPrefabs.key == key).poolSize;
        }
        
        public int GetGameObjectTeamByKey(string key)
        {
            return prefabDatas.Find(minionPrefabs => minionPrefabs.key == key).team;
        }
    }
}