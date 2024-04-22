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
        public int poolSize;
        
        [System.Serializable]
        public struct PrefabData
        {
            public string key;
            public GameObject prefab;
            [Range(1,2)]
            public int team;
        }
        
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
    }
}