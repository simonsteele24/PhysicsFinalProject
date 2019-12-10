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

        if (GetComponent<Particle3D>().collidingGameObject != null)
        {
            GetComponent<Particle3D>().AddForce(Vector3.down * GetComponent<Particle3D>().collidingGameObject.GetComponent<Particle3D>().mass);
        }
    }
}
