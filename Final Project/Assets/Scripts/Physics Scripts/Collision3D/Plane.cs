using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : CollisionHull3D
{
    public Vector3 halfSize;
    public bool hasAResolution;

    // Start is called before the first frame update
    void Start()
    {
        collisionType = CollisionHullType3D.Plane;



        // Initialize position of Collision hull
        position = transform.position;


        halfSize = new Vector3(transform.localScale.x / 2.0f, 0, transform.localScale.z / 2.0f);
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
    }
}
