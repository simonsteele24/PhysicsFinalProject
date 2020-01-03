using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingBobombIdleState : State
{
    GameObject player;

    public override States CheckForTransition()
    {
        if (player.GetComponent<Particle3D>().collidingGameObject == GetComponentInParent<KingBobomb>().bossPlane)
        {
            return States.Chase;
        }

        return States.Idle;
    }

    public override void OnEnterState()
    {
        player = GameObject.Find("Player");
    }

    public override void OnExitState() { }

    public override void UpdateState() { }
}
