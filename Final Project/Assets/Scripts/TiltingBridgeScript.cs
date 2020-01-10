using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltingBridgeScript : MonoBehaviour
{
    // Floats
    public float minAngle;
    public float maxAngle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Has the bridge reached its max Z rotation?
        if (GetComponent<Particle3D>().rotation.z > maxAngle && GetComponent<Particle3D>().rotation.z != minAngle)
        {
            // If yes, then stop it at its max
            GetComponent<Particle3D>().rotation = Quaternion.Euler(new Vector3(0, 45, 20));
            GetComponent<Particle3D>().angularVelocity = Vector3.zero;
        }
        // Has the bridge reached its min Z rotation?
        else if (GetComponent<Particle3D>().rotation.z < minAngle && GetComponent<Particle3D>().rotation.z != minAngle)
        {
            // If yes, then stop it at its min
            GetComponent<Particle3D>().rotation = Quaternion.Euler(new Vector3(0,45,-20));
            GetComponent<Particle3D>().angularVelocity = Vector3.zero;
        }
    }
}
