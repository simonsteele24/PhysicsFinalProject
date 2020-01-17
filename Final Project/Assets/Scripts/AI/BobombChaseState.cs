using System.Collections;
using UnityEngine;

public class BobombChaseState : State
{
    // Floats
    public float distanceToLeavePlayer = 10;
    public float alertJumpStrength = 300;
    public float bombLifetime = 5;
    public float chaseSpeed = 2;

    // Game Objects
    public GameObject player;





    // This function is triggered once per frame to check for transition of states
    public override States CheckForTransition()
    {
        // Since the bobomb has been trigger, there is no going back
        return States.Chase;
    }





    // This function is called once per frame 
    public override void UpdateState()
    {
        // Look at the player
        Vector3 position = new Vector3(player.transform.position.x, transform.parent.position.y, player.transform.position.z);
        transform.parent.LookAt(position);
        GetComponentInParent<Particle3D>().rotation = transform.rotation;

        // Sprint forward in that direction
        GetComponentInParent<Bobomb>().SprintInADirection(transform.forward, chaseSpeed);
    }





    // This function triggers when the state is entered
    public override void OnEnterState()
    {
        player = GameObject.Find("Player");
        StartCoroutine(StartDetonation());
    }





    // This function triggers when the state is exited
    public override void OnExitState()
    {
        player = null;
    }





    // This function starts when the bomb is ready to explode
    IEnumerator StartDetonation()
    {
        // Wait for bobomb lifetime cycle 
        yield return new WaitForSeconds(bombLifetime);

        // Explode the bomb
        GetComponentInParent<Bobomb>().Explode();
        Destroy(transform.parent.gameObject);
    }
}
