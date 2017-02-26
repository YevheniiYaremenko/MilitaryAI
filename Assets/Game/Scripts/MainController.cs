using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void SetPosition(Vector3 position)
    {
        target.position = position;
        Commander.Instance.TakeOrder(target.position);
    }
}
