using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour
{
    public Animator goombaAnimator;
    public Transform goombaTorsoPosition;
    public float walkSpeedAnimationMultiplier = 10.0f;
    public float sprintSpeedMultiplier = 2.0f;
    public float attackRadius = 2.0f;
    public float movementSpeed;
    public float raycastCheckHit = 1;
    public float movementCheckRaycatHit = 3;
    public float deathLifeTime = 2;
    public bool isGrounded = true;
    public bool isDying = false;
    public bool isChasing = false;

    public void MoveInADirection(Vector3 direction)
    {
        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * direction * movementSpeed);
        //transform.localEulerAngles = new Vector3(0, (Mathf.Atan2(direction.x, direction.z) * (180 / Mathf.PI)), 0);
    }

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
        
        RaycastHit hit;

        // See if player is colliding with ground
        if (Physics.Raycast(goombaTorsoPosition.position, Vector3.down, out hit, raycastCheckHit))
        {

            // Make sure that we set velocity to zero if the force of gravity is being applied
            if (GetComponent<Particle3D>().velocity.y < 0 && GetComponent<Particle3D>().velocity.y != 0)
            {
                GetComponent<Particle3D>().velocity.y = 0;
            }

            GetComponent<Particle3D>().position.y = hit.point.y;
            GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;

            // Set all values so player sticks to ground
            //GetComponent<Particle3D>().isUsingGravity = false;
            GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;
            isGrounded = true;
        }
        else
        {
            GetComponent<Particle3D>().collidingGameObject = null;
            // If in air, set all gravity values
            isGrounded = false;
        }

        if (Physics.Raycast(goombaTorsoPosition.position, transform.forward, out hit, attackRadius))
        {
            if (hit.collider.tag == "Player")
            {
                hit.collider.GetComponent<PlayerController>().DamagePlayer(hit.point);
            }
        }

        if (Physics.Raycast(transform.position, new Vector3(GetComponent<Particle3D>().velocity.normalized.x, 0, GetComponent<Particle3D>().velocity.normalized.z), out hit, movementCheckRaycatHit))
        {
            if (hit.collider.gameObject != GetComponent<Particle3D>().collidingGameObject)
            {
                GetComponent<Particle3D>().velocity.z = 0;
                GetComponent<Particle3D>().velocity.x = 0;
                GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;
            }
        }
    }

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
