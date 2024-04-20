using Control;
using Core;
using UnityEngine;

namespace Interfaces.Control
{
    /// <summary>
    /// Interface for controlling minion behavior.
    /// </summary>
    public interface IMinionBehavior
    {
        /// <summary>
        /// Moves the minion towards the next waypoint on the specified patrol path.
        /// </summary>
        /// <param name="patrolPath">The patrol path to follow.</param>
        /// <param name="minionPosition">The current position of the minion.</param>
        /// <returns>The position of the next waypoint, or null if no waypoints are available.</returns>
        Vector3? MoveToWaypoint(PatrolPath patrolPath,Vector3 minionPosition);
        /// <summary>
        /// Moves the minion towards the enemy target.
        /// </summary>
        /// <param name="fighter">The fighter component of the minion.</param>
        /// <param name="minionPosition">The current position of the minion.</param>
        /// <returns>The position of the enemy target, or null if no target is available or within range.</returns>
        Vector3? MoveToEnemy(Fighter fighter,Vector3 minionPosition);
        /// <summary>
        /// Checks if the minion has detected an enemy.
        /// </summary>
        /// <param name="fighter">The fighter component of the minion.</param>
        /// <returns>True if an enemy is detected, otherwise false.</returns>
        bool SawEnemy(Fighter fighter);

        /// <summary>
        /// Set the new current waypoint index, used when the minion is resurected.
        /// </summary>
        /// <param name="newWaypoint"></param>
        void SetCurrentWaypointIndex(int newWaypoint);
    }
}