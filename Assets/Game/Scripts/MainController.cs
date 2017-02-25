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

    public void SetPosition(Vector3 position)
    {
        Commander.Instance.TakeOrder(position);
    }
}
