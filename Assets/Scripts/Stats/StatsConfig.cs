using Static;
using UnityEngine;

namespace Stats
{
    /// <summary>
    /// Configuration settings for various stat-related properties of players, minions, and general combat.
    /// </summary>
    [CreateAssetMenu(fileName = "StatsConfig", menuName = "Stats/Stats Configuration")]
    public class StatsConfig : ScriptableObject
    {
        /// <summary>
        /// The multiplier applied to player stats upon leveling up.
        /// </summary>
        [Header("Player Stats")]
        public float levelUpMultiplier = 2f;
        /// <summary>
        /// The starting level for players.
        /// </summary>
        public int startLevel = 1;
        /// <summary>
        /// The maximum level players can achieve.
        /// </summary>
        public int maxLevel = 15;
        
        /// <summary>
        /// The distance threshold at which minions will start following a target.
        /// </summary>
        [Header("Minion Stats")]
        public float followDistanceThreshold = 7f; // Follow distance threshold for minions
        
        /// <summary>
        /// The maximum health points for entities.
        /// </summary>
        [Header("Common Stats")]
        public float maxHealth = 100f;
        /// <summary>
        /// The damage dealt per attack by entities.
        /// </summary>
        public float dealDmg = 50f;
        /// <summary>
        /// The time interval between consecutive attacks.
        /// </summary>
        public float timeBetweenAttack = 0.5f;
        /// <summary>
        /// The number of consecutive attacks an entity can perform.
        /// </summary>
        public int noOfAttacks = 3;

        /// <summary>
        /// The movement speed of entities.
        /// </summary>
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        /// <summary>
        /// The rotation speed of entities during movement or combat.
        /// </summary>
        public float rotationSpeed = 5f;

        /// <summary>
        /// The range within which an entity can engage in combat.
        /// </summary>
        [Header("Combat Settings")]
        public float weaponRange = 2f;
        /// <summary>
        /// The type of attack that an entity is equipped to perform.
        /// </summary>
        public AttackType attackType;
    }
}