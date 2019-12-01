using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABB3D : CollisionHull3D
{
    public Vector3 halfSize;

    // Start is called before the first frame update
    void Start()
    {
        collisionType = CollisionHullType3D.AABB;

        

        // Initialize position of Collision hull
        position = transform.position;

        halfSize = transform.localScale / 2.0f;
        minCorner = -halfSize;
        maxCorner = halfSize;

        // Add hull to hull list
        CollisionManager3D.manager.InsertToParticleList(this);
    }

    public override Vector3 GetDimensions() { return halfSize; }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
    }
}
