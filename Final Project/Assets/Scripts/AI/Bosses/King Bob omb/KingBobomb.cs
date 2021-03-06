﻿using UnityEngine;
using System.Collections;

public class KingBobomb : MonoBehaviour
{
    // Floats
    public float movementSpeed;
    public float raycastCheckHit = 1;
    public float movementCheckRaycatHit = 3;
    public float health = 3;
    public float timeToWaitUntilDeath = 3;

    // Booleans
    public bool isGrounded = true;
    public bool isProne = false;
    public bool isDying = false;

    // Gameobjects
    public GameObject bossPlane;
    public GameObject star;

    // Animation Controller
    public Animator bossAnimator;
    public Vector3 bossPlaneAxis;

    // Raycast hits
    RaycastHit hit;





    // Accessor functions
    public bool CheckIfGrounded() { return isGrounded; }
    public bool CheckIfProne() { return isProne; }

    // Mutator functions
    public void SetIsGrounded (bool _isGrounded) { isGrounded = _isGrounded; }
    public void SetIsProne (bool _isProne) { isProne = _isProne; }



    // This function adds a force to the object in a given direction
    public void MoveInADirection(Vector3 direction)
    {
        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * direction * movementSpeed);
    }





    private void Update()
    {
        bossAnimator.SetBool("isProne", isProne);

        if (transform.position.y < bossPlaneAxis.y)
        {
            Debug.Log("Here");
            
            GetComponent<Particle3D>().position = bossPlaneAxis + Vector3.up * 4;
            transform.position = GetComponent<Particle3D>().position;
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
            GetComponent<Particle3D>().position.y = hit.point.y + 0.1f;
            GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;
            isGrounded = true;

            // Is the bobomb prone?
            if (isProne && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().carryingObject == null && !isDying)
            {
                // If so, then damage him
                isProne = false;
                DamageKingBobomb();
            }
            
        }
        else
        {
            // If in air, set all gravity values
            GetComponent<Particle3D>().isUsingGravity = true;
            isGrounded = false;
            GetComponent<Particle3D>().collidingGameObject = null;
        }


        // Is the king moving into a wall?
        if (Physics.Raycast(transform.position, new Vector3(GetComponent<Particle3D>().velocity.normalized.x, 0, GetComponent<Particle3D>().velocity.normalized.z), out hit, movementCheckRaycatHit))
        {
            // If yes, is this a wall that he is already colliding with?
            if (hit.collider.gameObject != GetComponent<Particle3D>().collidingGameObject)
            {
                // If no, then set velocity to 0
                GetComponent<Particle3D>().velocity.z = 0;
                GetComponent<Particle3D>().velocity.x = 0;
                GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;
            }
        }
    }





    // This function damages the king bobomb and determines if he needs to be destroyed
    void DamageKingBobomb()
    {
        health--;
        if (health == 0)
        {
            star.SetActive(true);
            StartCoroutine(WaitToDestroyBobomb());
        }
    }





    IEnumerator WaitToDestroyBobomb()
    {
        isDying = true;
        isProne = true;
        yield return new WaitForSeconds(timeToWaitUntilDeath);
        Destroy(gameObject);
    }
}
