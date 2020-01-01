using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float jumpForce;
    public float movementSpeed = 1;
    public float raycastCheckHit = 1;
    public float movementCheckRaycatHit = 3;
    public float strongerJumpCooldown = 1;
    public float punchCooldown = 1;
    public float lastDirectionX;
    public float lastDirectionY;
    public float punchDistance;
    float originalGravitationalConstant;
    float inputAmountX;
    float inputAmountY;
    public int strongJumpMaxIndex = 4;
    public int groundPoundGravityMultiplier = 2;
    public bool isAttemptingToJump = false;
    public bool isGrounded = true;
    bool isGroundPounding = false;
    bool canDoStrongerJump = false;
    bool airTriggeredByJump = false;
    bool canPunch = true;
    bool isTriggerDown = false;
    public int strongerJumpKey = 1;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TakeLastInput());
        originalGravitationalConstant = GetComponent<Particle3D>().gravitationalConstant;
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
        inputAmountX = Input.GetAxis("Xbox_LeftStick_X");
        inputAmountY = Input.GetAxis("Xbox_LeftStick_Y");
        float sprintAmount = 1 + Input.GetAxis("Xbox_RT");

        // Set all animations
        GetComponentInChildren<Animator>().SetFloat("Forward", Mathf.Clamp(Mathf.Abs(inputAmountX) + Mathf.Abs(inputAmountY), 0, 1), 0.1f, Time.deltaTime);
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
        if (inputAmountY > 0 && !Physics.Raycast(transform.position, Vector3.forward, out hit, movementCheckRaycatHit))
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
        if (inputAmountX < 0 && !Physics.Raycast(transform.position, Vector3.left, out hit, movementCheckRaycatHit))
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
        if (inputAmountY < 0 && !Physics.Raycast(transform.position, Vector3.back, out hit, movementCheckRaycatHit))
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * movementSpeed * -GetComponent<Particle3D>().GetForwardVector());
            transform.GetChild(0).localEulerAngles = new Vector3(0, 180, 0);
            GetComponent<Particle3D>().isAttemptingToMove = true;
        }

        // Check if there is anything on the right of player that will prevent movement
        hasBeenHit = !Physics.Raycast(transform.position, Vector3.right, out hit, movementCheckRaycatHit);
        if (!hasBeenHit)
        {
            if (hit.collider.gameObject == GetComponent<Particle3D>().collidingGameObject)
            {
                hasBeenHit = true;
            }
        }

        // Move if nothing is in the way of the player
        if (inputAmountX > 0 && hasBeenHit)
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * movementSpeed * GetComponent<Particle3D>().GetRightwardVector());
            transform.GetChild(0).localEulerAngles = new Vector3(0, 90, 0);
            GetComponent<Particle3D>().isAttemptingToMove = true;
        }

        if (Input.GetButtonDown("Xbox_B") && canPunch)
        {
            Debug.Log("Punch");
            if (Physics.Raycast(transform.position, transform.GetChild(0).transform.forward, out hit, movementCheckRaycatHit))
            {
                if (hit.collider.gameObject.tag == "Destroyable" && Vector3.Distance(transform.position, hit.collider.transform.position) < punchDistance)
                {
                    Destroy(hit.collider.gameObject);
                }
            }
            StartCoroutine(StartPunchCooldown());
        }

        if (Input.GetAxis("Xbox_LT") > 0)
        {
            if (!isTriggerDown)
            {
                isTriggerDown = true;
                if (Input.GetButtonDown("Xbox_A") && isGrounded)
                {
                    if (Mathf.Abs(inputAmountX) > 0 || Mathf.Abs(inputAmountY) > 0)
                    {
                        Debug.Log("Long Jump");
                        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * 10 * Vector3.up * strongJumpMaxIndex);
                        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * 50 * transform.forward);
                        GetComponent<Particle3D>().position.y += 0.5f;

                        isAttemptingToJump = true;
                        airTriggeredByJump = true;
                        canDoStrongerJump = false;
                    }
                    else
                    {
                        Debug.Log("Backflip");
                        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * 50 * Vector3.up * strongJumpMaxIndex);
                        GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * 10 * -transform.forward);
                        GetComponent<Particle3D>().position.y += 0.5f;

                        isAttemptingToJump = true;
                        airTriggeredByJump = true;
                        canDoStrongerJump = false;
                    }
                }
                else if (!isGrounded)
                {
                    Debug.Log("Ground pound");
                    isGroundPounding = true; 
                }
                else
                {
                    Debug.Log("Crouch");
                }
            }
        }
        else
        {
            isTriggerDown = false;
            // Jump if the character is grounded
            if (Input.GetButtonDown("Xbox_A") && isGrounded)
            {
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

                if (canDoStrongerJump && strongerJumpKey <= strongJumpMaxIndex)
                {
                    strongerJumpKey++;
                    GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * Vector3.up * strongerJumpKey * jumpForce);
                    GetComponent<Particle3D>().position.y += 0.5f;
                }
                else
                {
                    strongerJumpKey = 0;
                    GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * Vector3.up * jumpForce);
                    GetComponent<Particle3D>().position.y += 0.5f;
                }
                isAttemptingToJump = true;
                airTriggeredByJump = true;
                canDoStrongerJump = false;
            }
        }
    }




    // This function checks all physics based values
    void CheckForPhysicsChange()
    {
        if (isGroundPounding)
        {
            GetComponent<Particle3D>().gravitationalConstant = originalGravitationalConstant * groundPoundGravityMultiplier;
        }
        else
        {
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

        RaycastHit hit;
        // See if player is colliding with ground
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastCheckHit))
        {
            if (hit.collider.gameObject.tag == "Destroyable" && isGroundPounding)
            {
                Destroy(hit.collider.gameObject);
                return;
            }
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
            isGroundPounding = false;

            if (hit.collider.gameObject.tag == "Obstacle" && airTriggeredByJump)
            {
                StartCoroutine(StartStrongerJumpWindow());
            }

            airTriggeredByJump = false;
        }
        else
        {
            // If in air, set all gravity values
            GetComponent<Particle3D>().isUsingGravity = true;
            GetComponent<Particle3D>().collidingGameObject = null;
            isGrounded = false;
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
        canPunch = true;
    }





    IEnumerator TakeLastInput()
    {
        while(true)
        {
            lastDirectionX = inputAmountX;
            lastDirectionY = inputAmountY;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
