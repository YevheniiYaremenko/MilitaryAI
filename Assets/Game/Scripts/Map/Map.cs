using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public List<Waypoint> Waypoints { get; private set; }

        private void Awake()
        {
            Waypoints = new List<Waypoint>();
        }

        public void AddWaypoint(Vector3 waypointPosition)
        {
            AddWaypoint(new Waypoint(waypointPosition));
            Instantiate(waypointPrefab, waypointPosition, Quaternion.identity, transform);
        }

        public void AddWaypoint(Waypoint waypoint)
        {
            Waypoints.Add(waypoint);
        }

        public void UpdateGraph()
        {
            foreach(var w in Waypoints)
            {
                w.UpdateRelations();
            }
        }

        public Vector3 NearestTarget()  //implement find path
        {
            return Waypoints[Random.Range(2, Waypoints.Count)].Position;
        }
    }
}
