using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using App.Map;
using UnityEngine.SceneManagement;

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

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadRambo()
    {
        SceneManager.LoadScene("Rambo");
    }

    public void LoadCommando()
    {
        SceneManager.LoadScene("Commando");
    }
}
