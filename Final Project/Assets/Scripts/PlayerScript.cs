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
            Debug.Log("Hitting");
            GetComponent<Particle3D>().velocity.y = 0;
            GetComponent<Particle3D>().isUsingGravity = false;
            GetComponent<Particle3D>().collidingGameObject = hit.collider.gameObject;
            GetComponentInChildren<Animator>().SetBool("OnGround", true);
        }
        else
        {
            GetComponent<Particle3D>().isUsingGravity = true;
            GetComponentInChildren<Animator>().SetBool("OnGround", false);
        }


        float inputAmountX = Mathf.Abs(Input.GetAxis("Horizontal"));
        float inputAmountY = Mathf.Abs(Input.GetAxis("Vertical"));

        GetComponentInChildren<Animator>().SetFloat("Forward", Mathf.Clamp(inputAmountX + inputAmountY, 0, 1), 0.1f, Time.deltaTime);
        GetComponentInChildren<Animator>().SetFloat("Jump", GetComponent<Particle3D>().velocity.y);

        if (Input.GetKey(KeyCode.W))
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * movementSpeed * GetComponent<Particle3D>().GetForwardVector());
            transform.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
            GetComponent<Particle3D>().isAttemptingToMove = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * movementSpeed * -GetComponent<Particle3D>().GetRightwardVector());
            transform.GetChild(0).localEulerAngles = new Vector3(0, 270, 0);
            GetComponent<Particle3D>().isAttemptingToMove = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * movementSpeed * -GetComponent<Particle3D>().GetForwardVector());
            transform.GetChild(0).localEulerAngles = new Vector3(0, 180, 0);
            GetComponent<Particle3D>().isAttemptingToMove = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Debug.Log(GetComponent<Particle3D>().Mass * movementSpeed * GetComponent<Particle3D>().GetRightwardVector());
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * Vector3.up * jumpForce);
        }
    }
}
