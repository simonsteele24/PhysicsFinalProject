using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chainchomp : MonoBehaviour
{
    public float movementSpeed = 1;
    public float lungeMovementSpeed = 10;
    public float distanceToMoveFromPole = 10;
    public float raycastCheckHit = 1;
    public float movementCheckRaycatHit = 3;
    public bool isGrounded = true;
    public GameObject pole;

    public void MoveInADirection(Vector3 direction)
    {
        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * direction * movementSpeed);
    }

    public void MoveInALungeDirection(Vector3 direction)
    {
        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * direction * lungeMovementSpeed);
    }

    private void Update()
    {
        if (Mathf.Abs(Vector3.Distance(transform.position, pole.transform.position)) > distanceToMoveFromPole)
        {
            Vector3 direction = (pole.transform.position - transform.position).normalized;
            Vector3 newPos = transform.position * (distanceToMoveFromPole - Mathf.Abs(Vector3.Distance(transform.position, pole.transform.position)));
            transform.position = newPos;
            GetComponent<Particle3D>().position = transform.position;
            GetComponent<Particle3D>().velocity = Vector3.zero;
        }

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
}
