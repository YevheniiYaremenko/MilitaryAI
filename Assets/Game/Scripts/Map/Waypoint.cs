using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App.Map
{
    public class Waypoint : MonoBehaviour
    {
        public Vector3 Position { get; private set; }
        public List<Waypoint> Relations { get; private set; }

        public Waypoint(Vector3 position)
        {
            Position = position;
        }

        public void UpdateRelations()
        {
            foreach(var w in Map.Instance.Waypoints)
            {
                Relations = new List<Waypoint>();
                Relations.Clear();
                if (HasRelation(w))
                {
                    Relations.Add(w);
                }
            }
        }

        bool HasRelation(Waypoint target)
        {
            Vector3 originPos = Position + Vector3.up;
            Vector3 targetPos = target.Position + Vector3.up;
            
            var hits = Physics.SphereCastAll(originPos, MainController.Instance.characterRadius, targetPos - originPos);
            if (hits.Length<0)
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
    }
}
