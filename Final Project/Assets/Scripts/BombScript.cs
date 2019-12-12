using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    public GameObject explosion;
    public Vector3 explosionOffset = new Vector3(0, 10, 0);

    // Update is called once per frame
    void Update()
    {
        // Create an explosion if touching another object
        if (GetComponent<Particle3D>().collidingGameObject != null)
        {
            Instantiate(explosion, transform.position + explosionOffset, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }
}
