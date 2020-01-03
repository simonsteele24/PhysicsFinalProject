﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobombChaseState : State
{
    public float distanceToLeavePlayer = 10;
    public float bombLifetime = 5;
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
        GetComponentInParent<Goomba>().MoveInADirection(transform.forward);
    }

    public override void OnEnterState()
    {
        player = GameObject.Find("Player");
        StartCoroutine(StartBombLifetime());
    }

    public override void OnExitState()
    {
        player = null;
    }

    IEnumerator StartBombLifetime()
    {
        yield return new WaitForSeconds(bombLifetime);
        if (player != null)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
