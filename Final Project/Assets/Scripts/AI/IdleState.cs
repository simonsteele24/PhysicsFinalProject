using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public float rotationSpeed;
    public float distanceToChasePlayer = 5;
    GameObject player;

    public override States CheckForTransition()
    {
        float distance = Mathf.Abs(Vector3.Distance(transform.parent.position, player.transform.position));

        if (distance < distanceToChasePlayer)
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

    public override void UpdateState()
    {
        transform.parent.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        transform.parent.GetComponent<Particle3D>().rotation = transform.parent.rotation;
        GetComponentInParent<Goomba>().MoveInADirection(transform.parent.forward);
    }
}
