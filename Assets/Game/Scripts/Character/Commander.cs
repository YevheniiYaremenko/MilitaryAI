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

    bool NearTarget { get { return (transform.position - target).magnitude < .3f; } }
    bool NearLocalTarget { get { return path.Count>0 && (transform.position - path[0].Position).magnitude < .3f; } }

    public CharacterState state;
    
    public void TakeOrder(Vector3 position)
    {
        NewTarget(position);
    }

    private void Update()
    {
        if (state == CharacterState.Stop)
        {
            return;
        }
        
        CheckPath();

        switch (state)
        {
            case CharacterState.Idle:
                FindPath();
                break;
            case CharacterState.Scanning:
                break;
            case CharacterState.Moving:
                MoveToTarget();
                break;
            case CharacterState.Break:
                OnPathpointAchieve();
                break;
        }
    }

    #region Path

    protected override void FindPath()
    {
        if (Map.Instance.FullMap)
        {
            state = CharacterState.Stop;
            return;
        }
        
        base.FindPath();

        if (path.Count == 0)
        {
            StartCoroutine(ScanPosition());
        }
        else
        {
            state = CharacterState.Moving;
        }
    }

    protected override IEnumerator ScanPosition()
    {
        if (state == CharacterState.Scanning)
        {
            yield break;
        }
        
        state = CharacterState.Scanning;

        yield return base.ScanPosition();

        yield return new WaitForSeconds(1);
        Map.Instance.UpdateGraph();
        Map.Instance.InvestigatePosition(); 
        yield return new WaitForSeconds(.5f);
        state = CharacterState.Idle;
    }

    void NewTarget(Vector3 target)
    {
        this.target = target;
        path.Clear();
        TryCutPath();
        if (path.Count==0)
        {
            StartCoroutine(ScanPosition());
        }
    }

    void CheckPath()
    {
        if (path.Count > 0 && (transform.position - path[0].Position).magnitude < .3f)
        {
            state = CharacterState.Break;
        }
    }

    void OnPathpointAchieve()
    {
        path.RemoveAt(0);
        if (NearTarget)
        {
            state = CharacterState.Stop;
        }
        else
        {
            if (path.Count==0)
            {
                StartCoroutine(ScanPosition());
            }
            else
            {
                state = CharacterState.Moving;
            }
        }
    }

    #endregion Path

    #region Moving

    protected override void MoveToTarget()
    {
        base.MoveToTarget();

        TryCutPath();
    }

    void TryCutPath()
    {
        if (Map.Instance.CanCutPath())
        {
            path.Clear();
            path.Add(Map.Instance.Target);
            state = CharacterState.Moving;
        }
    }

    #endregion Moving
}
