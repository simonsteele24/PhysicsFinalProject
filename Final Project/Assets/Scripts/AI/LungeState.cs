using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungeState : State
{
    public float lungeTime = 2;
    bool isOverWithLunging = false;
    GameObject player;

    public override States CheckForTransition()
    {
        float distance = Mathf.Abs(Vector3.Distance(transform.parent.position, player.transform.position));

        if (isOverWithLunging)
        {
            return States.Return;
        }

        return States.Lunge;
    }

    public override void OnEnterState()
    {
        player = GameObject.Find("Player");
        StartCoroutine(StartLungeCooldown());
    }

    public override void OnExitState() { }

    public override void UpdateState()
    {
        Vector3 position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.parent.LookAt(position);
        GetComponentInParent<Chainchomp>().MoveInALungeDirection(transform.forward);
    }

    IEnumerator StartLungeCooldown()
    {
        yield return new WaitForSeconds(lungeTime);
        isOverWithLunging = true;
    }
}
