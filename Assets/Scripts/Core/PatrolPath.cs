using UnityEngine;

namespace Core
{
    /// <summary>
    /// A class that is creating a patrol path on the editor
    /// </summary>
    public class PatrolPath : MonoBehaviour
    {
        private const float waypointGizmoRadius = 0.3f;
        [SerializeField] 
        [Range(1,2)]
        private int team=1;
        
        /// <summary>
        /// Drow on engine the waypoints and the path between
        /// </summary>
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (i == 0)
                {
                    Gizmos.DrawIcon(GetCurrentWayPointLocation(i),"sv_icon_dot9_pix16_gizmo");
                }
                else if(i==transform.childCount-1)
                {
                    Gizmos.DrawIcon(GetCurrentWayPointLocation(i),"sv_icon_dot14_pix16_gizmo");
                }
                else
                {
                    Gizmos.DrawIcon(GetCurrentWayPointLocation(i),"sv_icon_dot8_pix16_gizmo");
                }
                
                Gizmos.color = team == 1 ? Color.blue: Color.red;
                Gizmos.DrawLine(GetCurrentWayPointLocation(i),GetCurrentWayPointLocation(GetNextIndex(i)));
            }
        }

        /// <summary>
        /// Get the next Index from the list of waypoints
        /// </summary>
        /// <param name="i">The current index</param>
        /// <returns>0 if the next position is out of list or the next index</returns>
        public int GetNextIndex(int i)
        {
            if (i + 1 == transform.childCount)
            {
                return 0;
            }
            
            return i + 1;
        }
        
        /// <summary>
        /// Get current waypoint location on the map
        /// </summary>
        /// <param name="i">The current index</param>
        /// <returns>Vector3 position</returns>
        public Vector3 GetCurrentWayPointLocation(int i)
        {
            return transform.GetChild(i).transform.position;
        }
        
        public Vector3[] GetWaypoints()
        {
            Vector3[] waypoints = new Vector3[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                waypoints[i] = transform.GetChild(i).position;
            }
            return waypoints;
        }
    }
}