using System;
using UnityEngine;

namespace Interfaces.Core
{
    /// <summary>
    /// Interface for managing the health of an object.
    /// </summary>
    public interface IHealth
    {
        /// <summary>
        /// Checks if the object is dead.
        /// </summary>
        /// <returns>True if the object is dead, otherwise false.</returns>
        public bool IsDead();
        /// <summary>
        /// Deals damage to the object.
        /// </summary>
        /// <param name="damage">The amount of damage to deal.</param>
        /// <param name="gameObjectTag">The tag of the object dealing the damage.</param>
        /// <param name="gameObject">The game object dealing the damage.</param>
        void DealDamage(float damage,ref string gameObjectTag, GameObject gameObject);
        /// <summary>
        /// Revives the object.
        /// </summary>
        /// <param name="gameObjectTag">The tag to assign to the revived object.</param>
        void Revive(ref string gameObjectTag);
        /// <summary>
        /// Event triggered when the object's health reaches zero.
        /// </summary>
        event Action<IHealth> OnDeathHandle; // Event to notify when the minion dies
    }
}