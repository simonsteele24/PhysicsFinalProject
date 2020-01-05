using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject objectToFollow;
    public Vector3 cameraOffset;

    private void Start()
    {
        transform.position = objectToFollow.transform.position + cameraOffset;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(objectToFollow.transform.position.x + cameraOffset.x, objectToFollow.transform.position.y + cameraOffset.y, objectToFollow.transform.position.z + cameraOffset.z);
    }
}
