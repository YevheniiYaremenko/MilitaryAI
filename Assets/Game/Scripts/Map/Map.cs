using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace App.Map
{
    public class Map : MonoBehaviour
    {
        static Map instance;
        public static Map Instance
        {
            get
            {
                if (instance==null)
                {
                    instance = FindObjectOfType<Map>();
                }
                return instance;
            }
        }
        
        [SerializeField] GameObject waypointPrefab;
        [SerializeField] GameObject pathpointPrefab;

        [SerializeField] float minWaypointGraphDistance = 1;

        public List<Waypoint> Waypoints { get; private set; }
        public Waypoint Target { get { return Waypoints[1]; } }
        public bool FullMap
        {
            get
            {
                if (Waypoints.Count==2)
                {
                    return false;
                }
                for (int i=2;i<Waypoints.Count;i++)
                {
                    if (!Waypoints[i].IsInvestigated)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private void Awake()
        {
            Waypoints = new List<Waypoint>();
        }

        public void AddWaypoint(Vector3 waypointPosition)
        {
            if (Waypoints.Count(way=>(way.Position-waypointPosition).magnitude<=minWaypointGraphDistance)==0)
            {
                var w = Instantiate(waypointPrefab, waypointPosition, Quaternion.identity, transform);
                AddWaypoint(w.GetComponent<Waypoint>());
            }
        }

        public void AddWaypoint(Waypoint waypoint)
        {
            Waypoints.Add(waypoint);
        }

        public void UpdateGraph()
        {
            for (int i=0;i<Waypoints.Count; i++)
            {
                if (i == 1)
                {
                    continue;
                }
                Waypoints[i].UpdateRelations();
            }
        }

        public List<Waypoint> FindWayToTarget()
        {
            List<Waypoint> path = new List<Waypoint>();

            if (CanCutPath())
            {
                Debug.Log("See target.");
                path.Add(Waypoints[1]);
                return path;
            }
            else
            {
                Debug.Log("Don`t see target. Find path using map...");
            }

            //try find path to target
            if (Waypoints[1].Relations.Count>0)
            {
                path = TryGoTo(Waypoints[1]);
                if (path.Count>0)
                {
                    Debug.Log("Find path using map. Moving to target throught point "+path[0].Position+"...");
                    return path;
                }
            }
            Debug.Log("Don`t find path using map.Try to approach to the target...");

            //try find path to nearest not investigated waypoint
            var ways = Waypoints.Where(
                              w => Waypoints.IndexOf(w) > 1
                              && !w.IsInvestigated)
                          .OrderBy(w => (w.Position - Waypoints[1].Position).magnitude);
            if (ways.Count()>0)
            {
                foreach(var w in ways)
                {
                    path = TryGoTo(w);
                    if (path.Count>0)
                    {
                        Debug.Log("Moving to nearest point to target ( throught point " + path[0].Position + ")...");
                        return path;
                    }
                }
                Debug.Log("All points are discovered. Try to find new points...");
            }

            return null;
        }

        public bool CanCutPath()
        {
            return CanWalk(Waypoints[0], Waypoints[1]);
        }

        public bool CanWalk(Waypoint w1, Waypoint w2)
        {
            Vector3 p1 = w1.Position + Vector3.up;
            Vector3 p2 = w2.Position + Vector3.up;
            foreach (var hit in Physics.RaycastAll(p1, (p2 - p1).normalized, (p1 - p2).magnitude))
            {
                if (hit.collider!=null && hit.collider.gameObject.layer==LayerMask.NameToLayer("Obstacle"))
                {
                    return false;
                }
            }
            return (p1 - p2).magnitude < Commander.Instance.VisionDistance;
        }
        
        List<Waypoint> TryGoTo(Waypoint target)
        {
            var path = PathResolver.FindPath(Waypoints, Waypoints[0], target);
            if (path.Count>0)
            {
                ShowWay(path);
            }
            return path;
        }

        void ShowWay(List<Waypoint> path)
        {
            foreach(var flag in FindObjectsOfType<Pathpoint>())
            {
                DestroyImmediate(flag.gameObject);
            }
            foreach(var p in path)
            {
                Instantiate(pathpointPrefab, p.Position, Quaternion.identity);
            }
        }

        public void InvestigatePosition()
        {
            foreach(var w in Waypoints.Where(w=>(w.Position-Waypoints[0].Position).magnitude<.3f))
            {
                w.Investigate();
            }
        }
    }
}
