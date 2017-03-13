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
        }

        public void UpdateRelations()
        {
            Relations = new List<Waypoint>();
            Relations.Clear();
            foreach (var w in Map.Instance.Waypoints)
            {
                if (w==this)
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
            Vector3 originPos = Position + Vector3.up;
            Vector3 targetPos = target.Position + Vector3.up;
            
            var hits = Physics.SphereCastAll(originPos, MainController.Instance.characterRadius, targetPos - originPos,(targetPos - originPos).magnitude);
            if (hits.Length==0)
            {
                return true;
            }
            foreach(var hit in hits)
            {
                if (hit.collider!=null && hit.collider.gameObject.layer==LayerMask.NameToLayer("Obstacle"))
                {
                    return false;
                }
            }
            return true;
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
