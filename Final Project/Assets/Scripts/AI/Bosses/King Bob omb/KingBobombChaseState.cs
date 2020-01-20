using System.Collections;
using UnityEngine;

public class KingBobombChaseState : State
{
    // Floats
    public float rotationUpdateValue = 2;
    public float distanceToLeavePlayer = 10;
    public float distanceToThrowPlayer = 1;
    public float rotationSpeed = 0.1f;
    public float bossplaneY;

    // Gameobjects
    GameObject player;





    // This function checks for a transition between AI states
    public override States CheckForTransition()
    {
        // Is the player actively on the boss plane?
        if (player.transform.position.y < bossplaneY && player.GetComponent<Particle3D>().collidingGameObject != GetComponentInParent<KingBobomb>().bossPlane)
        {
            // If no, then keep chasing
            return States.Idle;
        }

        // Is the king currently prone?
        if (GetComponentInParent<KingBobomb>().CheckIfProne())
        {
            // If yes, then keep him prone
            return States.Prone;
        }

        // Find the distance to the player
        float distance = Mathf.Abs(Vector3.Distance(transform.parent.position, player.transform.position));
        // Is the distance less than what we need to throw the player?
        if (distance < distanceToThrowPlayer)
        {
            // If yes then throw the player
            Vector3 direction = player.transform.position - transform.parent.position;
            float orientation = Vector3.Dot(direction, transform.parent.forward);

            if (orientation > 0)
            {
                return States.Throw;
            }
        }

        // If all else is not true, then keep chasing
        return States.Chase;
    }





    // This function calls the state to update the object with its behaviour
    public override void UpdateState()
    {
        GetComponentInParent<KingBobomb>().MoveInADirection(transform.forward);
        GetComponentInParent<KingBobomb>().bossAnimator.SetTrigger("Walking");

        Vector3 dir = player.transform.position - transform.parent.position;
        dir.y = 0; // keep the direction strictly horizontal
        Quaternion rot = Quaternion.LookRotation(dir);

        GetComponentInParent<Particle3D>().rotation = Quaternion.Slerp(transform.parent.rotation, rot, Time.deltaTime * rotationSpeed);
    }





    // This function is called whenever the state has been entered
    public override void OnEnterState()
    {
        // Find the player gameobject and update position
        bossplaneY = transform.parent.position.y;
        player = GameObject.FindGameObjectWithTag("Player");
    }





    // This function is called whenever the state has been exited
    public override void OnExitState()
    {
        // Set player to null
        player = null;
    }
}
