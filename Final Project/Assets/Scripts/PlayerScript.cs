using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float jumpForce;
    public float movementSpeed = 1;
    public float raycastCheckHit = 1;
    public float movementCheckRaycatHit = 3;
    public bool isAttemptingToJump = false;
    public bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPhysicsChange();
        CheckForInput();
    }





    // This function checks for all input values and does all appropriate calculations
    void CheckForInput()
    {
        RaycastHit hit;

        // Get input
        float inputAmountX = Mathf.Abs(Input.GetAxis("Horizontal"));
        float inputAmountY = Mathf.Abs(Input.GetAxis("Vertical"));

        // Set all animations
        GetComponentInChildren<Animator>().SetFloat("Forward", Mathf.Clamp(inputAmountX + inputAmountY, 0, 1), 0.1f, Time.deltaTime);
        GetComponentInChildren<Animator>().SetBool("OnGround", isGrounded);
        GetComponentInChildren<Animator>().SetFloat("Jump", GetComponent<Particle3D>().velocity.y);

        // Check if there is anything in front of player that will prevent movement
        bool hasBeenHit = !Physics.Raycast(transform.position, Vector3.forward, out hit, movementCheckRaycatHit);
        if (!hasBeenHit && GetComponent<Particle3D>().collidingGameObject != null)
        {
            if (hit.collider.gameObject == GetComponent<Particle3D>().collidingGameObject)
            {
                hasBeenHit = true;
            }
        }

        // Move if nothing is in the way of the player
        if (Input.GetKey(KeyCode.W) && !Physics.Raycast(transform.position, Vector3.forward, out hit, movementCheckRaycatHit))
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * movementSpeed * GetComponent<Particle3D>().GetForwardVector());
            transform.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
            GetComponent<Particle3D>().isAttemptingToMove = true;
        }

        // Check if there is anything on the left of player that will prevent movement
        hasBeenHit = !Physics.Raycast(transform.position, Vector3.left, out hit, movementCheckRaycatHit);
        if (!hasBeenHit && GetComponent<Particle3D>().collidingGameObject != null)
        {
            if (hit.collider.gameObject == GetComponent<Particle3D>().collidingGameObject)
            {
                hasBeenHit = true;
            }
        }

        // Move if nothing is in the way of the player
        if (Input.GetKey(KeyCode.A) && !Physics.Raycast(transform.position, Vector3.left, out hit, movementCheckRaycatHit))
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * movementSpeed * -GetComponent<Particle3D>().GetRightwardVector());
            transform.GetChild(0).localEulerAngles = new Vector3(0, 270, 0);
            GetComponent<Particle3D>().isAttemptingToMove = true;
        }

        // Check if there is anything in back of player that will prevent movement
        hasBeenHit = !Physics.Raycast(transform.position, Vector3.back, out hit, movementCheckRaycatHit);
        if (!hasBeenHit && GetComponent<Particle3D>().collidingGameObject != null)
        {
            if (hit.collider.gameObject == GetComponent<Particle3D>().collidingGameObject)
            {
                hasBeenHit = true;
            }
        }

        // Move if nothing is in the way of the player
        if (Input.GetKey(KeyCode.S) && !Physics.Raycast(transform.position, Vector3.back, out hit, movementCheckRaycatHit))
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * movementSpeed * -GetComponent<Particle3D>().GetForwardVector());
            transform.GetChild(0).localEulerAngles = new Vector3(0, 180, 0);
            GetComponent<Particle3D>().isAttemptingToMove = true;
        }

        // Check if there is anything on the right of player that will prevent movement
        hasBeenHit = !Physics.Raycast(transform.position, Vector3.right, out hit, movementCheckRaycatHit);
        if (!hasBeenHit && GetComponent<Particle3D>().collidingGameObject != null)
        {
            if (hit.collider.gameObject == GetComponent<Particle3D>().collidingGameObject)
            {
                hasBeenHit = true;
            }
        }

        // Move if nothing is in the way of the player
        if (Input.GetKey(KeyCode.D) && hasBeenHit)
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * movementSpeed * GetComponent<Particle3D>().GetRightwardVector());
            transform.GetChild(0).localEulerAngles = new Vector3(0, 90, 0);
            GetComponent<Particle3D>().isAttemptingToMove = true;
        }

        // Jump if the character is grounded
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * Vector3.up * jumpForce);
            GetComponent<Particle3D>().position.y += 0.5f;
            isAttemptingToJump = true;
        }
    }




    // This function checks all physics based values
    void CheckForPhysicsChange()
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
            isAttemptingToJump = false;
            GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;
            isGrounded = true;
        }
        else
        {
            // If in air, set all gravity values
            GetComponent<Particle3D>().isUsingGravity = true;
            GetComponent<Particle3D>().collidingGameObject = null;
            isGrounded = false;
        }

        // Check if the colliding object is moving, if so then move with it
        if (GetComponent<Particle3D>().collidingGameObject != null)
        {
            if (GetComponent<Particle3D>().collidingGameObject.gameObject.tag == "Dynamic Object" && !isAttemptingToJump)
            {
                GetComponent<Particle3D>().velocity.y = GetComponent<Particle3D>().collidingGameObject.GetComponent<Particle3D>().velocity.y;
            }
        }
    }
}
