using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commander : Character
{
    static Commander instance;
    public static Commander Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Commander>();
            }
            return instance;
        }
    }

    Vector3 target = Vector3.zero;

    bool NearTarget { get { return Vector3.Distance(transform.position, target) <.5f; } }

    public void TakeOrder(Vector3 position)
    {
        Debug.Log(string.Format("Commander: 'New target - {0}'", position));
        target = position;
    }

    private void Update()
    {
        if (!NearTarget)
        {
            var localTarget = GetWaypoint();
            
            if (Vector3.Angle(transform.forward,localTarget-transform.position)>speedRotation*Time.deltaTime)
            {
                int side = Vector3.Angle(-transform.right, localTarget - transform.position) < Vector3.Angle(transform.right, localTarget - transform.position) ? -1 : 1;
                transform.Rotate(Vector3.up * speedRotation * Time.deltaTime * side);
            }

            if (CanStep)
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.forward * 100, speedMoving * Time.deltaTime);
            }
        }
    }

    Vector3 GetWaypoint()
    {
        return target;
    }
}
