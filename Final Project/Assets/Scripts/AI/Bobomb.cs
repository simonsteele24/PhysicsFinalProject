using System.Collections;
using UnityEngine;

public class Bobomb : MonoBehaviour
{
    // Particle system
    public ParticleSystem smoke;

    // Transforms
    public Transform bobombTorsoPosition;
    public Transform bobombBottom;

    // Animator
    public Animator bobombAnimator;

    // Floats
    public float walkSpeedAnimationMultiplier = 5.0f;
    public float sprintSpeedMultiplier = 2.0f;
    public float movementSpeed;
    public float raycastCheckHit = 1;
    public float movementCheckRaycatHit = 3;
    public float deathLifeTime = 2;
    public float distanceToHurtPlayer = 3;

    // Booleans
    bool isGrounded = true;
    bool isDying = false;
    bool isChasing = false;

    // Raycast hits
    RaycastHit hit;






    private void Start()
    {
        smoke.Stop();
    }





    // This function moves the object in a given direction
    public void MoveInADirection(Vector3 direction)
    {
        smoke.Stop();
        bobombAnimator.SetFloat("AnimWalkSpeed", new Vector3(GetComponent<Particle3D>().velocity.x, 0, GetComponent<Particle3D>().velocity.z).magnitude * walkSpeedAnimationMultiplier);
        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * direction * movementSpeed);
    }





    // This function moves the in a given direction with a given sprint speed
    public void SprintInADirection(Vector3 direction, float sprintSpeed)
    {
        smoke.Play();
        bobombAnimator.SetFloat("AnimWalkSpeed", new Vector3(GetComponent<Particle3D>().velocity.x, 0, GetComponent<Particle3D>().velocity.z).magnitude * sprintSpeedMultiplier);
        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * direction * sprintSpeed);
    }





    private void Update()
    {
        // See if player is colliding with ground
        if (Physics.Raycast(bobombTorsoPosition.position, Vector3.down, out hit, raycastCheckHit))
        {

            // Make sure that we set velocity to zero if the force of gravity is being applied
            if (GetComponent<Particle3D>().velocity.y < 0 && GetComponent<Particle3D>().velocity.y != 0)
            {
                GetComponent<Particle3D>().velocity.y = 0;
            }

            // Move position to be above plane
            GetComponent<Particle3D>().position.y = hit.point.y;

            // Set all values so player sticks to ground
            GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;
            isGrounded = true;
        }
        else
        {
            // If in air, set all gravity values
            GetComponent<Particle3D>().collidingGameObject = null;
            isGrounded = false;
        }





        // Check if bobomb is moving into a wall
        if (Physics.Raycast(transform.position, new Vector3(GetComponent<Particle3D>().velocity.normalized.x, 0, GetComponent<Particle3D>().velocity.normalized.z), out hit, movementCheckRaycatHit))
        {
            // Is this something that is already being collided into?
            if (hit.collider.gameObject != GetComponent<Particle3D>().collidingGameObject)
            {
                // Is yes, then stop the bobomb
                GetComponent<Particle3D>().velocity.z = 0;
                GetComponent<Particle3D>().velocity.x = 0;
                GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;
            }
        }
    }





    // This function triggers whenever the bobomb explodes
    public void Explode()
    {
        // Is the explosion close enough to the player to hurt them?
        if (Vector3.Distance(GetComponentInChildren<BobombChaseState>().player.transform.position, transform.position) < distanceToHurtPlayer)
        {
            // If yes, then inflict damage onto the player
            GetComponentInChildren<BobombChaseState>().player.GetComponent<PlayerController>().DamagePlayer(transform.position);
        }
    }
}
