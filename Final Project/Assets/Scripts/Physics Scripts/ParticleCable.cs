using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCable : MonoBehaviour
{
    public float maxLength;
    public GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    float getLength()
    {
        return (transform.position - parent.transform.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        float length = getLength();

        if (length > maxLength)
        {
            float penetration = length - maxLength;
            Vector3 collisionNormal = new Vector3(0, parent.transform.position.y - transform.position.y,0);
            collisionNormal = collisionNormal.normalized;

            AABB3D parentHull = parent.GetComponent<AABB3D>();

            CollisionManager3D.CollisionInfo newCollision = new CollisionManager3D.CollisionInfo(GetComponent<AABB3D>(), parent.GetComponent<AABB3D>(), penetration, collisionNormal, Vector3.zero); ;
            CollisionResolution3D.ResolvePenetration(newCollision);
            CollisionResolution3D.ResolveVelocities(newCollision, Time.deltaTime);
        }
    }
}
