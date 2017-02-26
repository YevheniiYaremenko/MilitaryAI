using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] protected Transform eyes;

    [SerializeField] protected float speedMoving = 2;
    [SerializeField] protected float speedRotation = 2;
    [SerializeField] float maxVisionDistance = 30;

    [HideInInspector] public Vector3 direction;
    Vector3 lastPosition = Vector3.zero;

    public bool IsMoving { get { return direction != Vector3.zero; } }
    protected float VisionDistance { get { return maxVisionDistance * FindObjectOfType<Light>().intensity; } }

    private void Update()
    {
        direction = transform.position - lastPosition;
        lastPosition = transform.position;
    }
}
