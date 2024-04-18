using Control;
using Core;
using UnityEngine;

namespace Interfaces.Minion
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

        public Vector3? MoveToEnemy(Fighter fighter, Vector3 minionPosition) 
        {
            if (fighter.GetEnemyTarget() == null)
            {
                // No enemy detected
                return null;
            }

            float distanceToEnemy = Vector3.Distance(minionPosition, fighter.GetEnemyTarget().transform.position);

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
            Vector3 targetPosition = fighter.GetEnemyTarget().transform.position;
            return targetPosition;
        }

        public bool SawEnemy(Fighter fighter)
        {
            if (fighter.GetEnemyTarget() != null) return true;
            else return false;
        }
    }
}