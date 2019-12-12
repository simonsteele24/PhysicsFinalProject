using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float jumpForce;
    public float bulletSpeed;
    public GameObject bullet;
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
        RaycastHit hit;
        if (Physics.Raycast(transform.position,Vector3.down,out hit,raycastCheckHit))
        {
            if (GetComponent<Particle3D>().velocity.y < 0 && GetComponent<Particle3D>().velocity.y != 0)
            {
                GetComponent<Particle3D>().velocity.y = 0;
            }
            GetComponent<Particle3D>().isUsingGravity = false;
            isAttemptingToJump = false;
            GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;
            isGrounded = true;
        }
        else
        {
            GetComponent<Particle3D>().isUsingGravity = true;
            GetComponent<Particle3D>().collidingGameObject = null;
            isGrounded = false;
        }

        if (GetComponent<Particle3D>().collidingGameObject != null)
        {
            if (GetComponent<Particle3D>().collidingGameObject.gameObject.tag == "Dynamic Object" && !isAttemptingToJump)
            {
                GetComponent<Particle3D>().velocity.y = GetComponent<Particle3D>().collidingGameObject.GetComponent<Particle3D>().velocity.y;
            }
        }


        float inputAmountX = Mathf.Abs(Input.GetAxis("Horizontal"));
        float inputAmountY = Mathf.Abs(Input.GetAxis("Vertical"));

        GetComponentInChildren<Animator>().SetFloat("Forward", Mathf.Clamp(inputAmountX + inputAmountY, 0, 1), 0.1f, Time.deltaTime);
        GetComponentInChildren<Animator>().SetBool("OnGround", isGrounded);
        GetComponentInChildren<Animator>().SetFloat("Jump", GetComponent<Particle3D>().velocity.y);

        if (Input.GetKey(KeyCode.W) && !Physics.Raycast(transform.position, Vector3.forward, out hit, movementCheckRaycatHit))
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * movementSpeed * GetComponent<Particle3D>().GetForwardVector());
            transform.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
            GetComponent<Particle3D>().isAttemptingToMove = true;
        }
        if (Input.GetKey(KeyCode.A) && !Physics.Raycast(transform.position, Vector3.left, out hit, movementCheckRaycatHit))
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * movementSpeed * -GetComponent<Particle3D>().GetRightwardVector());
            transform.GetChild(0).localEulerAngles = new Vector3(0, 270, 0);
            GetComponent<Particle3D>().isAttemptingToMove = true;
        }
        if (Input.GetKey(KeyCode.S) && !Physics.Raycast(transform.position, Vector3.back, out hit, movementCheckRaycatHit))
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * movementSpeed * -GetComponent<Particle3D>().GetForwardVector());
            transform.GetChild(0).localEulerAngles = new Vector3(0, 180, 0);
            GetComponent<Particle3D>().isAttemptingToMove = true;
        }

        bool hasBeenHit = !Physics.Raycast(transform.position, Vector3.right, out hit, movementCheckRaycatHit);
        if (!hasBeenHit && GetComponent<Particle3D>().collidingGameObject != null)
        {
            if (hit.collider.gameObject == GetComponent<Particle3D>().collidingGameObject)
            {
                hasBeenHit = true;
            }
        }
        
        if (Input.GetKey(KeyCode.D) && hasBeenHit)
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * movementSpeed * GetComponent<Particle3D>().GetRightwardVector());
            transform.GetChild(0).localEulerAngles = new Vector3(0, 90, 0);
            GetComponent<Particle3D>().isAttemptingToMove = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            var newBullet = Instantiate(bullet, transform.position + Vector3.forward * 2.0f, transform.rotation);
            newBullet.GetComponent<Particle3D>().position = transform.position + Vector3.forward * 2.0f;
            newBullet.GetComponent<Particle3D>().velocity = Vector3.forward * bulletSpeed;
            
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * Vector3.up * jumpForce);
            GetComponent<Particle3D>().position.y += 0.5f;
            isAttemptingToJump = true;
        }
    }
}
