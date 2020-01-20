using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chainchomp : MonoBehaviour
{
    // Floats
    public float movementSpeed = 1;
    public float lungeMovementSpeed = 10;
    public float distanceToMoveFromPole = 10;
    public float distanceToStop;
    public float raycastCheckHit = 1;
    public float movementCheckRaycatHit = 3;
    public float attackRadius = 2;
    public float hitRadius = 1.5f;

    // Booleans
    public bool isGrounded = true;
    public bool isAttacking = false;

    // Gameobjects
    public GameObject pole;

    // Raycast hits
    RaycastHit hit;





    // This function adds a force to the object in a given direction
    public void MoveInADirection(Vector3 direction)
    {
        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * direction * movementSpeed);
    }





    // This function adds a lunging force in a given direction
    public void MoveInALungeDirection(Vector3 direction)
    {
        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * direction * lungeMovementSpeed);
    }





    private void Update()
    {
        Vector3 direction;

        // Is the Chainchomp outside of the range of the chain?
        if (Mathf.Abs(Vector3.Distance(transform.position, pole.transform.position)) > distanceToMoveFromPole)
        {
            // If so, move them back into chain range
            direction = (pole.transform.position - transform.position).normalized;
            Vector3 newPos = transform.position * (distanceToMoveFromPole - Mathf.Abs(Vector3.Distance(transform.position, pole.transform.position)));
            transform.position = newPos;
            GetComponent<Particle3D>().position = transform.position;
            GetComponent<Particle3D>().velocity = Vector3.zero;
        }

        direction = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;

        if (Physics.Raycast(transform.position, direction.normalized, out hit, hitRadius))
        {
            if (hit.collider.tag == "Player")
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().DamagePlayer(transform.position);
            }
        }

        // Is the player within attack radius and hittable?
        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRadius))
        {
            if (hit.collider.tag == "Player")
            {
                // If yes, then damage the player
                hit.collider.GetComponent<PlayerController>().DamagePlayer(hit.point);
            }
        }

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
            GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;
        }
        else
        {
            // If in air, set all gravity values
            GetComponent<Particle3D>().collidingGameObject = null;
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
    }
}
