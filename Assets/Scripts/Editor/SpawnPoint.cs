using UnityEngine;
using UnityEditor;
using Core;

namespace Editor
{
    /// <summary>
    /// A class for editor only
    /// </summary>
    public class SpawnWaypoint
    {
        /// <summary>
        /// Spawn the gameobj waypoint to the patrolpath gameobj shortcut ctrl+g
        /// </summary>
        [MenuItem("Tools/PatrolPath/Add Waypoint %g", false)]
        static void SpawnWaypointFromEditor()
        {
            if (Selection.activeGameObject.GetComponent<PatrolPath>() != null)
            {
                var selectObj = Selection.activeGameObject.GetComponent<PatrolPath>();
                new GameObject()
                {
                    name = $"Waypoint {selectObj.transform.childCount}",
                    transform =
                    {
                        position = selectObj.transform.position,
                        parent = selectObj.transform
                    }
                };
            }
            else if (Selection.activeGameObject.GetComponentInParent<PatrolPath>() != null)
            {
                var selectObjInParent = Selection.activeGameObject.GetComponentInParent<PatrolPath>();

                new GameObject()
                {
                    name = $"Waypoint {selectObjInParent.transform.childCount}",
                    transform =
                    {
                        position = selectObjInParent.transform.position,
                        parent = selectObjInParent.transform
                    }
                };
            }
        }

        /// <summary>
        /// Validate if the selected obj in editor contain PatrolPath script
        /// </summary>
        /// <returns>true or false</returns>
        [MenuItem("Tools/PatrolPath/Add Waypoint %g", true)]
        static bool SpawnWaypointValidation()
        {
            if (Selection.activeGameObject == null) return false;

            if (Selection.activeGameObject.GetComponent<PatrolPath>()
                || Selection.activeGameObject.GetComponentInParent<PatrolPath>()) return true;
            else return false;
        }
    }
}