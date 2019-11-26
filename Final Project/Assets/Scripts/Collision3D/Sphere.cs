using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : CollisionHull3D
{
    public float radius;

    // Start is called before the first frame update
    void Start()
    {
        collisionType = CollisionHullType3D.Sphere;

        // Initialize position of Collision hull
        position = transform.position;

        // Add hull to hull list
        CollisionManager3D.manager.InsertToParticleList(this);
    }

    public override float GetDimensions() { return radius; }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
        minCorner = new Vector3(position.x - radius, position.y - radius, position.z - radius);
        maxCorner = new Vector3(position.x + radius, position.y + radius, position.z + radius);
    }
}
