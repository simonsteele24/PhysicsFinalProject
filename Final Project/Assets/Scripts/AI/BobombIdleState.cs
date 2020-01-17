using UnityEngine;

public class BobombIdleState : State
{
    // Floats
    public float rotationSpeed;
    public float distanceToChasePlayer = 5;

    // Gameobjects
    GameObject player;





    // This function checks for a state transition once per frame
    public override States CheckForTransition()
    {
        // Calculate the distance to the player
        float distance = Mathf.Abs(Vector3.Distance(transform.parent.position, player.transform.position));

        // Is the player within sight range?
        if (distance < distanceToChasePlayer)
        {
            // If yes then check if player is within line of sight
            Vector3 direction = player.transform.position - transform.parent.position;
            float orientation = Vector3.Dot(direction, transform.parent.forward);

            if (orientation > 0)
            {
                // Is so, then chase the player
                return States.Chase;
            }
        }

        // If all else fails, then do nothing
        return States.Idle;
    }





    // This function triggers whenever this state is entered
    public override void OnEnterState()
    {
        player = GameObject.Find("Player");
    }

    public override void OnExitState() { }





    // This function is called once per frame to update the object based on the state behaviour
    public override void UpdateState()
    {
        // Move around in a cirlce
        transform.parent.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        transform.parent.GetComponent<Particle3D>().rotation = transform.parent.rotation;
        GetComponentInParent<Bobomb>().MoveInADirection(transform.parent.forward);
    }
}
