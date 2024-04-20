using Control;
using UnityEngine;

namespace Interfaces.Control
{
    public class MinionBehavior : IMinionBehavior
    {
        private int _currentWaypointIndex = 0;
        private float _followDistanceThreshold;
        private float _weaponRange;

        public MinionBehavior(float followDistanceThreshold, float weaponRange)
        {
            _followDistanceThreshold = followDistanceThreshold;
            _weaponRange = weaponRange;
        }

        public void SetCurrentWaypointIndex(int newWaypoint)
        {
            _currentWaypointIndex = newWaypoint;
        }
        
        public Vector3? MoveToWaypoint(PatrolPath patrolPath,Vector3 minionPosition)
        {
            if (_currentWaypointIndex >= patrolPath.GetWaypoints().Length) return null;
            Vector3 targetPosition = patrolPath.GetWaypoints()[_currentWaypointIndex];
            
            if (Vector3.Distance(minionPosition, targetPosition) < 0.1f)
            {
                _currentWaypointIndex++;
            }

            return targetPosition;
        }

        public Vector3? MoveToEnemy(GameObject enemyTarget, Vector3 minionPosition) 
        {
            if (enemyTarget == null)
            {
                // No enemy detected
                return null;
            }

            float distanceToEnemy = Vector3.Distance(minionPosition, enemyTarget.transform.position);

            if (distanceToEnemy > _followDistanceThreshold)
            {
                // If the enemy is too far, return null to move to waypoint
                return null;
            }

            if (distanceToEnemy < _weaponRange)
            {
                // Within weapon range, return null to stay still
                return Vector3.zero; // This indicates staying still
            }

            // Move towards the enemy position
            Vector3 targetPosition = enemyTarget.transform.position;
            return targetPosition;
        }

        public bool SawEnemy(GameObject enemyTarget)
        {
            if (enemyTarget != null) return true;
            else return false;
        }
    }
}