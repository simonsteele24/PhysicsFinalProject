using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    // Floats
    public float rotation;
    public float bombForce;

    // Gameobjects
    public GameObject bomb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Get Input from user
        float rotationVertical = Input.GetAxis("Vertical");
        float rotationHorizontal = Input.GetAxis("Horizontal");

        // Apply rotation based on user input
        transform.Rotate(Vector3.forward, rotationVertical * rotation * Time.deltaTime);
        transform.Rotate(Vector3.up, rotationHorizontal * rotation * Time.deltaTime);

        // Instantiate bomb if space is down
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var newBomb = Instantiate(bomb, transform.position,Quaternion.identity);
            newBomb.GetComponent<Particle3D>().AddForce(transform.right * bombForce);
        }
    }
}
