﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobombChaseState : State
{
    public float distanceToLeavePlayer = 10;
    public float alertJumpStrength = 300;
    public float bombLifetime = 5;
    public float chaseSpeed;
    public GameObject player;

    public override States CheckForTransition()
    {
        float distance = Mathf.Abs(Vector3.Distance(transform.parent.position, player.transform.position));

        return States.Chase;
    }

    public override void UpdateState()
    {
        Vector3 position = new Vector3(player.transform.position.x, transform.parent.position.y, player.transform.position.z);
        transform.parent.LookAt(position);
        GetComponentInParent<Particle3D>().rotation = transform.rotation;

        if (GetComponentInParent<Bobomb>().isGrounded)
        {
            GetComponentInParent<Bobomb>().SprintInADirection(transform.forward, chaseSpeed);
        }
    }

    public override void OnEnterState()
    {
        player = GameObject.Find("Player");
        GetComponentInParent<Bobomb>().isChasing = true;
        GetComponentInParent<Particle3D>().AddForce(GetComponentInParent<Particle3D>().mass * Vector3.up * alertJumpStrength);
        StartCoroutine(StartBombLifetime());
    }

    public override void OnExitState()
    {
        player = null;
        GetComponentInParent<Bobomb>().isChasing = false;
    }

    IEnumerator StartBombLifetime()
    {
        yield return new WaitForSeconds(bombLifetime);
        if (player != null)
        {
            GetComponentInParent<Bobomb>().Explode();
            Destroy(transform.parent.gameObject);
        }
    }
}
