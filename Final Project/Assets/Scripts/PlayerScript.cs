using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    // Floats
    public float jumpForce;
    public float angleThreshold = 40;
    public float movementSpeed = 1;
    public float jumpMovementSpeed = 1;
    public float raycastCheckHit = 1;
    public float movementCheckRaycatHit = 3;
    public float strongerJumpCooldown = 1;
    public float punchCooldown = 1;
    public float lastDirectionX;
    public float lastDirectionY;
    public float punchDistance;
    public float throwingOffset = 20;
    public float wallJumpForce = 250;
    public float forceOfKnockbackHits = 20;
    float originalGravitationalConstant;
    float inputAmountX;
    float inputAmountY;

    // Integers
    public int strongJumpMaxIndex = 4;
    public int groundPoundGravityMultiplier = 2;
    public int strongerJumpKey = 1;

    // Booleans
    public bool isAttemptingToJump = false;
    public bool isGrounded = true;
    bool isGroundPounding = false;
    bool canDoStrongerJump = false;
    bool airTriggeredByJump = false;
    bool canPunch = true;
    bool isTriggerDown = false;
    bool isSliding = false;
    public bool canWallJump = false;

    // GameObjects
    GameObject carryingObject;

    // Transforms
    public Transform playerTorsoTransform;
    public Animator animator;

    // Vector3's
    public Vector3 carryingOffset;
    Vector3 collidingPoint;
    

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TakeLastInput());
        originalGravitationalConstant = GetComponent<Particle3D>().gravitationalConstant;
    }

    // Update is called once per frame
    void Update()
    {
        // Update Physics
        CheckForPhysicsChange();

        // Check for Player Input
        CheckForInput();
    }

    // This function checks for all input values and does all appropriate calculations
    void CheckForInput()
    {
        RaycastHit hit;
        inputAmountY = Input.GetAxis("Xbox_LeftStick_X");
        inputAmountX = -Input.GetAxis("Xbox_LeftStick_Y");

        // Create sprint amount
        float sprintAmount = isGrounded ? sprintAmount = 1 + Input.GetAxis("Xbox_RT") : sprintAmount = 1;

        // Set all animations
        animator.SetFloat("Forward", Mathf.Clamp(Mathf.Abs(inputAmountX) + Mathf.Abs(inputAmountY), 0, 1), 0.1f, Time.deltaTime);
        animator.SetBool("isGrounded", isGrounded || (!isAttemptingToJump && (GetComponent<Particle3D>().collidingGameObject != null && !canWallJump)));
        animator.SetBool("isCrouching", isTriggerDown);
        GetComponentInChildren<Animator>().SetFloat("Jump", GetComponent<Particle3D>().velocity.y);

        if (isSliding)
        {
            GetComponent<Particle3D>().AddForce(ForceGenerator.GenerateForce_sliding(new Vector3(0, GetComponent<Particle3D>().gravitationalConstant, 0), GetComponent<Particle3D>().collidingGameObject.transform.right * 20));
        }

        // Move if nothing is in the way of the player
        if (!Physics.Raycast(playerTorsoTransform.position, new Vector3(inputAmountX, 0, inputAmountY).normalized, out hit, movementCheckRaycatHit))
        {
            if (!isSliding)
            {
                // Is the player grounded?
                if (isGrounded)
                {
                    // If yes, then move the player in the direction given by controller input
                    GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * movementSpeed * sprintAmount * Camera.main.transform.TransformDirection(new Vector3(-inputAmountY, 0, -inputAmountX).normalized) /*GetComponent<Particle3D>().GetForwardVector()*/);
                }
                else
                {
                    // If no, then move the player in the direction given by controller input but directed by a jump speed
                    GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * jumpMovementSpeed * Camera.main.transform.TransformDirection(new Vector3(-inputAmountY, 0, -inputAmountX).normalized) /*GetComponent<Particle3D>().GetForwardVector()*/);
                }

                // Has the player put in any sort of input?
                if (inputAmountX != 0 || inputAmountY != 0)
                {
                    // If yes, then rotate the object appropriatly
                    transform.GetChild(0).localEulerAngles = new Vector3(0, (Mathf.Atan2(inputAmountX, -inputAmountY) * (180 / Mathf.PI)) + (Camera.main.transform.localEulerAngles.y + 90), 0);
                }
            }
            else
            {
                // If yes, then move the player in the direction given by controller input
                GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * movementSpeed * sprintAmount * GetComponent<Particle3D>().collidingGameObject.transform.TransformDirection(new Vector3(0, 0, inputAmountY).normalized) /*GetComponent<Particle3D>().GetForwardVector()*/);

                // Has the player put in any sort of input?
                if (inputAmountY != 0)
                {
                    // If yes, then rotate the object appropriatly
                    transform.GetChild(0).localEulerAngles = new Vector3(0, (Mathf.Atan2(-inputAmountY,0) * (180 / Mathf.PI)) + (GetComponent<Particle3D>().collidingGameObject.transform.localEulerAngles.y + 90), 0);
                }
            }

            GetComponent<Particle3D>().isAttemptingToMove = true;
        }

        // Is the player pressing the punch button and are they allowed to punch?
        if (Input.GetButtonDown("Xbox_B") && canPunch)
        {
            Debug.Log("Punch");
            animator.SetBool("isPunching", true);

            // If yes, is the player carrying anything?
            if (carryingObject != null)
            {
                // If yes, then throw whatever the player is carrying
                carryingObject.GetComponent<Particle3D>().AddForce(carryingObject.GetComponent<Particle3D>().mass * transform.forward * throwingOffset);
                carryingObject.GetComponent<Particle3D>().AddForce(carryingObject.GetComponent<Particle3D>().mass * transform.up * throwingOffset);
                carryingObject = null;
            }


            // Is there something in front of the player to punch at?
            if (Physics.Raycast(playerTorsoTransform.position, transform.GetChild(0).transform.forward, out hit, movementCheckRaycatHit))
            {
                // If yes, is it a king Bob omb?
                if (hit.collider.gameObject.tag == "King Bobomb" && Vector3.Distance(transform.position, hit.collider.transform.position) < punchDistance)
                {
                    // If yes, then pick it up
                    carryingObject = hit.collider.gameObject;
                    hit.collider.gameObject.GetComponent<KingBobomb>().isProne = true;
                }

                // If yes, is it a destroyable object?
                if (hit.collider.gameObject.tag == "Destroyable" && Vector3.Distance(transform.position, hit.collider.transform.position) < punchDistance)
                {
                    // If yes, then destroy that object
                    Destroy(hit.collider.gameObject);
                }
            }

            // Start the punch cooldown afterwards
            StartCoroutine(StartPunchCooldown());
        }


        // Is the player pressing the left trigger down?
        if (Input.GetAxis("Xbox_LT") > 0)
        {
            // If yes, has the trigger already been down?
            if (!isTriggerDown)
            {
                isTriggerDown = true;

                // If yes, is the A button down as well and is the player grounded?
                if (Input.GetButtonDown("Xbox_A") && isGrounded)
                {
                    // If yes, has the player put any sort of effort into moving?
                    if (Mathf.Abs(inputAmountX) > 0 || Mathf.Abs(inputAmountY) > 0)
                    {
                        // If yes, then perform a long jump
                        Debug.Log("Long Jump");
                        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * 50 * Vector3.up * strongJumpMaxIndex);
                        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * 500 * transform.GetChild(0).forward);
                        GetComponent<Particle3D>().position.y += 0.5f;

                        isAttemptingToJump = true;
                        airTriggeredByJump = true;
                        canDoStrongerJump = false;
                    }
                    else
                    {
                        // If no, then perform a backflip
                        Debug.Log("Backflip");
                        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * 500 * Vector3.up * strongJumpMaxIndex);
                        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * 50 * -transform.GetChild(0).forward);
                        GetComponent<Particle3D>().position.y += 0.5f;

                        isAttemptingToJump = true;
                        airTriggeredByJump = true;
                        canDoStrongerJump = false;
                    }
                }

                // if no, is the player in the air?
                else if (!isGrounded)
                {
                    // If yes, then perform a ground pound
                    Debug.Log("Ground pound");
                    isGroundPounding = true; 
                }
                else
                {
                    // If no, then simply perform a crouch
                    Debug.Log("Crouch");
                }
            }
        }
        else
        {
            isTriggerDown = false;
            // Is the A button down?
            if (Input.GetButtonDown("Xbox_A"))
            {
                // If yes, are they grounded?
                if (isGrounded)
                {
                    // If yes, then check if they have done any sudden movements and then opposite movements and then perform a backward sommersault if applicable
                    if (inputAmountX < 0)
                    {
                        if (inputAmountX < lastDirectionX && lastDirectionX > 0)
                        {
                            Debug.Log("Backward Somersault");
                            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * 50 * Vector3.up * strongJumpMaxIndex);
                            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * 50 * transform.forward);
                            GetComponent<Particle3D>().position.y += 0.5f;

                            isAttemptingToJump = true;
                            airTriggeredByJump = true;
                            canDoStrongerJump = false;
                        }
                    }
                    if (inputAmountX > 0)
                    {
                        if (inputAmountX > lastDirectionX && lastDirectionX < 0)
                        {
                            Debug.Log("Backward Somersault");
                            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * 50 * Vector3.up * strongJumpMaxIndex);
                            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * 50 * transform.forward);
                            GetComponent<Particle3D>().position.y += 0.5f;

                            isAttemptingToJump = true;
                            airTriggeredByJump = true;
                            canDoStrongerJump = false;
                        }
                    }
                    if (inputAmountY > 0)
                    {
                        if (inputAmountY > lastDirectionY && lastDirectionY < 0)
                        {
                            Debug.Log("Backward Somersault");
                            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * 50 * Vector3.up * strongJumpMaxIndex);
                            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * 50 * transform.forward);
                            GetComponent<Particle3D>().position.y += 0.5f;

                            isAttemptingToJump = true;
                            airTriggeredByJump = true;
                            canDoStrongerJump = false;
                        }
                    }
                    if (inputAmountY < 0)
                    {
                        if (inputAmountY < lastDirectionY && lastDirectionY > 0)
                        {
                            Debug.Log("Backward Somersault");
                            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * 50 * Vector3.up * strongJumpMaxIndex);
                            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * 50 * transform.forward);
                            GetComponent<Particle3D>().position.y += 0.5f;

                            isAttemptingToJump = true;
                            airTriggeredByJump = true;
                            canDoStrongerJump = false;
                        }
                    }

                    // If no to all, can they do a stronger jump?
                    if (canDoStrongerJump && strongerJumpKey < strongJumpMaxIndex)
                    {
                        // If yes, then perform a stronger jump
                        strongerJumpKey++;
                        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * Vector3.up * strongerJumpKey * jumpForce);
                        GetComponent<Particle3D>().position.y += 0.5f;
                    }
                    else
                    {
                        // If no, then perform just a regular jump
                        strongerJumpKey = 0;
                        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * Vector3.up * jumpForce);
                        GetComponent<Particle3D>().position.y += 0.5f;
                    }
                    isAttemptingToJump = true;
                    airTriggeredByJump = true;
                    canDoStrongerJump = false;
                }
                // If all else fails, can the player do a wall jump?
                else if (canWallJump)
                {
                    // If yes, then calculate the normal between the two points and jump accordingly
                    Vector3 normal = transform.position - collidingPoint;
                    GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * wallJumpForce * new Vector3(normal.normalized.x, 0, normal.normalized.z) * strongJumpMaxIndex);
                    GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * wallJumpForce * Vector3.up * strongJumpMaxIndex);
                    transform.GetChild(0).transform.rotation = Quaternion.Euler(transform.GetChild(0).transform.eulerAngles.x, transform.GetChild(0).transform.eulerAngles.y + 180, transform.GetChild(0).transform.eulerAngles.z);
                }
            }
        }
    }




    // This function checks all physics based values
    void CheckForPhysicsChange()
    {
        isSliding = false;

        // Is the player carrying an object?
        if (carryingObject != null)
        {
            // If yes, then change the position of the carrying object to wherever the player is
            carryingObject.GetComponent<Particle3D>().position = transform.position + transform.InverseTransformDirection(carryingOffset);
        }

        RaycastHit hit;

        // Is the player ground pounding?
        if (isGroundPounding)
        {
            // If yes, then increase vertical velocity
            GetComponent<Particle3D>().gravitationalConstant = originalGravitationalConstant * groundPoundGravityMultiplier;
        }
        else
        {
            // If no, then keep regular vertical velocity
            GetComponent<Particle3D>().gravitationalConstant = originalGravitationalConstant;
        }

        // Check if the colliding object is moving, if so then move with it
        if (GetComponent<Particle3D>().collidingGameObject != null)
        {
            if (GetComponent<Particle3D>().collidingGameObject.gameObject.tag == "Dynamic Object" && !isAttemptingToJump)
            {
                GetComponent<Particle3D>().velocity.y = GetComponent<Particle3D>().collidingGameObject.GetComponent<Particle3D>().velocity.y;
            }
        }

        // See if player is colliding with ground
        if (Physics.Raycast(playerTorsoTransform.transform.position, Vector3.down, out hit, raycastCheckHit) && (!isAttemptingToJump || GetComponent<Particle3D>().velocity.y < 0))
        {
            if (hit.collider.gameObject.tag == "Slideable")
            {
                isSliding = true;
            }

            // Is the colliding ground a tilting bridge?
            if (hit.collider.gameObject.tag == "TiltingBridge")
            {
                // If yes, then apply a rotational force to the bridge
                hit.collider.GetComponent<Particle3D>().ApplyTorque(new Vector3(0, 0, -hit.collider.GetComponent<Particle3D>().invTransformMatrix.MultiplyPoint(new Vector3(hit.point.x, hit.collider.transform.position.y, hit.point.z)).x * GetComponent<Particle3D>().mass * Time.deltaTime));
            }

            // Is the colliding ground a destroyable block and is the player ground pounding?
            if (hit.collider.gameObject.tag == "Destroyable" && isGroundPounding)
            {
                // If yes, then destroy the tile
                Destroy(hit.collider.gameObject);
                return;
            }

            // Is the colliding ground a goomba?
            if (hit.collider.gameObject.tag == "Goomba")
            {
                // If yes, then bounce off of the goomba and destroy it
                GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().mass * Vector3.up * 100 * strongJumpMaxIndex);
                StartCoroutine(hit.collider.gameObject.GetComponent<Goomba>().CommenceDeath());
            }

            // Make sure that we set velocity to zero if the force of gravity is being applied
            if ((GetComponent<Particle3D>().velocity.y < 0 && GetComponent<Particle3D>().velocity.y != 0))
            {
                GetComponent<Particle3D>().velocity.y = 0;
            }

            // Reposition object to be on top of colliding ground
            GetComponent<Particle3D>().position.y = hit.point.y;

            // Set all values so player sticks to ground
            //GetComponent<Particle3D>().isUsingGravity = false;
            isAttemptingToJump = false;
            GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;
            GetComponent<Particle3D>().velocity.y = 0;
            isGrounded = true;
            isGroundPounding = false;

            // Is the colliding object an obstavle?
            if (airTriggeredByJump)
            {
                // If yes, then create a stronger jump window
                StartCoroutine(StartStrongerJumpWindow());
            }

            airTriggeredByJump = false;
        }
        else
        {
            // If in air, set all gravity values
            //GetComponent<Particle3D>().isUsingGravity = true;
            // If no, then set the colliding game object to null
            GetComponent<Particle3D>().collidingGameObject = null;
            isGrounded = false;
        }

        // Is the player colliding with any vertical surfaces?
        if (Physics.Raycast(playerTorsoTransform.position, new Vector3(GetComponent<Particle3D>().velocity.normalized.x, 0, GetComponent<Particle3D>().velocity.normalized.z), out hit, movementCheckRaycatHit))
        {
            // Does the colliding object happen to be an already colliding object?
            if (hit.collider.gameObject != GetComponent<Particle3D>().collidingGameObject && hit.collider.gameObject.tag != "Ball Wall")
            {
                // If no then prevent the player from going inside of it
                GetComponent<Particle3D>().velocity.z = 0;
                GetComponent<Particle3D>().velocity.x = 0;
                GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;

                // Is the player in the air?
                if (!isGrounded)
                {
                    // If yes, then the player can wall jump
                    canWallJump = true;
                    collidingPoint = hit.point;
                }
                else
                {
                    // If no then the player cannot wall jump
                    canWallJump = false;
                }
            }
            else
            {
                canWallJump = false;
            }
        }
        else if (Physics.Raycast(playerTorsoTransform.position, transform.GetChild(0).transform.forward, out hit, movementCheckRaycatHit))
        {
            if (hit.collider.gameObject != GetComponent<Particle3D>().collidingGameObject)
            {
                GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;
                if (!isGrounded)
                {
                    canWallJump = true;
                    collidingPoint = hit.point;
                }
                else
                {
                    canWallJump = false;
                }
            }
            else
            {
                canWallJump = false;
            }
        }
        else
        {
            canWallJump = false;
        }
    }





    // This function triggers whenever the player reaches the ground after jumping
    IEnumerator StartStrongerJumpWindow()
    {
        canDoStrongerJump = true;
        yield return new WaitForSeconds(strongerJumpCooldown);
        canDoStrongerJump = false;
    }





    // This function triggers whenever the player punches
    IEnumerator StartPunchCooldown()
    {
        canPunch = false;
        yield return new WaitForSeconds(punchCooldown);
        animator.SetBool("isPunching", false);
        canPunch = true;
    }




    // This function takes the input of the last frame and stores it
    IEnumerator TakeLastInput()
    {
        while(true)
        {
            lastDirectionX = inputAmountX;
            lastDirectionY = inputAmountY;
            yield return new WaitForSeconds(0.1f);
        }
    }



    public void AddKnockBack(Vector3 pointOfHit)
    {
        Vector3 direction = (transform.position - pointOfHit).normalized;
        GetComponent<Particle3D>().AddForce(direction * GetComponent<Particle3D>().mass * forceOfKnockbackHits);
        GetComponent<Particle3D>().AddForce(Vector3.up * GetComponent<Particle3D>().mass * forceOfKnockbackHits);
        GetComponent<Particle3D>().position.y += raycastCheckHit;
        isAttemptingToJump = true;
    }
}
