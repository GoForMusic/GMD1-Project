using Static;
using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "StatsConfig", menuName = "Stats/Stats Configuration")]
    public class StatsConfig : ScriptableObject
    {
        [Header("Minion Stats")]
        public float followDistanceThreshold = 7f; // Follow distance threshold for minions
        
        [Header("Common Stats")]
        public float maxHealth = 100f;
        public float dealDmg = 50f;
        public float timeBetweenAttack = 0.5f;
        public int noOfAttacks = 3;

        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float rotationSpeed = 5f;

        [Header("Combat Settings")]
        public float weaponRange = 2f;
        public AttackType attackType;
    }
}