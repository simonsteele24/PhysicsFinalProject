using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABB3D : CollisionHull3D
{
    public float halfSize;

    // Start is called before the first frame update
    void Start()
    {
        collisionType = CollisionHullType3D.AABB;

        

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
    }
}
