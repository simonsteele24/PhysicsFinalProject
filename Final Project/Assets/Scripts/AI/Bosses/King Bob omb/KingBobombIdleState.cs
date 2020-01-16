using UnityEngine;

public class KingBobombIdleState : State
{
    // Gameobjects
    GameObject player;





    // This function checks for any transitions between states
    public override States CheckForTransition()
    {
        // Is the player currently on the boss plane?
        if (player.GetComponent<Particle3D>().collidingGameObject == GetComponentInParent<KingBobomb>().bossPlane)
        {
            // If yes, then start chasing them
            return States.Chase;
        }

        // Otherwise, keep being idle
        return States.Idle;
    }





    // This function is triggered whenever the state is entered
    public override void OnEnterState()
    {
        player = GameObject.Find("Player");
    }

    public override void OnExitState() { }

    public override void UpdateState() { }
}
