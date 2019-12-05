using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : CollisionHull3D
{
    public float radius;
    public bool hasAResolution;
    public bool hasCollided;

    // Start is called before the first frame update
    void Start()
    {
        collisionType = CollisionHullType3D.Sphere;

        // Initialize position of Collision hull
        position = transform.position;

        // Add hull to hull list
        CollisionManager3D.manager.InsertToParticleList(this);
    }

    public override Vector3 GetDimensions() { return new Vector3(radius, radius, radius); }

    public override bool GetHasResolution() { return hasAResolution; }

    // Update is called once per frame
    void Update()
    {
        hasCollided = isColliding;
        position = transform.position;
        minCorner = new Vector3(position.x - radius, position.y - radius, position.z - radius);
        maxCorner = new Vector3(position.x + radius, position.y + radius, position.z + radius);
    }
}
