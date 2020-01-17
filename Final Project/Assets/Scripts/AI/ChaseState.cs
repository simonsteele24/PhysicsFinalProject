using UnityEngine;

public class ChaseState : State
{
    // Floats
    public float distanceToStopChasing = 10;
    public float alertJumpStrength = 20;
    public float chaseSpeed;

    // Gameobjects
    GameObject player;
    Goomba goombaScript;





    // This function is called once per frame and checks for a change in state
    public override States CheckForTransition()
    {
        // Check if distance is out of chasing range
        float distance = Mathf.Abs(Vector3.Distance(transform.parent.position, player.transform.position));
        if (distance > distanceToStopChasing)
        {
            // If yes, then go back to idle
            return States.Idle;
        }

        // If no, then continue to chase
        return States.Chase;
    }





    // This function is called once per frame to update the object based on state behaviour
    public override void UpdateState()
    {
        // Look at the player
        Vector3 position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.parent.LookAt(position);
        GetComponentInParent<Particle3D>().rotation = transform.rotation;

        // Is the object grounded?
        if (goombaScript.GetGroundedState())
        {
            // Move forward, towards the player
            goombaScript.SprintInADirection(transform.forward, chaseSpeed);
        }
    }





    // This function is called whenever the state is entered
    public override void OnEnterState()
    {
        player = GameObject.Find("Player");
        goombaScript = GetComponentInParent<Goomba>();
        GetComponentInParent<Particle3D>().AddForce(GetComponentInParent<Particle3D>().mass * Vector3.up * alertJumpStrength);
        goombaScript.SetHoppingState(true);
        goombaScript.SetChasingState(true);
    }

    public override void OnExitState()
    {
        GetComponentInParent<Goomba>().SetChasingState(false);
    }
}
