using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteIdleState : State
{
    public float distanceToLungeAtPlayer = 5;
    GameObject player;

    public override States CheckForTransition()
    {
        float distance = Mathf.Abs(Vector3.Distance(transform.parent.position, player.transform.position));

        if (distance < distanceToLungeAtPlayer)
        {
            return States.Lunge;
        }

        return States.CompleteIdle;
    }

    public override void OnEnterState()
    {
        player = GameObject.Find("Player");
    }

    public override void OnExitState() { }

    public override void UpdateState()
    {
        Vector3 position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.parent.LookAt(position);
    }
}
