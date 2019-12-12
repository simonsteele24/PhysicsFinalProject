using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject objectToFollow;
    public Vector3 cameraOffset;

    // Update is called once per frame
    void Update()
    {
        transform.position = objectToFollow.transform.position + cameraOffset;
    }
}
