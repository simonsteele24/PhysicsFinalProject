using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBB3D : CollisionHull3D
{
    // Floats
    public Vector3 halfSize;
    public bool hasAResolution;

    // Start is called before the first frame update
    void Start()
    {
        collisionType = CollisionHullType3D.OBBB;

        // Initialize position of Collision hull
        position = transform.position;

        halfSize = transform.localScale / 2.0f;
        minCorner = -halfSize;
        maxCorner = halfSize;

        // Add hull to hull list
        CollisionManager3D.manager.InsertToParticleList(this);
    }


    public override Vector3 GetDimensions() { return halfSize; }

    public override bool GetHasResolution() { return hasAResolution; }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
        rotation = transform.eulerAngles.z;
    }
}
