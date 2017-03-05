using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using App.Map;

public class MainController : MonoBehaviour
{
    static MainController instance;
    public static MainController Instance
    {
        get
        {
            if (instance==null)
            {
                instance = FindObjectOfType<MainController>();
            }
            return instance;
        }
    }

    [SerializeField] Transform target;
    [SerializeField] Commander commander;
    public Light sun;

    public float characterRadius = .5f;

    void Start()
    {
        Map.Instance.AddWaypoint(commander.GetComponent<Waypoint>());
        Map.Instance.AddWaypoint(target.GetComponent<Waypoint>());
    }

    public void SetPosition(Vector3 position)
    {
        target.position = position;
        Commander.Instance.TakeOrder(target.position);
    }
}
