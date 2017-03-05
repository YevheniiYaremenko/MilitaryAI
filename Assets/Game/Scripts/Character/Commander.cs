using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using App.Map;

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

    public CharacterState state;

    Vector3 target = Vector3.zero;
    Vector3 localTarget = Vector3.zero;
    bool NearTarget { get { return Vector3.Distance(transform.position, target) <.5f; } }

    public void TakeOrder(Vector3 position)
    {
        Debug.Log(string.Format("Commander: 'New target - {0}'", position));
        target = position;
    }

    private void Update()
    {
        if (NearTarget)
        {
            return;
        }

        switch (state)
        {
            case CharacterState.Idle:
                FindPath();
                break;
        }
    }

    #region Path

    void FindPath()
    {
        state = CharacterState.Scanning;
        StartCoroutine(ScanPosition());
    }

    IEnumerator ScanPosition()
    {
        float angle = 0;

        transform.eulerAngles -= Vector3.up * speedScaning * Time.deltaTime;
        var lastHit = LookForward();
        transform.eulerAngles += Vector3.up * speedScaning * Time.deltaTime;

        while (angle<360)
        {
            angle += speedScaning * Time.deltaTime;
            transform.eulerAngles += Vector3.up * speedScaning * Time.deltaTime;

            var currentHit = LookForward();
            if (lastHit.collider!=null && currentHit.collider==null)
            {
                GenerateWaypoint(lastHit);
            }
            if (lastHit.collider == null && currentHit.collider != null)
            {
                GenerateWaypoint(currentHit);
            }
            if (lastHit.collider!=null && currentHit.collider!=null && (lastHit.point-currentHit.point).magnitude>1)
            {
                GenerateWaypoint(currentHit);
                GenerateWaypoint(lastHit);
            }

            lastHit = currentHit;
            yield return new WaitForEndOfFrame();
        }

        Map.Instance.UpdateGraph();
        yield return new WaitForSeconds(.5f);
        FindOptimalTarget();
    }

    void FindOptimalTarget()
    {
        localTarget = Map.Instance.NearestTarget();
    }

    void GenerateWaypoint(RaycastHit hit)
    {
        Vector3 pos = hit.point + hit.normal.normalized;
        pos.y = 0;
        Map.Instance.AddWaypoint(pos);
    }

    #endregion Path
}
