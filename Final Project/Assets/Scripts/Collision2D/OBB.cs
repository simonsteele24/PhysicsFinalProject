using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBB : CollisionHull2D
{
    // Floats
    public float halfLength;
    public float halfWidth;

    // Start is called before the first frame update
    void Start()
    {
        collisionType = CollisionHullType2D.OBBB;

        // Initialize position of Collision hull
        position = transform.position;

        // Add hull to hull list
        CollisionManager.manager.InsertToParticleList(this);
    }


    public override Vector2 GetDimensions() { return new Vector2(halfLength, halfWidth); }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
        rotation = transform.eulerAngles.z;

        Vector3 originalMin = new Vector3(transform.position.x - halfLength, transform.position.y - halfWidth,0);
        Vector3 originalMax = new Vector3(transform.position.x + halfLength, transform.position.y + halfWidth,0);

        minCorner = Quaternion.Euler(0,0,rotation) * (originalMin - transform.position) + transform.position;
        maxCorner = Quaternion.Euler(0,0,rotation) * (originalMax - transform.position) + transform.position;
    }
}
