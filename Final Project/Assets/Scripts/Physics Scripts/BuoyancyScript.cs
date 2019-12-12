using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyancyScript : MonoBehaviour
{
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
        // Add a force to simulate buoyancy
        GetComponent<Particle3D>().AddForce(ForceGenerator.GenerateForce_buoyancy(transform.position, waterHeight, maxDepth, volume, liquidDensity));
    }
}
