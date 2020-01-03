using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingBobombThrowState : State
{
    public float durationToThrow = 2;
    public float durationToTransition = 2;
    public float verticalThrowingDistance = 20;
    public float horizontalThrowingDistance = 20;
    public Vector3 playerHoldingOffset = new Vector3(0,10,0);
    bool isReadyToThrow = false;
    bool isReadyToTransition = false;
    bool isHoldingPlayer = true;
    GameObject player;

    public override States CheckForTransition()
    {
        if (isReadyToTransition)
        {
            return States.Chase;
        }

        return States.Throw;
    }

    public override void OnEnterState()
    {
        player = GameObject.Find("Player");
        StartCoroutine(ConductThrowingSequence());
    }

    public override void OnExitState()
    {
        isReadyToThrow = false;
        isReadyToTransition = false;
        isHoldingPlayer = false;
    }

    public override void UpdateState()
    {
        if (isReadyToThrow)
        {
            player.GetComponent<Particle3D>().AddForce(player.GetComponent<Particle3D>().mass * transform.forward * horizontalThrowingDistance);
            player.GetComponent<Particle3D>().AddForce(player.GetComponent<Particle3D>().mass * transform.up * verticalThrowingDistance);
            isReadyToThrow = false;
        }
        else if (isHoldingPlayer)
        {
            player.GetComponent<Particle3D>().position = transform.parent.position + transform.InverseTransformDirection(playerHoldingOffset);
        }
    }

    IEnumerator ConductThrowingSequence()
    {
        yield return new WaitForSeconds(durationToThrow);
        isHoldingPlayer = false;
        isReadyToThrow = true;
        yield return new WaitForSeconds(durationToTransition);
        isReadyToTransition = true;
    }
}
