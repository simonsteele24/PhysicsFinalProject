using UnityEngine;

public class CompleteIdleState : State
{
    // Floats
    public float distanceToLungeAtPlayer = 5;

    // Game Objects
    GameObject player;





    // This function is called once per frame to check for a transition
    public override States CheckForTransition()
    {
        // Is the distance close enough to lunge at the player
        float distance = Mathf.Abs(Vector3.Distance(transform.parent.position, player.transform.position));
        if (distance < distanceToLungeAtPlayer)
        {
            // If so, then lunge at the player
            return States.Lunge;
        }

        // If all else fails, stay completely idle
        return States.CompleteIdle;
    }





    // This function gets called whenever the state is entered
    public override void OnEnterState()
    {
        player = GameObject.Find("Player");
    }





    public override void OnExitState() { }





    // This function gets called once per frame and moves the object based on the state behaviour
    public override void UpdateState()
    {
        Vector3 position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.parent.LookAt(position);
    }
}
