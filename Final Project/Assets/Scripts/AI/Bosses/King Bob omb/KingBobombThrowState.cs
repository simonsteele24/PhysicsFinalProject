using System.Collections;
using UnityEngine;

public class KingBobombThrowState : State
{
    // Floats
    public float durationToThrow = 2;
    public float durationToTransition = 2;
    public float verticalThrowingDistance = 20;
    public float horizontalThrowingDistance = 20;

    // Vector3's
    public Vector3 playerHoldingOffset = new Vector3(0,10,0);

    // Booleans
    bool isReadyToTransition = false;
    bool isHoldingPlayer = true;

    // Game objects
    GameObject player;
    Particle3D playerParticle;




    // This function checks for any tranitions bettween states
    public override States CheckForTransition()
    {
        // Has the bobomb already thrown the player?
        if (isReadyToTransition)
        {
            // If yes, then continue to chase
            return States.Chase;
        }

        // Otherwise, keep preparing to throw
        return States.Throw;
    }





    // This function is called whenever the state is entered
    public override void OnEnterState()
    {
        // Set the player and ready the throw
        player = GameObject.Find("Player");
        playerParticle = player.GetComponent<Particle3D>();
        StartCoroutine(ConductThrowingSequence());
    }





    // This function is called whenever the state is exited
    public override void OnExitState()
    {
        isReadyToTransition = false;
        isHoldingPlayer = false;
    }





    // This function is called once per frame to update the object based on the state behaviours
    public override void UpdateState()
    {
        // Is the king currently holding the player?
        if (isHoldingPlayer)
        {
            // If yes, then set the player's position to the king's hands
            player.GetComponent<Particle3D>().position = transform.parent.position + transform.InverseTransformDirection(playerHoldingOffset);
        }
    }





    // This function is called to conduct the sequence of events to throw the player
    IEnumerator ConductThrowingSequence()
    {
        // Wait to throw player
        yield return new WaitForSeconds(durationToThrow);

        // Throw player and wait
        isHoldingPlayer = false;
        playerParticle.AddForce(playerParticle.mass * transform.forward * horizontalThrowingDistance);
        playerParticle.AddForce(playerParticle.mass * transform.up * verticalThrowingDistance);
        yield return new WaitForSeconds(durationToTransition);

        // Transition out of throwing sequence
        isReadyToTransition = true;
    }
}
