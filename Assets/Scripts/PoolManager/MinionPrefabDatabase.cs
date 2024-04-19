using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PoolManager
{
    [CreateAssetMenu(fileName = "MinionPrefabDatabase", menuName = "Minion Pool/Minion Prefab Database")]
    public class MinionPrefabDatabase : ScriptableObject
    {
        public int poolSize;
        
        [System.Serializable]
        public struct MinionPrefabData
        {
            public string key;
            public GameObject prefab;
            [Range(1,2)]
            public int team;
        }
        
        [SerializeField]
        public List<MinionPrefabData> minionPrefabs = new List<MinionPrefabData>();
        
        public GameObject GetPrefabByKey(string key)
        {
            return minionPrefabs.Find(minionPrefabs => minionPrefabs.key == key).prefab;
        }
    }
}