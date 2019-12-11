using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    public float rotation;
    public GameObject bomb;
    public float bombForce;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rotationVertical = Input.GetAxis("Vertical");
        float rotationHorizontal = Input.GetAxis("Horizontal");

        transform.Rotate(Vector3.forward, rotationVertical * rotation * Time.deltaTime);
        transform.Rotate(Vector3.up, rotationHorizontal * rotation * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var newBomb = Instantiate(bomb, transform.position,Quaternion.identity);
            newBomb.GetComponent<Particle3D>().AddForce(transform.right * bombForce);
        }
    }
}
