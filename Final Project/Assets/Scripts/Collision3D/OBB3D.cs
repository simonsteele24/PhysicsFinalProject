using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBB3D : CollisionHull3D
{
    // Floats
    public float halfSize = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        collisionType = CollisionHullType3D.OBBB;

        // Initialize position of Collision hull
        position = transform.position;

        minCorner = new Vector3(-halfSize, -halfSize, -halfSize);
        maxCorner = new Vector3(halfSize, halfSize, halfSize);

        // Add hull to hull list
        CollisionManager3D.manager.InsertToParticleList(this);
    }


    public override float GetDimensions() { return halfSize; }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
        rotation = transform.eulerAngles.z;
    }
}
