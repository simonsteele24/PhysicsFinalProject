using System;
using System.Collections.Generic;
using UnityEngine;

// Collision Hull type enum
public enum CollisionHullType3D
{
    Sphere,
    AABB,
    OBBB
}

public class CollisionManager3D : MonoBehaviour
{
    // This function represents the information about a given collision
    public class CollisionInfo
    {
        // Vector2's
        public Vector3 normal;
        public Vector3 contactPoint;

        // Floats
        public float separatingVelocity;
        public float penetration;

        // Collision Hulls
        public CollisionHull3D a;
        public CollisionHull3D b;


        // This function intializes the collision info class based on given information
        public CollisionInfo(CollisionHull3D _a, CollisionHull3D _b, float _penetration)
        {
            // Is collision A's collision type have less priority to collision B? 
            if (_a.collisionType > _b.collisionType)
            {
                // If yes, then switch their priorities
                a = _b;
                b = _a;
            }
            else
            {
                // If no, then keep them as so
                a = _a;
                b = _b;
            }


            // Based on collision hulls, calculate the rest of the values
            separatingVelocity = CollisionResolution3D.CalculateSeparatingVelocity(a,b);
            normal = (b.GetPosition() - a.GetPosition()).normalized;
            penetration = _penetration;
        }

        // This function intializes the collision info class based on given information
        public CollisionInfo(CollisionHull3D _a, CollisionHull3D _b, float _penetration, Vector3 _normal, Vector3 _contactPoint)
        {
            // Is collision A's collision type have less priority to collision B? 
            if (_a.collisionType > _b.collisionType)
            {
                // If yes, then switch their priorities
                a = _b;
                b = _a;
            }
            else
            {
                // If no, then keep them as so
                a = _a;
                b = _b;
            }


            // Based on collision hulls, calculate the rest of the values
            separatingVelocity = CollisionResolution3D.CalculateSeparatingVelocity(a, b);
            normal = _normal;
            penetration = _penetration;
        }

    }


    public static CollisionManager3D manager;

    private Dictionary<CollisionPairKey3D, Func<CollisionHull3D, CollisionHull3D, CollisionInfo>> _collisionTypeCollisionTestFunctions = new Dictionary<CollisionPairKey3D, Func<CollisionHull3D, CollisionHull3D, CollisionInfo>>(new CollisionPairKey3D.EqualityComparitor());

    // Constants
    public static float RESTING_CONTACT_VALUE = 0.1f;
    public static float UNIVERSAL_COEFFICIENT_OF_RESTITUTION = 0.5f;

    // Lists
    public List<CollisionHull3D> particles;
    public List<CollisionInfo> collisions;




    // Set all the initial values
    private void Awake()
    {
        particles = new List<CollisionHull3D>();
        collisions = new List<CollisionInfo>();
        manager = this;

        _collisionTypeCollisionTestFunctions.Add(new CollisionPairKey3D(CollisionHullType3D.Sphere, CollisionHullType3D.Sphere), CircleToCircleCollision);
        _collisionTypeCollisionTestFunctions.Add(new CollisionPairKey3D(CollisionHullType3D.AABB, CollisionHullType3D.AABB), AABBToAABBCollision);
        _collisionTypeCollisionTestFunctions.Add(new CollisionPairKey3D(CollisionHullType3D.OBBB, CollisionHullType3D.OBBB), OBBToOBBCollision);
        _collisionTypeCollisionTestFunctions.Add(new CollisionPairKey3D(CollisionHullType3D.Sphere, CollisionHullType3D.OBBB), CircleToOBBCollision);
        _collisionTypeCollisionTestFunctions.Add(new CollisionPairKey3D(CollisionHullType3D.Sphere, CollisionHullType3D.AABB), CircleToABBCollision);
        _collisionTypeCollisionTestFunctions.Add(new CollisionPairKey3D(CollisionHullType3D.AABB, CollisionHullType3D.OBBB), AABBToOBBCollision);
    }





    // Update is called once per frame
    void Update()
    {
        collisions.Clear();
        for (int i = 0; i < particles.Count; i++)
        {
            if (particles[i] != null)
            {
                particles[i].ResetCollidingChecker();
                particles[i].ResetColliding();
            }
        }

        // Iterate through all particles
        for (int x = 0; x < particles.Count; x++)
        {
            for (int y = 0; y < particles.Count; y++)
            {
                if (particles[x] != null && particles[y] != null)
                {
                    // If the one being checked equal to itself?
                    if (x != y && (particles[x].transform.parent != particles[y].transform.parent || particles[x].transform.parent == null))
                    {
                        CollisionPairKey3D key = new CollisionPairKey3D(particles[y].collisionType, particles[x].collisionType);

                        CollisionInfo collision;

                        if (particles[x].collisionType > particles[y].collisionType)
                        {
                            collision = _collisionTypeCollisionTestFunctions[key](particles[y], particles[x]);
                        }
                        else
                        {
                            collision = _collisionTypeCollisionTestFunctions[key](particles[x], particles[y]);
                        }


                        if (collision != null)
                        {
                            if ((collision.a.GetComponent<Particle3D>().isCharacterController && !collision.a.GetComponent<Particle3D>().isUsingGravity) || (collision.b.GetComponent<Particle3D>().isCharacterController && !collision.b.GetComponent<Particle3D>().isUsingGravity))
                            {

                            }
                            else
                            {
                                if (collision.a.GetComponent<WindZoneScript>() != null)
                                {
                                    collision.b.GetComponent<Particle3D>().AddForce(collision.a.GetComponent<WindZoneScript>().force);
                                }
                                else if (collision.b.GetComponent<WindZoneScript>() != null)
                                {
                                    collision.a.GetComponent<Particle3D>().AddForce(collision.b.GetComponent<WindZoneScript>().force);
                                }


                                bool isDuplicate = false;
                                for (int i = 0; i < collisions.Count; i++)
                                {
                                    if ((collisions[i].a == particles[y] && collisions[i].b == particles[x]) || (collisions[i].a == particles[x] && collisions[i].b == particles[y]))
                                    {
                                        isDuplicate = true;
                                    }
                                }

                                if (!isDuplicate)
                                {
                                    collisions.Add(collision);
                                }
                            }
                        }
                    }
                }
            }
        }
        CollisionResolution3D.ResolveCollisions(collisions,Time.deltaTime);
    }





    // Inserts a particle to the particle list
    public void InsertToParticleList(CollisionHull3D collision)
    {
        particles.Add(collision);
    }





    // This function computes circle to circle collisions
    public static CollisionInfo CircleToCircleCollision(CollisionHull3D a, CollisionHull3D b)
    {
        // Calculate the distance between both colliders
        Vector3 distance = a.GetPosition() - b.GetPosition();

        float penetration = a.GetDimensions().x + b.GetDimensions().x * (a.GetDimensions().x + b.GetDimensions().x) - Vector3.Dot(distance, distance);

        // Are the Radii less than or equal to the distance between both circles?
        if (penetration > 0)
        {
            // If yes, then inform the parents of the complex shape object (if applicable)
            ReportCollisionToParent(a, b);
        }
        else
        {
            // If no, return nothing
            return null;
        }

        // Return result
        return new CollisionInfo(a, b, penetration);
    }





    // This function computes AABB to AABB collisions
    public static CollisionInfo AABBToAABBCollision(CollisionHull3D a, CollisionHull3D b)
    {
        // Get the penetration values for both axes
        float penetration = 0.0f;

        // Calculate half extents along x axis for each object
        float a_extent = a.GetDimensions().x;
        float b_extent = b.GetDimensions().x;

        // Get the distance between a and b
        Vector3 n = (b.GetPosition() - a.GetPosition());
        n = new Vector3(Mathf.Abs(n.x), Mathf.Abs(n.y),Mathf.Abs(n.z));

        // Calculate overlap on x axis
        float x_overlap = a_extent + b_extent - Mathf.Abs(n.x);

        // SAT test on x axis
        if (x_overlap > 0)
        {
            // Calculate half extents along x axis for each object
            a_extent = a.GetDimensions().y;
            b_extent = b.GetDimensions().y;

            // Calculate overlap on y axis
            float y_overlap = a_extent + b_extent - Mathf.Abs(n.y);

            // SAT test on y axis
            if (y_overlap > 0)
            {
                a_extent = a.GetDimensions().z;
                b_extent = b.GetDimensions().z;

                float z_overlap = a_extent + b_extent - Mathf.Abs(n.z);

                if (z_overlap > 0)
                {
                    // Find out which axis is axis of least penetration
                    if (x_overlap > y_overlap && y_overlap < z_overlap)
                    {
                        // If it is Y, then return Y's overlap
                        penetration = y_overlap;
                    }
                    else if (x_overlap < y_overlap && x_overlap < z_overlap)
                    {
                        penetration = x_overlap;
                    }
                    else
                    {
                        // If it is Y, then return X's overlap
                        penetration = z_overlap;
                    }
                }
            }
        }

        // Do the two checks pass?
        if (penetration > 0)
        {
            // If yes, then inform the parents of the complex shape object (if applicable)
            ReportCollisionToParent(a, b);
        }
        else
        {
            // If no, return nothing
            return null;
        }

        // Return full details of the Collision list if the two collide
        return new CollisionInfo(a, b, penetration);
    }





    // This function computes AABB to OBBB collisions
    public static CollisionInfo AABBToOBBCollision(CollisionHull3D a, CollisionHull3D b)
    {
        List<float> overlaps = new List<float>();
        List<Vector3> axes = new List<Vector3>();

        // Get the transform values for each axis for each shape
        Vector3 x1 = a.GetComponent<Particle3D>().transformMatrix.GetColumn(0);
        Vector3 y1 = a.GetComponent<Particle3D>().transformMatrix.GetColumn(1);
        Vector3 z1 = a.GetComponent<Particle3D>().transformMatrix.GetColumn(2);

        Vector3 x2 = b.GetComponent<Particle3D>().transformMatrix.GetColumn(0);
        Vector3 y2 = b.GetComponent<Particle3D>().transformMatrix.GetColumn(1);
        Vector3 z2 = b.GetComponent<Particle3D>().transformMatrix.GetColumn(2);

        // Go through and check through all overlapping axes

        // Face/Face Object 1
        overlaps.Add(CheckOBBAxis(a, b, x1));
        axes.Add(x1);
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, y1));
        axes.Add(y1);
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, z1));
        axes.Add(z1);
        if (overlaps[overlaps.Count - 1] < 0) { return null; }


        // Face/Face Object 2
        overlaps.Add(CheckOBBAxis(a, b, x2));
        axes.Add(x2);
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, y2));
        axes.Add(y2);
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, z2));
        axes.Add(z2);
        if (overlaps[overlaps.Count - 1] < 0) { return null; }


        // Edge/Edge
        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(x1, x2)));
        axes.Add(Vector3.Cross(x1, x2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(x1, y2)));
        axes.Add(Vector3.Cross(x1, y2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(x1, z2)));
        axes.Add(Vector3.Cross(x1, z2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(y1, x2)));
        axes.Add(Vector3.Cross(y1, x2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(y1, y2)));
        axes.Add(Vector3.Cross(y1, y2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(y1, z2)));
        axes.Add(Vector3.Cross(y1, z2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(z1, x2)));
        axes.Add(Vector3.Cross(z1, x2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(z1, y2)));
        axes.Add(Vector3.Cross(z1, y2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(z1, z2)));
        axes.Add(Vector3.Cross(z1, z2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        float bestOverlap = Mathf.Infinity;
        int bestIndex = 0;
        for (int i = 0; i < overlaps.Count; i++)
        {
            if (overlaps[i] < bestOverlap)
            {
                bestOverlap = overlaps[i];
                bestIndex = i;
            }
        }

        if (bestIndex > 3)
        {
            Vector3 normal = -axes[bestIndex];
            Vector3 axis = axes[bestIndex];
            if (Vector3.Dot(axis,(b.GetPosition()- a.GetPosition())) > 0)
            {
                axis *= -1;
            }

            Vector3 vertex = a.GetDimensions();

            if (Vector3.Dot(a.GetComponent<Particle3D>().transformMatrix.GetColumn(0), normal) > 0) vertex.x = -vertex.x;
            if (Vector3.Dot(a.GetComponent<Particle3D>().transformMatrix.GetColumn(1), normal) > 0) vertex.y = -vertex.y;
            if (Vector3.Dot(a.GetComponent<Particle3D>().transformMatrix.GetColumn(2), normal) > 0) vertex.z = -vertex.z;

            vertex = a.GetComponent<Particle3D>().invTransformMatrix.MultiplyPoint(vertex);

            // If all axis are overlaping, then we have a collision
            ReportCollisionToParent(a, b);
            return new CollisionInfo(a, b, CollisionResolution.GetFinalPenetration(overlaps),normal,vertex);

        }
        else if (bestIndex > 6)
        {
            Vector3 normal = -axes[bestIndex];
            Vector3 axis = axes[bestIndex];
            if (Vector3.Dot(axis, (b.GetPosition() - a.GetPosition())) > 0)
            {
                axis *= -1;
            }

            Vector3 vertex = b.GetDimensions();

            if (Vector3.Dot(b.GetComponent<Particle3D>().transformMatrix.GetColumn(0), normal) > 0) vertex.x = -vertex.x;
            if (Vector3.Dot(b.GetComponent<Particle3D>().transformMatrix.GetColumn(1), normal) > 0) vertex.y = -vertex.y;
            if (Vector3.Dot(b.GetComponent<Particle3D>().transformMatrix.GetColumn(2), normal) > 0) vertex.z = -vertex.z;

            vertex = b.GetComponent<Particle3D>().invTransformMatrix.MultiplyPoint(vertex);

            // If all axis are overlaping, then we have a collision
            ReportCollisionToParent(a, b);
            return new CollisionInfo(a, b, CollisionResolution.GetFinalPenetration(overlaps), normal, vertex);
        }
        else
        {
            int index = bestIndex - 6;
            int oneIndex = index / 3;
            int twoIndex = index % 3;

            Vector3 oneAxis = axes[oneIndex];
            Vector3 twoAxis = axes[twoIndex];

            Vector3 axis = axes[bestIndex].normalized;

            if (Vector3.Dot(axis, b.GetPosition() - a.GetPosition()) > 0) axis *= -1;

            Vector3 pointOnOneEdge = a.GetDimensions();
            Vector3 pointOnTwoEdge = b.GetDimensions();

            if (oneIndex == 0) pointOnOneEdge.x = 0;
            else if (Vector2.Dot(x1, axis) > 0) pointOnOneEdge.x = -pointOnOneEdge.x;

            if (twoIndex == 3) pointOnTwoEdge.x = 0;
            else if (Vector2.Dot(x2, axis) > 0) pointOnTwoEdge.x = -pointOnTwoEdge.x;

            if (oneIndex == 1) pointOnOneEdge.y = 0;
            else if (Vector2.Dot(y1, axis) > 0) pointOnOneEdge.y = -pointOnOneEdge.y;

            if (twoIndex == 4) pointOnTwoEdge.y = 0;
            else if (Vector2.Dot(y2, axis) > 0) pointOnTwoEdge.y = -pointOnTwoEdge.y;

            if (oneIndex == 2) pointOnOneEdge.z = 0;
            else if (Vector2.Dot(z1, axis) > 0) pointOnOneEdge.z = -pointOnOneEdge.z;

            if (twoIndex == 5) pointOnTwoEdge.z = 0;
            else if (Vector2.Dot(z2, axis) > 0) pointOnTwoEdge.z = -pointOnTwoEdge.z;

            pointOnOneEdge = a.GetComponent<Particle3D>().invTransformMatrix.MultiplyPoint(pointOnOneEdge);
            pointOnTwoEdge = b.GetComponent<Particle3D>().invTransformMatrix.MultiplyPoint(pointOnTwoEdge);

            Vector3 contactPoint = GetCollisionPoint(pointOnOneEdge, oneAxis, a.GetDimensions().x, pointOnTwoEdge, twoAxis, b.GetDimensions().x, bestIndex > 2);

            ReportCollisionToParent(a, b);
            return new CollisionInfo(a, b, CollisionResolution.GetFinalPenetration(overlaps), axis, contactPoint);
        }
    }

    public static Vector3 GetCollisionPoint (Vector3 pOne, Vector3 dOne, float oneSize, Vector3 pTwo, Vector3 dTwo, float twoSize, bool useOne)
    {
        float smOne = dOne.sqrMagnitude;
        float smTwo = dTwo.sqrMagnitude;
        float dpOneTwo = Vector3.Dot(dTwo,dOne);

        Vector3 toSt = pOne - pTwo;
        float dpStaOne = Vector3.Dot(dOne, toSt);
        float dpStaTwo = Vector3.Dot(dTwo, toSt);

        float denom = smOne * smTwo - dpOneTwo * dpOneTwo;

        // Zero denominator indicates parrallel lines
        if (Mathf.Abs(denom) < 0.0001f)
        {
            return useOne ? pOne : pTwo;
        }

        float mua = (dpOneTwo * dpStaTwo - smTwo * dpStaOne) / denom;
        float mub = (smOne * dpStaTwo - dpOneTwo * dpStaOne) / denom;

        // If either of the edges has the nearest point out
        // of bounds, then the edges aren't crossed, we have
        // an edge-face contact. Our point is on the edge, which
        // we know from the useOne parameter.
        if (mua > oneSize ||
            mua < -oneSize ||
            mub > twoSize ||
            mub < -twoSize)
        {
            return useOne ? pOne : pTwo;
        }
        else
        {
            Vector3 cOne = pOne + dOne * mua;
            Vector3 cTwo = pTwo + dTwo * mub;

            return cOne * 0.5f + cTwo * 0.5f;
        }
    }





    // This function calculates Circle to OBB collisions
    public static CollisionInfo CircleToABBCollision(CollisionHull3D a, CollisionHull3D b)
    {
        // Find the relative centre by transforming the center of the circle to the local space of the AABB
        Vector3 relativeCentre = b.GetComponent<Particle3D>().invTransformMatrix.MultiplyPoint(a.GetPosition());

        Vector3 closestPointToCircle = new Vector3(Math.Max(b.GetMinimumCorner().x, Math.Min(relativeCentre.x, b.GetMaximumCorner().x)), Math.Max(b.GetMinimumCorner().y, Math.Min(relativeCentre.y, b.GetMaximumCorner().y)), Math.Max(b.GetMinimumCorner().z, Math.Min(relativeCentre.z, b.GetMaximumCorner().z)));

        Vector3 distance = relativeCentre - closestPointToCircle;
        float distanceSquared = Vector3.Dot(distance, distance);
        float penetration = a.GetDimensions().x - Mathf.Sqrt(distanceSquared);

        // Is the penetration a positive value
        if (penetration >= 0)
        {
            // If yes, then inform the parents of the complex shape object (if applicable)
            ReportCollisionToParent(a, b);
        }
        else
        {
            // If no, return nothing
            return null;
        }

        // Return full details of the Collision list if the two collide
        return new CollisionInfo(a, b, penetration, (closestPointToCircle - relativeCentre).normalized, closestPointToCircle);
    }





    // This function calculate Circle to ABB collisions
    public static CollisionInfo CircleToOBBCollision(CollisionHull3D a, CollisionHull3D b)
    {
        // Find the relative centre by transforming the center of the circle to the local space of the AABB
        Vector3 relativeCentre = b.GetComponent<Particle3D>().invTransformMatrix.MultiplyPoint(a.GetPosition());

        Vector3 closestPointToCircle = new Vector3(Math.Max(b.GetMinimumCorner().x, Math.Min(relativeCentre.x, b.GetMaximumCorner().x)), Math.Max(b.GetMinimumCorner().y, Math.Min(relativeCentre.y, b.GetMaximumCorner().y)), Math.Max(b.GetMinimumCorner().z, Math.Min(relativeCentre.z, b.GetMaximumCorner().z)));

        Vector3 distance = relativeCentre - closestPointToCircle;
        float distanceSquared = Vector3.Dot(distance, distance);
        float penetration = a.GetDimensions().x - Mathf.Sqrt(distanceSquared);

        // Is the penetration a positive value
        if (penetration >= 0)
        {
            // If yes, then inform the parents of the complex shape object (if applicable)
            ReportCollisionToParent(a, b);
        }
        else
        {
            // If no, return nothing
            return null;
        }

        // Return full details of the Collision list if the two collide
        return new CollisionInfo(a, b, penetration, (closestPointToCircle - relativeCentre).normalized, closestPointToCircle);
    }





    // This function calculates OBB to OBB colisions
    public static CollisionInfo OBBToOBBCollision(CollisionHull3D a, CollisionHull3D b)
    {
        List<float> overlaps = new List<float>();
        List<Vector3> axes = new List<Vector3>();

        // Get the transform values for each axis for each shape
        Vector3 x1 = a.GetComponent<Particle3D>().transformMatrix.GetColumn(0);
        Vector3 y1 = a.GetComponent<Particle3D>().transformMatrix.GetColumn(1);
        Vector3 z1 = a.GetComponent<Particle3D>().transformMatrix.GetColumn(2);

        Vector3 x2 = b.GetComponent<Particle3D>().transformMatrix.GetColumn(0);
        Vector3 y2 = b.GetComponent<Particle3D>().transformMatrix.GetColumn(1);
        Vector3 z2 = b.GetComponent<Particle3D>().transformMatrix.GetColumn(2);

        // Go through and check through all overlapping axes

        // Face/Face Object 1
        overlaps.Add(CheckOBBAxis(a, b, x1));
        axes.Add(x1);
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, y1));
        axes.Add(y1);
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, z1));
        axes.Add(z1);
        if (overlaps[overlaps.Count - 1] < 0) { return null; }


        // Face/Face Object 2
        overlaps.Add(CheckOBBAxis(a, b, x2));
        axes.Add(x2);
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, y2));
        axes.Add(y2);
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, z2));
        axes.Add(z2);
        if (overlaps[overlaps.Count - 1] < 0) { return null; }


        // Edge/Edge
        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(x1, x2)));
        axes.Add(Vector3.Cross(x1, x2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(x1, y2)));
        axes.Add(Vector3.Cross(x1, y2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(x1, z2)));
        axes.Add(Vector3.Cross(x1, z2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(y1, x2)));
        axes.Add(Vector3.Cross(y1, x2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(y1, y2)));
        axes.Add(Vector3.Cross(y1, y2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(y1, z2)));
        axes.Add(Vector3.Cross(y1, z2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(z1, x2)));
        axes.Add(Vector3.Cross(z1, x2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(z1, y2)));
        axes.Add(Vector3.Cross(z1, y2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        overlaps.Add(CheckOBBAxis(a, b, Vector3.Cross(z1, z2)));
        axes.Add(Vector3.Cross(z1, z2));
        if (overlaps[overlaps.Count - 1] < 0) { return null; }

        float bestOverlap = Mathf.Infinity;
        int bestIndex = 0;
        for (int i = 0; i < overlaps.Count; i++)
        {
            if (overlaps[i] < bestOverlap)
            {
                bestOverlap = overlaps[i];
                bestIndex = i;
            }
        }

        if (bestIndex > 2)
        {
            Vector3 normal = axes[bestIndex];
            Vector3 axis = axes[bestIndex];
            if (Vector3.Dot(axis, (b.GetPosition() - a.GetPosition())) > 0)
            {
                axis *= -1;
            }

            Vector3 vertex = a.GetDimensions();

            if (Vector3.Dot(a.GetComponent<Particle3D>().transformMatrix.GetColumn(0), normal) > 0) vertex.x = -vertex.x;
            if (Vector3.Dot(a.GetComponent<Particle3D>().transformMatrix.GetColumn(1), normal) > 0) vertex.y = -vertex.y;
            if (Vector3.Dot(a.GetComponent<Particle3D>().transformMatrix.GetColumn(2), normal) > 0) vertex.z = -vertex.z;

            vertex = a.GetComponent<Particle3D>().invTransformMatrix.MultiplyPoint(vertex);

            // If all axis are overlaping, then we have a collision
            ReportCollisionToParent(a, b);
            return new CollisionInfo(a, b, CollisionResolution.GetFinalPenetration(overlaps), normal, vertex);

        }
        else if (bestIndex > 5)
        {
            Vector3 normal = axes[bestIndex];
            Vector3 axis = axes[bestIndex];
            if (Vector3.Dot(axis, (a.GetPosition() - b.GetPosition())) > 0)
            {
                axis *= -1;
            }

            Vector3 vertex = b.GetDimensions();

            if (Vector3.Dot(b.GetComponent<Particle3D>().transformMatrix.GetColumn(0), normal) > 0) vertex.x = -vertex.x;
            if (Vector3.Dot(b.GetComponent<Particle3D>().transformMatrix.GetColumn(1), normal) > 0) vertex.y = -vertex.y;
            if (Vector3.Dot(b.GetComponent<Particle3D>().transformMatrix.GetColumn(2), normal) > 0) vertex.z = -vertex.z;

            vertex = b.GetComponent<Particle3D>().invTransformMatrix.MultiplyPoint(vertex);

            // If all axis are overlaping, then we have a collision
            ReportCollisionToParent(a, b);
            return new CollisionInfo(a, b, CollisionResolution.GetFinalPenetration(overlaps), normal, vertex);
        }
        else
        {
            int index = bestIndex - 6;
            int oneIndex = index / 3;
            int twoIndex = index % 3;

            Vector3 oneAxis = axes[oneIndex];
            Vector3 twoAxis = axes[twoIndex];

            Vector3 axis = axes[bestIndex].normalized;

            if (Vector3.Dot(axis, b.GetPosition() - a.GetPosition()) > 0) axis *= -1;

            Vector3 pointOnOneEdge = a.GetDimensions();
            Vector3 pointOnTwoEdge = b.GetDimensions();

            if (oneIndex == 0) pointOnOneEdge.x = 0;
            else if (Vector2.Dot(x1, axis) > 0) pointOnOneEdge.x = -pointOnOneEdge.x;

            if (twoIndex == 3) pointOnTwoEdge.x = 0;
            else if (Vector2.Dot(x2, axis) > 0) pointOnTwoEdge.x = -pointOnTwoEdge.x;

            if (oneIndex == 1) pointOnOneEdge.y = 0;
            else if (Vector2.Dot(y1, axis) > 0) pointOnOneEdge.y = -pointOnOneEdge.y;

            if (twoIndex == 4) pointOnTwoEdge.y = 0;
            else if (Vector2.Dot(y2, axis) > 0) pointOnTwoEdge.y = -pointOnTwoEdge.y;

            if (oneIndex == 2) pointOnOneEdge.z = 0;
            else if (Vector2.Dot(z1, axis) > 0) pointOnOneEdge.z = -pointOnOneEdge.z;

            if (twoIndex == 5) pointOnTwoEdge.z = 0;
            else if (Vector2.Dot(z2, axis) > 0) pointOnTwoEdge.z = -pointOnTwoEdge.z;

            pointOnOneEdge = a.GetComponent<Particle3D>().invTransformMatrix.MultiplyPoint(pointOnOneEdge);
            pointOnTwoEdge = b.GetComponent<Particle3D>().invTransformMatrix.MultiplyPoint(pointOnTwoEdge);

            Vector3 contactPoint = GetCollisionPoint(pointOnOneEdge, oneAxis, a.GetDimensions().x, pointOnTwoEdge, twoAxis, b.GetDimensions().x, bestIndex > 2);

            ReportCollisionToParent(a, b);
            return new CollisionInfo(a, b, CollisionResolution.GetFinalPenetration(overlaps), axis, contactPoint);
        }
    }





    // This function reports two sets of collision hulls to their respective parents (if possible)
    public static void ReportCollisionToParent(CollisionHull3D shapeA, CollisionHull3D shapeB)
    {
        // If yes, then inform the parents of the complex shape object (if applicable)
        if (shapeA.transform.parent != null)
        {

            //shapeA.GetComponentInParent<ParentCollisionScript3D>().ReportCollisionToParent();
        }
        if (shapeB.transform.parent != null)
        {
            //shapeB.GetComponentInParent<ParentCollisionScript3D>().ReportCollisionToParent();
        }
    }





    // This function checks for a collision between two objects by projecting onto a specific axis
    public static float CheckOBBAxis(CollisionHull3D shapeA, CollisionHull3D shapeB, Vector3 rotationAxis)
    {
        float one = transformToOBBAxis(shapeA, rotationAxis);
        float two = transformToOBBAxis(shapeB, rotationAxis);

        float distance = Mathf.Abs(Vector3.Dot(shapeB.GetPosition() - shapeA.GetPosition(),rotationAxis));

        return one + two - distance;
    }


    public static float transformToOBBAxis(CollisionHull3D shape, Vector3 rotationAxis)
    {
        return shape.GetDimensions().x * Mathf.Abs(Vector3.Dot(shape.GetComponent<Particle3D>().transformMatrix.GetColumn(0), rotationAxis)) +
               shape.GetDimensions().y * Mathf.Abs(Vector3.Dot(shape.GetComponent<Particle3D>().transformMatrix.GetColumn(1), rotationAxis)) +
               shape.GetDimensions().z * Mathf.Abs(Vector3.Dot(shape.GetComponent<Particle3D>().transformMatrix.GetColumn(2), rotationAxis));
    }
}
