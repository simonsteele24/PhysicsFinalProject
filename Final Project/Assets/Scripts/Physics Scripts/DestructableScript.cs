using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Check if y position is less than 0 and then destroy if necessary
        if (transform.position.y < 0.0f)
        {
            gameObject.SetActive(false);
        }
    }
}
