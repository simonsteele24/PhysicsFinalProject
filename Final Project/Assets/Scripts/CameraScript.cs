using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject objectToFollow;
    public float rotationSpeed;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Xbox_RightStick_X");

        transform.RotateAround(objectToFollow.transform.position, Vector3.up, inputX * rotationSpeed);
    }
}
