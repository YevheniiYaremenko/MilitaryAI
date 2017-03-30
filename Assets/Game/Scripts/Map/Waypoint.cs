using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App.Map
{
    public class Waypoint : MonoBehaviour
    {
        [SerializeField] LineRenderer relationViewer;
        public Color viewerColor;

        public Vector3 Position { get { return transform.position; } }
        public List<Waypoint> Relations { get; private set; }
        public bool IsInvestigated { get; private set; }

        void Awake()
        {
            IsInvestigated = false;
            if (relationViewer!=null)
            {
                viewerColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", viewerColor);
                relationViewer.material.SetColor("_Color", viewerColor);
            }
            Relations = new List<Waypoint>();
        }

        public void UpdateRelations()
        {
            Relations = new List<Waypoint>();
            Relations.Clear();
            for (int i=0;i< Map.Instance.Waypoints.Count;i++)
            {
                if (i==1)
                {
                    continue;
                }
                var w = Map.Instance.Waypoints[i];
                if (w == this)
                {
                    continue;
                }
                if (HasRelation(w))
                {
                    Relations.Add(w);
                }
            }
            ShowRelations();
        }
        public void Investigate()
        {
            IsInvestigated = true;
        }

        bool HasRelation(Waypoint target)
        {
            return Map.Instance.CanWalk(this, target);
        }

        void ShowRelations()
        {
            if (relationViewer==null)
            {
                return;
            }
            List<Vector3> points = new List<Vector3>();
            points.Add(transform.position + Vector3.up);
            foreach(var w in Relations)
            {
                points.Add(w.Position + Vector3.up);
                points.Add(transform.position + Vector3.up);
            }

            relationViewer.numPositions = points.Count;
            relationViewer.SetPositions(points.ToArray());
        }
    }
}
