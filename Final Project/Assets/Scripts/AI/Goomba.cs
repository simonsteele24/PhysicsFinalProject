using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour
{
    public float movementSpeed;
    public float raycastCheckHit = 1;
    public float movementCheckRaycatHit = 3;
    public float deathLifeTime = 2;
    public bool isGrounded = true;
    public bool isDying = false;

    public void MoveInADirection(Vector3 direction)
    {
        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * direction * movementSpeed);
    }

    private void Update()
    {
        RaycastHit hit;

        // See if player is colliding with ground
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastCheckHit))
        {
            // Make sure that we set velocity to zero if the force of gravity is being applied
            if (GetComponent<Particle3D>().velocity.y < 0 && GetComponent<Particle3D>().velocity.y != 0)
            {
                GetComponent<Particle3D>().velocity.y = 0;
            }

            // Set all values so player sticks to ground
            GetComponent<Particle3D>().isUsingGravity = false;
        }
        else
        {
            // If in air, set all gravity values
            GetComponent<Particle3D>().isUsingGravity = true;
            isGrounded = false;
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

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2.1f))
        {
            GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;
        }
        else
        {
            GetComponent<Particle3D>().collidingGameObject = null;
        }
    }

    public IEnumerator CommenceDeath()
    {
        isDying = true;
        yield return new WaitForSeconds(deathLifeTime);
        Destroy(gameObject);
    }
}
