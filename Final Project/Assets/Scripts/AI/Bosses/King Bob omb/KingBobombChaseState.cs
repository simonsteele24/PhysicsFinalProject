using System.Collections;
using UnityEngine;

public class KingBobombChaseState : State
{
    // Floats
    public float rotationUpdateValue = 2;
    public float distanceToLeavePlayer = 10;
    public float distanceToThrowPlayer = 1;

    // Gameobjects
    GameObject player;





    // This function checks for a transition between AI states
    public override States CheckForTransition()
    {
        // Is the player actively on the boss plane?
        if (player.transform.position.y < GetComponentInParent<KingBobomb>().bossPlane.transform.transform.position.y && player.GetComponent<Particle3D>().collidingGameObject != GetComponentInParent<KingBobomb>().bossPlane)
        {
            // If no, then keep chasing
            return States.Idle;
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

        // Is the king currently prone?
        if (GetComponentInParent<KingBobomb>().CheckIfProne())
        {
            // If yes, then keep him prone
            return States.Prone;
        }

        // If all else is not true, then keep chasing
        return States.Chase;
    }





    // This function calls the state to update the object with its behaviour
    public override void UpdateState()
    {
        GetComponentInParent<KingBobomb>().MoveInADirection(transform.forward);
        GetComponentInParent<KingBobomb>().bossAnimator.SetTrigger("Walking");
    }





    // This function is called whenever the state has been entered
    public override void OnEnterState()
    {
        // Find the player gameobject and update position
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(UpdatePosition());
    }





    // This function is called whenever the state has been exited
    public override void OnExitState()
    {
        // Set player to null
        player = null;
    }





    // This function updates the rotation of the object to look toward the player after a certain amount of time
    IEnumerator UpdatePosition()
    {
        // Is the player valid?
        while (player != null)
        {
            // If yes, then find the position of the player and look towards them
            Vector3 position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.parent.LookAt(position);
            GetComponentInParent<Particle3D>().rotation = transform.parent.rotation;

            yield return new WaitForSeconds(rotationUpdateValue);
        }
    }
}
