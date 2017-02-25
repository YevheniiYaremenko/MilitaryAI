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

    public void TakeOrder(Vector3 position)
    {
        Debug.Log(string.Format("Commander: 'New target - {0}'", position));
        target = position;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speedMoving * Time.deltaTime);
    }
}
