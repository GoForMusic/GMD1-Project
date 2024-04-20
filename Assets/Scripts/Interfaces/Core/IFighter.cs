using UnityEngine;

namespace Interfaces.Core
{
    /// <summary>
    /// Interface for defining the behavior of a fighter.
    /// </summary>
    public interface IFighter
    {
        /// <summary>
        /// Defines the behavior of the fighter's attack.
        /// </summary>
        /// <param name="animator">The animator component used for animations.</param>
        /// <param name="attackInput">The input value representing the animation of attack.</param>
        void AttackBehavior(Animator animator, float attackInput);
        /// <summary>
        /// Sets the enemy target for the fighter.
        /// </summary>
        /// <param name="target">The GameObject representing the enemy target.</param>
        void SetEnemyTarger(GameObject target);
        /// <summary>
        /// Gets the tag of the enemy target.
        /// </summary>
        /// <returns>The tag of the enemy target.</returns>
        string GetEnemyTag();
        /// <summary>
        /// Gets the GameObject representing the enemy target.
        /// </summary>
        /// <returns>The enemy target GameObject.</returns>
        GameObject GetEnemyTarget();
        /// <summary>
        /// Performs a hit action at the specified position.
        /// </summary>
        /// <param name="position">The position where the hit action occurs.</param>
        void Hit(Vector3 position);
    }
}