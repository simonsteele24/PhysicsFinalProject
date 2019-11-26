using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : CollisionHull2D
{
    public float radius;

    // Start is called before the first frame update
    void Start()
    {
        collisionType = CollisionHullType2D.Circle;

        // Initialize position of Collision hull
        position = transform.position;

        // Add hull to hull list
        CollisionManager.manager.InsertToParticleList(this);
    }

    public override Vector2 GetDimensions() { return new Vector2(radius, 0); }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
        minCorner = new Vector2(position.x - radius, position.y - radius);
        maxCorner = new Vector2(position.x + radius, position.y + radius);
    }
}
