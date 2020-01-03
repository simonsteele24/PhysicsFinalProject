using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBeforeReturnState : State
{
    public float idleTime = 2;
    bool isDoneWithIdle = false;
    Quaternion originalOrientation;

    public override States CheckForTransition()
    {
        if (isDoneWithIdle)
        {
            return States.Return;
        }

        return States.IdleBeforeReturn;
    }

    public override void OnEnterState()
    {
        originalOrientation = GetComponentInParent<Particle3D>().rotation;
        StartCoroutine(StartIdleCooldown());
    }

    public override void OnExitState() { }

    public override void UpdateState()
    {
        GetComponentInParent<Particle3D>().velocity = Vector3.zero;
        GetComponentInParent<Particle3D>().rotation = originalOrientation;
    }

    IEnumerator StartIdleCooldown()
    {
        yield return new WaitForSeconds(idleTime);
        isDoneWithIdle = true;
    }
}
