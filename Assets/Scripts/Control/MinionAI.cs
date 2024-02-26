using System;
using Core;
using UnityEngine;

namespace Control
{
    [RequireComponent(typeof(Health))]
    public class MinionAI : MonoBehaviour
    {
        public PatrolPath patrolPath;
        public float moveSpeed = 5f;
        
        private int currentWaypointIndex = 0;
        
        void Start()
        {
            // Make sure there is a PatrolPath assigned
            if (patrolPath == null)
            {
                Debug.LogError("PatrolPath not assigned to MinionAI!");
                enabled = false; // Disable the script
                return;
            }

            // Set the minion's position to the first waypoint
            transform.position = patrolPath.GetWaypoints()[0];
        }
        
        void Update()
        {
            // Move towards the current waypoint
            Vector3 targetPosition = patrolPath.GetWaypoints()[currentWaypointIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // If the minion reaches the current waypoint, move to the next one
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= patrolPath.GetWaypoints().Length)
                {
                    currentWaypointIndex = 0; // Loop back to the beginning
                }
            }
        }
    }
}