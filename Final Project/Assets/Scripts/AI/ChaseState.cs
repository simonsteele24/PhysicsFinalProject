using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public float distanceToLeavePlayer = 10;
    public float alertJumpStrength = 20;
    public float chaseSpeed;
    GameObject player;

    public override States CheckForTransition()
    {
        float distance = Mathf.Abs(Vector3.Distance(transform.parent.position, player.transform.position));

        if (distance > distanceToLeavePlayer)
        {
            return States.Idle;
        }

        return States.Chase;
    }

    public override void UpdateState()
    {
        Vector3 position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.parent.LookAt(position);
        GetComponentInParent<Particle3D>().rotation = transform.rotation;
        if (GetComponentInParent<Goomba>().isGrounded)
        {
            GetComponentInParent<Goomba>().SprintInADirection(transform.forward, chaseSpeed);
        }
    }

    public override void OnEnterState()
    {
        player = GameObject.Find("Player");
        GetComponentInParent<Particle3D>().AddForce(GetComponentInParent<Particle3D>().mass * Vector3.up * alertJumpStrength);
        GetComponentInParent<Goomba>().isChasing = true;
    }

    public override void OnExitState()
    {
        GetComponentInParent<Goomba>().isChasing = false;
    }
}
