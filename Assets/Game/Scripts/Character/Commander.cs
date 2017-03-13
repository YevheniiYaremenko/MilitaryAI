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

    bool NearTarget { get { return (transform.position - target).magnitude <.5f; } }
    bool NearLocalTarget { get { return localTarget!=null && (transform.position - localTarget.Position).magnitude < .5f; } }

    public void TakeOrder(Vector3 position)
    {
        NewTarget(position);
    }

    private void Update()
    {
        if (NearTarget || state == CharacterState.Stop)
        {
            return;
        }

        if (localTarget != null)
        {
            if (!NearLocalTarget)
            {
                state = CharacterState.Moving;
            }
            else
            {
                OnLocalTargetAchieve();
            }
        }

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
        if (localTarget==null)
        {
            StartCoroutine(ScanPosition());
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
        yield return new WaitForSeconds(.5f);
        state = CharacterState.Idle;
    }

    void NewTarget(Vector3 target)
    {
        this.target = target;
        localTarget = null;
        StartCoroutine(ScanPosition());
    }

    void OnLocalTargetAchieve()
    {
        if (localTarget!=null && !localTarget.IsInvestigated)
        {
            StartCoroutine(ScanPosition());
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
            localTarget = Map.Instance.Target;
        }
    }

    #endregion Moving
}
