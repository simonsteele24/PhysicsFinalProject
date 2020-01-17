using System.Collections;
using UnityEngine;

public class Goomba : MonoBehaviour
{
    // Animator
    public Animator goombaAnimator;

    // Transforms
    public Transform goombaTorsoPosition;

    // Raycast hits
    RaycastHit hit;

    // Floats
    public float walkSpeedAnimationMultiplier = 10.0f;
    public float sprintSpeedMultiplier = 2.0f;
    public float attackRadius = 2.0f;
    public float movementSpeed;
    public float raycastCheckHit = 1;
    public float movementCheckRaycatHit = 3;
    public float deathLifeTime = 2;

    // Booleans
    bool isGrounded = true;
    bool isDying = false;
    bool isChasing = false;
    bool isHopping = false;





    // Accesors
    public bool GetGroundedState() { return isGrounded; }
    public bool GetHoppingState() { return isHopping; }
    public bool GetChasingState() { return isChasing; }
    public bool GetDyingState() { return isDying; }

    // Mutators
    public void SetGroundedState(bool _isGrounded) { isGrounded = _isGrounded; }
    public void SetHoppingState(bool _isHopping) { isHopping = _isHopping; }
    public void SetChasingState(bool _isChasing) { isChasing = _isChasing; }
    public void SetDyingState(bool _isDying) { isDying = _isDying; }





    // This function adds a force to the Goomba in a given direction
    public void MoveInADirection(Vector3 direction)
    {
        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * direction * movementSpeed);
    }





    // This function adds a given sprint force to the Goomba in a given direction
    public void SprintInADirection(Vector3 direction, float sprintSpeed)
    {
        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * direction * sprintSpeed);
    }





    private void Update()
    {
        if (isChasing)
        {
            goombaAnimator.SetFloat("AnimWalkSpeed", new Vector3(GetComponent<Particle3D>().velocity.x, 0, GetComponent<Particle3D>().velocity.z).magnitude * sprintSpeedMultiplier);
        }
        else
        {
            goombaAnimator.SetFloat("AnimWalkSpeed", new Vector3(GetComponent<Particle3D>().velocity.x, 0, GetComponent<Particle3D>().velocity.z).magnitude * walkSpeedAnimationMultiplier);
        }

        // See if player is colliding with ground
        if (Physics.Raycast(goombaTorsoPosition.position, Vector3.down, out hit, raycastCheckHit) && (!isHopping || GetComponent<Particle3D>().velocity.y < 0))
        {
            GetComponent<Particle3D>().position.y = hit.point.y;
            GetComponent<Particle3D>().velocity.y = 0;
            GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;
            isGrounded = true;
        }
        else
        {
            GetComponent<Particle3D>().collidingGameObject = null;
            // If in air, set all gravity values
            isGrounded = false;
        }


        // Is the goomba colliding with the player?
        if (Physics.Raycast(goombaTorsoPosition.position, transform.forward, out hit, attackRadius))
        {
            if (hit.collider.tag == "Player")
            {
                // If yes, then add damage to the player
                hit.collider.GetComponent<PlayerController>().DamagePlayer(hit.point);
            }
        }

        // Has the Goomba collided with the wall?
        if (Physics.Raycast(transform.position, new Vector3(GetComponent<Particle3D>().velocity.normalized.x, 0, GetComponent<Particle3D>().velocity.normalized.z), out hit, movementCheckRaycatHit))
        {
            // If so, has the goomba already collided with said wall?
            if (hit.collider.gameObject != GetComponent<Particle3D>().collidingGameObject)
            {
                // If no, then stop the goomba from entering the wall
                GetComponent<Particle3D>().velocity.z = 0;
                GetComponent<Particle3D>().velocity.x = 0;
                GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;
            }
        }
    }





    // This function waits a certain amount of time and then destroys the goomba
    public IEnumerator CommenceDeath()
    {
        isDying = true;
        yield return new WaitForSeconds(deathLifeTime);

        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
