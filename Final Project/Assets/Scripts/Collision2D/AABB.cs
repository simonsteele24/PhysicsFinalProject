using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABB : CollisionHull2D
{
    public float halfLength;
    public float halfWidth;

    // Start is called before the first frame update
    void Start()
    {
        collisionType = CollisionHullType2D.AABB;

        

        // Initialize position of Collision hull
        position = transform.position;

        minCorner = new Vector2(position.x - halfLength, position.y - halfWidth);
        maxCorner = new Vector2(position.x + halfLength, position.y + halfWidth);

        // Add hull to hull list
        CollisionManager.manager.InsertToParticleList(this);
    }

    public override Vector2 GetDimensions() { return new Vector2(halfLength, halfWidth); }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
        minCorner = new Vector2(position.x - halfLength, position.y - halfWidth);
        maxCorner = new Vector2(position.x + halfLength, position.y + halfWidth);
    }
}
