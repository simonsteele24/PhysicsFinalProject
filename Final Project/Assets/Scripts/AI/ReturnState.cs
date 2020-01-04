using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnState : State
{
    public float distanceToRemainIdle = 1;
    GameObject pole;

    public override States CheckForTransition()
    {
        float distance = Mathf.Abs(Vector3.Distance(transform.parent.position, pole.transform.position));

        if (distance < distanceToRemainIdle)
        {
            return States.CompleteIdle;
        }

        return States.Return;
    }

    public override void OnEnterState()
    {
        pole = GetComponentInParent<Chainchomp>().pole;
    }

    public override void OnExitState()
    {
        GetComponentInParent<Chainchomp>().isAttacking = false;
    }

    public override void UpdateState()
    {
        Vector3 position = new Vector3(pole.transform.position.x, transform.position.y, pole.transform.position.z);
        transform.parent.LookAt(position);
        GetComponentInParent<Chainchomp>().MoveInADirection(transform.forward);
    }
}
