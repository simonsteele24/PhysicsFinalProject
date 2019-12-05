using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float jumpForce;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float inputAmountX = Mathf.Abs(Input.GetAxis("Horizontal"));
        float inputAmountY = Mathf.Abs(Input.GetAxis("Vertical"));

        GetComponentInChildren<Animator>().SetFloat("Forward", Mathf.Clamp(inputAmountX + inputAmountY, 0, 1), 0.1f, Time.deltaTime);
        GetComponentInChildren<Animator>().SetBool("OnGround", GetComponent<Sphere>().hasCollided);
        GetComponentInChildren<Animator>().SetFloat("Jump", GetComponent<Particle3D>().velocity.y);

        if (Input.GetKey(KeyCode.W))
        {
            GetComponent<Particle3D>().AddForceAtPoint(new Vector3(0.0f, 0.0f, 0.1f), GetComponent<Particle3D>().Mass * Vector3.forward);
            transform.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            GetComponent<Particle3D>().AddForceAtPoint(new Vector3(-0.1f, 0.0f, 0.0f), GetComponent<Particle3D>().Mass * Vector3.left);
            transform.GetChild(0).localEulerAngles = new Vector3(0, 270, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            GetComponent<Particle3D>().AddForceAtPoint(new Vector3(0.0f, 0.0f, -0.1f), GetComponent<Particle3D>().Mass * Vector3.back);
            transform.GetChild(0).localEulerAngles = new Vector3(0, 180, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            GetComponent<Particle3D>().AddForceAtPoint(new Vector3(0.1f, 0.0f, 0.0f), GetComponent<Particle3D>().Mass * Vector3.right);
            transform.GetChild(0).localEulerAngles = new Vector3(0, 90, 0);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * Vector3.up * jumpForce);
        }
    }
}
