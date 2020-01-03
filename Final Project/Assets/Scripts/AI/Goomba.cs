using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour
{
    public float movementSpeed;

    public void MoveTowardsObject(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);
    }

    public void MoveInADirection(Vector3 direction)
    {
        transform.Translate(direction * movementSpeed * Time.deltaTime);
    }

    private void Update()
    {
        
    }
}
