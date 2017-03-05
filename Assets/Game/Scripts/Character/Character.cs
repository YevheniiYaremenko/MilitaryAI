using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using App.Map;

public enum CharacterState
{
    Idle,
    Moving,
    Scanning
}

public class Character : MonoBehaviour
{
    [SerializeField] protected Transform eyes;
    [SerializeField] protected CapsuleCollider collider;

    [SerializeField] protected float eyeAngleRange = 180;
    [SerializeField] protected float eyeAngleIncremention = 1;
    [SerializeField] protected float speedMoving = 2;
    [SerializeField] protected float speedRotation = 2;
    [SerializeField] protected float speedScaning = 180;
    [SerializeField] float maxVisionDistance = 30;
    [SerializeField] protected float waypointOffset = .5f;

    [HideInInspector] public Vector3 direction;
    Vector3 lastPosition = Vector3.zero;

    public bool IsMoving { get { return direction != Vector3.zero; } }
    protected float VisionDistance { get { return maxVisionDistance * MainController.Instance.sun.intensity; } }

    protected RaycastHit LookForward()      //update for look forward without characters
    {
        RaycastHit hit;
        Physics.SphereCast(eyes.position, collider.radius, eyes.forward, out hit, VisionDistance);
        return hit;
    }

    private void Update()
    {
        direction = transform.position - lastPosition;
        lastPosition = transform.position;
    }
}
