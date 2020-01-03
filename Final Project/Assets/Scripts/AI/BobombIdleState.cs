using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobombIdleState : State
{
    public float rotationSpeed;
    public float distanceToChasePlayer = 5;
    GameObject player;

    public override States CheckForTransition()
    {
        float distance = Mathf.Abs(Vector3.Distance(transform.parent.position, player.transform.position));

        if (distance < distanceToChasePlayer)
        {
            Vector3 direction = player.transform.position - transform.parent.position;
            float orientation = Vector3.Dot(direction, transform.parent.forward);

            if (orientation > 0)
            {
                return States.Chase;
            }
        }

        return States.Idle;
    }

    public override void OnEnterState()
    {
        player = GameObject.Find("Player");
    }

    public override void OnExitState() { }

    public override void UpdateState()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        GetComponentInParent<Goomba>().MoveInADirection(transform.forward);
    }
}
