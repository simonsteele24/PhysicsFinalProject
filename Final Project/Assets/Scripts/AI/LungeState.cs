using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungeState : State
{
    Vector3 player;
    public float waitToLunge = 2.0f;
    bool canLunge = false;

    public override States CheckForTransition()
    {
        if (!canLunge)
        {
            player = GameObject.Find("Player").transform.position;
            Vector3 position = new Vector3(player.x, transform.position.y, player.z);
            transform.parent.LookAt(position);
            GetComponentInParent<Particle3D>().rotation = transform.parent.rotation;
        }

        if (Vector3.Distance(transform.parent.position, GetComponentInParent<Chainchomp>().pole.transform.position) > GetComponentInParent<Chainchomp>().distanceToStop)
        {
            return States.IdleBeforeReturn;
        }

        return States.Lunge;
    }

    public override void OnEnterState()
    {
        canLunge = false;
        StartCoroutine(WaitToLunge());
    }

    public override void OnExitState() { }

    public override void UpdateState()
    {
        if (canLunge)
        {
            GetComponentInParent<Chainchomp>().isAttacking = true;
            GetComponentInParent<Chainchomp>().MoveInALungeDirection(transform.parent.forward);
        }
    }

    IEnumerator WaitToLunge()
    {
        yield return new WaitForSeconds(waitToLunge);
        canLunge = true;
    }
}
