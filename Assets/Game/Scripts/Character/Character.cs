using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using App.Map;
using System.Linq;

public enum CharacterState
{
    Idle,
    Moving,
    Scanning,
    Stop
}

public class Character : MonoBehaviour
{
    [SerializeField] protected Transform eyes;
    [SerializeField] protected CapsuleCollider collider;
    [SerializeField] Transform azimuth;

    [SerializeField] protected float eyeAngleRange = 180;
    [SerializeField] protected float eyeAngleIncremention = 1;
    [SerializeField] protected float speedMoving = 2;
    [SerializeField] protected float speedRotation = 2;
    [SerializeField] protected float speedScaning = 180;
    [SerializeField] float maxVisionDistance = 30;
    [SerializeField] protected float waypointOffset = .5f;

    [HideInInspector] public Vector3 direction;
    Vector3 lastPosition = Vector3.zero;

    protected Vector3 target = Vector3.zero;
    protected Waypoint localTarget;

    public bool IsMoving { get { return direction != Vector3.zero; } }
    public float VisionDistance { get { return maxVisionDistance * MainController.Instance.sun.intensity; } }
    protected bool CanStep
    {
        get
        {
            RaycastHit hit;
            Physics.Raycast(eyes.position, transform.forward, out hit, VisionDistance);
            return DistanceTo(hit.point) > 1;
        }
    }

    protected RaycastHit LookForward()
    {
        return new List<RaycastHit>(Physics.SphereCastAll(eyes.position, collider.radius, eyes.forward, VisionDistance))
            .OrderBy(h => DistanceTo(h.point))
            .Where( h=> 
                h.collider == null
                || (h.collider != null && h.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
             )
            .FirstOrDefault();
    }

    private void Update()
    {
        direction = transform.position - lastPosition;
        lastPosition = transform.position;
    }

    #region Path
    protected virtual void FindPath()
    {
        localTarget = Map.Instance.FindWayToTarget();
    }

    protected virtual IEnumerator ScanPosition()
    {
        float angle = 0;
        int waypointCount = 0;

        transform.eulerAngles -= Vector3.up * speedScaning * Time.deltaTime;
        var lastHit = LookForward();
        transform.eulerAngles += Vector3.up * speedScaning * Time.deltaTime;

        while (angle < 360)
        {
            angle += speedScaning * Time.deltaTime;
            transform.eulerAngles += Vector3.up * speedScaning * Time.deltaTime;

            var currentHit = LookForward();
            if (lastHit.collider != null && currentHit.collider == null)
            {
                waypointCount++;
                GenerateWaypoint(lastHit);
            }
            if (lastHit.collider == null && currentHit.collider != null)
            {
                waypointCount++;
                GenerateWaypoint(currentHit);
            }
            if (lastHit.collider != null && currentHit.collider != null && (lastHit.point - currentHit.point).magnitude > 1)
            {
                waypointCount++;
                GenerateWaypoint(currentHit);
                GenerateWaypoint(lastHit);
            }

            lastHit = currentHit;
            yield return new WaitForEndOfFrame();
        }

        if (waypointCount == 0)
        {
            GenerateWaypoint(lastHit);
        }

        if (localTarget!=null)
        {
            localTarget.Investigate();
        }
    }

    protected void GenerateWaypoint(RaycastHit hit)
    {
        Vector3 pos = hit.point + hit.normal.normalized;
        pos.y = 0;
        Map.Instance.AddWaypoint(pos);
    }

    float DistanceTo(Vector3 point)
    {
        return (eyes.position - point).magnitude;
    }

    #endregion Path

    #region Moving

    protected virtual void MoveToTarget()
    {
        azimuth.LookAt(localTarget.Position);
        transform.eulerAngles = Vector3.up * Mathf.MoveTowards(transform.eulerAngles.y, azimuth.eulerAngles.y, speedRotation * Time.deltaTime);
        if (CanStep)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.forward * 100, speedMoving * Time.deltaTime);
        }
    }

    #endregion Moving
}
