using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    public GameObject explosion;

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Particle3D>().collidingGameObject != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            //Destroy(gameObject);
        }
    }
}
