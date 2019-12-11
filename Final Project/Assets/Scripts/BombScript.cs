using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Particle3D>().collidingGameObject != null)
        {
            Debug.Log("Boom!");
        }
    }
}
