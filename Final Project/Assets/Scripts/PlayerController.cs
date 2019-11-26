/*
Author: Simon Steele
Class: GPR-350-101
Assignment: Lab 3
Certification of Authenticity: We certify that this
assignment is entirely our own work.
*/

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Particle classes
    private Particle2D particle;

    // Floats
    public float speed;



    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<Particle2D>();
    }



    // Update is called once per frame
    void Update()
    {

        // Has the D key been pressed?
        if (Input.GetKey(KeyCode.D))
        {
        
            // If yes, then add a force and torque in the right direction (opposite direction for torque)
            particle.AddForce(new Vector2(speed, 0));
            particle.ApplyTorque(new Vector2(-speed, 0), new Vector2(0, transform.position.y + particle.boxDimensions.y));

        }

        // Has the A key been pressed?
        if (Input.GetKey(KeyCode.A))
        {

            // If yes, then add a force and torque in the left direction (opposite direction for torque)
            particle.AddForce(new Vector2(-speed, 0));
            particle.ApplyTorque(new Vector2(speed, 0), new Vector2(0, transform.position.y + particle.boxDimensions.y));

        }

    }
}
