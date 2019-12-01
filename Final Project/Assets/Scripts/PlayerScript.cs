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
        if (Input.GetKey(KeyCode.W))
        {
            GetComponent<Particle3D>().AddForceAtPoint(new Vector3(0.0f, 0.0f, 0.1f), GetComponent<Particle3D>().Mass * Vector3.forward);
        }
        if (Input.GetKey(KeyCode.A))
        {
            GetComponent<Particle3D>().AddForceAtPoint(new Vector3(-0.1f, 0.0f, 0.0f), GetComponent<Particle3D>().Mass * Vector3.left);
        }
        if (Input.GetKey(KeyCode.S))
        {
            GetComponent<Particle3D>().AddForceAtPoint(new Vector3(0.0f, 0.0f, -0.1f), GetComponent<Particle3D>().Mass * Vector3.back);
        }
        if (Input.GetKey(KeyCode.D))
        {
            GetComponent<Particle3D>().AddForceAtPoint(new Vector3(0.1f, 0.0f, 0.0f), GetComponent<Particle3D>().Mass * Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Particle3D>().AddForce(GetComponent<Particle3D>().Mass * Vector3.up * jumpForce);
        }
    }
}
