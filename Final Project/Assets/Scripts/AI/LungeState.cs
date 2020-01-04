using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungeState : State
{
    Vector3 player;

    public override States CheckForTransition()
    {
        Debug.Log(Vector3.Distance(transform.parent.position, GetComponentInParent<Chainchomp>().pole.transform.position));
        Debug.Log(GetComponentInParent<Chainchomp>().distanceToMoveFromPole);

        if (Vector3.Distance(transform.parent.position, GetComponentInParent<Chainchomp>().pole.transform.position) > GetComponentInParent<Chainchomp>().distanceToStop)
        {
            return States.IdleBeforeReturn;
        }

        return States.Lunge;
    }

    public override void OnEnterState()
    {
        GetComponentInParent<Chainchomp>().isAttacking = true;
        player = GameObject.Find("Player").transform.position;
        Vector3 position = new Vector3(player.x, transform.position.y, player.z);
        transform.parent.LookAt(position);
        GetComponentInParent<Particle3D>().rotation = transform.parent.rotation;
    }

    public override void OnExitState() { }

    public override void UpdateState()
    {
        GetComponentInParent<Chainchomp>().MoveInALungeDirection(transform.parent.forward);
    }
}
