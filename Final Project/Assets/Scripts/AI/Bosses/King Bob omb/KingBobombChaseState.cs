using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingBobombChaseState : State
{
    public float rotationUpdateValue = 2;
    public float distanceToLeavePlayer = 10;
    public float distanceToThrowPlayer = 1;
    GameObject player;

    public override States CheckForTransition()
    {
        if (player.transform.position.y < GetComponentInParent<KingBobomb>().bossPlane.transform.transform.position.y && player.GetComponent<Particle3D>().collidingGameObject != GetComponentInParent<KingBobomb>().bossPlane)
        {
            return States.Idle;
        }

        float distance = Mathf.Abs(Vector3.Distance(transform.parent.position, player.transform.position));

        if (distance < distanceToThrowPlayer)
        {
            Vector3 direction = player.transform.position - transform.parent.position;
            float orientation = Vector3.Dot(direction, transform.parent.forward);

            if (orientation > 0)
            {
                return States.Throw;
            }
        }

        if (GetComponentInParent<KingBobomb>().isProne)
        {
            return States.Prone;
        }

        return States.Chase;
    }

    public override void UpdateState()
    {
        GetComponentInParent<KingBobomb>().MoveInADirection(transform.forward);
    }

    public override void OnEnterState()
    {
        player = GameObject.Find("Player");
        StartCoroutine(UpdatePosition());
    }

    public override void OnExitState()
    {
        player = null;
    }

    IEnumerator UpdatePosition()
    {
        while (player != null)
        {
            Vector3 position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.parent.LookAt(position);
            GetComponentInParent<Particle3D>().rotation = transform.parent.rotation;
            yield return new WaitForSeconds(rotationUpdateValue);
        }
    }
}
