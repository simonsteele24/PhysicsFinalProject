using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatControllerScript : MonoBehaviour
{
    public float turningSpeed;
    public float thrustSpeed;

    public float waterHeight;
    public float maxDepth;
    public float volume;
    public float liquidDensity;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Particle3D>().AddForce(ForceGenerator.GenerateForce_buoyancy(transform.position, waterHeight, maxDepth, volume, liquidDensity));

        if (Input.GetKey(KeyCode.W))
        {
            GetComponent<Particle3D>().AddForce(thrustSpeed * GetComponent<Particle3D>().Mass * transform.right);
        }
        if (Input.GetKey(KeyCode.A))
        {
            GetComponent<Particle3D>().ApplyTorque(new Vector3(0.0f, -1.0f,0.0f) * turningSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            GetComponent<Particle3D>().AddForce(thrustSpeed * GetComponent<Particle3D>().Mass * -transform.right);
        }
        if (Input.GetKey(KeyCode.D))
        {
            GetComponent<Particle3D>().ApplyTorque(new Vector3(0.0f, 1.0f, 0.0f) * turningSpeed);
        }
    }
}
