using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingBobombProneState : State
{
    public override States CheckForTransition()
    {
        if (!GetComponentInParent<KingBobomb>().isProne)
        {
            return States.Chase;
        }

        return States.Prone;
    }

    public override void OnEnterState() { }

    public override void OnExitState() { }

    public override void UpdateState() { }
}
