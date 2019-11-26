using System;
using System.Collections.Generic;
using UnityEngine;

// Collision Hull type enum
public enum CollisionHullType2D
{
    Circle,
    AABB,
    OBBB
}

public class CollisionManager : MonoBehaviour
{
    // This function represents the information about a given collision
    public class CollisionInfo
    {
        // Vector2's
        public Vector2 normal;

        // Floats
        public float separatingVelocity;
        public float penetration;

        // Collision Hulls
        public CollisionHull2D a;
        public CollisionHull2D b;


        // This function intializes the collision info class based on given information
        public CollisionInfo(CollisionHull2D _a, CollisionHull2D _b, float _penetration)
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
            separatingVelocity = CollisionResolution.CalculateSeparatingVelocity(a,b);
            normal = (b.GetPosition() - a.GetPosition()).normalized;
            penetration = _penetration;
        }

    }


    public static CollisionManager manager;

    private Dictionary<CollisionPairKey, Func<CollisionHull2D, CollisionHull2D, CollisionInfo>> _collisionTypeCollisionTestFunctions = new Dictionary<CollisionPairKey, Func<CollisionHull2D, CollisionHull2D, CollisionInfo>>(new CollisionPairKey.EqualityComparitor());

    // Constants
    public static float RESTING_CONTACT_VALUE = 0.1f;
    public static float UNIVERSAL_COEFFICIENT_OF_RESTITUTION = 0.5f;

    // Lists
    public List<CollisionHull2D> particles;
    public List<CollisionInfo> collisions;




    // Set all the initial values
    private void Awake()
    {
        particles = new List<CollisionHull2D>();
        collisions = new List<CollisionInfo>();
        manager = this;

        _collisionTypeCollisionTestFunctions.Add(new CollisionPairKey(CollisionHullType2D.Circle, CollisionHullType2D.Circle), CircleToCircleCollision);
        _collisionTypeCollisionTestFunctions.Add(new CollisionPairKey(CollisionHullType2D.AABB, CollisionHullType2D.AABB), AABBToAABBCollision);
        _collisionTypeCollisionTestFunctions.Add(new CollisionPairKey(CollisionHullType2D.OBBB, CollisionHullType2D.OBBB), OBBToOBBCollision);
        _collisionTypeCollisionTestFunctions.Add(new CollisionPairKey(CollisionHullType2D.Circle, CollisionHullType2D.OBBB), CircleToOBBCollision);
        _collisionTypeCollisionTestFunctions.Add(new CollisionPairKey(CollisionHullType2D.Circle, CollisionHullType2D.AABB), CircleToABBCollision);
        _collisionTypeCollisionTestFunctions.Add(new CollisionPairKey(CollisionHullType2D.AABB, CollisionHullType2D.OBBB), AABBToOBBCollision);
    }





    // Update is called once per frame
    void Update()
    {
        collisions.Clear();
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].ResetCollidingChecker();
        }

        // Iterate through all particles
        for (int x = 0; x < particles.Count; x++)
        {
            for (int y = 0; y < particles.Count; y++)
            {
                // If the one being checked equal to itself?
                if (x != y && (particles[x].transform.parent != particles[y].transform.parent || particles[x].transform.parent == null))
                {
                    CollisionPairKey key = new CollisionPairKey(particles[y].collisionType, particles[x].collisionType);

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
        CollisionResolution.ResolveCollisions(collisions,Time.deltaTime);
    }





    // Inserts a particle to the particle list
    public void InsertToParticleList(CollisionHull2D collision)
    {
        particles.Add(collision);
    }





    // This function computes circle to circle collisions
    public static CollisionInfo CircleToCircleCollision(CollisionHull2D a, CollisionHull2D b)
    {
        // Calculate the distance between both colliders
        Vector2 distance = a.GetPosition() - b.GetPosition();

        float penetration = a.GetDimensions().x + b.GetDimensions().x * (a.GetDimensions().x + b.GetDimensions().x) - Vector2.Dot(distance, distance);

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
    public static CollisionInfo AABBToAABBCollision(CollisionHull2D a, CollisionHull2D b)
    {
        // Get the penetration values for both axes
        float penetration = 0.0f;

        // Calculate half extents along x axis for each object
        float a_extent = a.GetDimensions().x;
        float b_extent = b.GetDimensions().x;

        // Get the distance between a and b
        Vector2 n = (b.GetPosition() - a.GetPosition());
        n = new Vector2(Mathf.Abs(n.x), Mathf.Abs(n.y));

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
                // Find out which axis is axis of least penetration
                if (x_overlap > y_overlap)
                {
                    // If it is Y, then return Y's overlap
                    penetration = y_overlap;
                }
                else
                {
                    // If it is Y, then return X's overlap
                    penetration = x_overlap;
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
    public static CollisionInfo AABBToOBBCollision(CollisionHull2D a, CollisionHull2D b)
    {

        // Compute the R hat and U hat for A
        Vector2 ARHat = new Vector2((Mathf.Cos(b.GetRotation())), Mathf.Abs(-Mathf.Sin(b.GetRotation())));
        Vector2 AUHat = new Vector2((Mathf.Sin(b.GetRotation())), Mathf.Abs(Mathf.Cos(b.GetRotation())));

        // Create a list of all penetrations from all axes
        List<float> overlaps = new List<float>();

        // Do axis checks
        overlaps.Add(CheckOBBAxis(a, b, AUHat));

        // Do all checks pass?
        if (overlaps[0] == Mathf.Infinity)
        {
            // If no, then return nothing
            return null;
        }

        // Do secondary axis checks
        overlaps.Add(CheckOBBAxis(a, b, ARHat));

        // Do all checks pass?
        if (overlaps[1] != Mathf.Infinity)
        {
            // If yes, then inform the parents of the complex shape object (if applicable)
            ReportCollisionToParent(a, b);
        }
        else
        {
            // If no, then return nothing
            return null;
        }

        // Return full details of the Collision list if the two collide
        return new CollisionInfo(a, b, CollisionResolution.GetFinalPenetration(overlaps));
    }





    // This function calculates Circle to OBB collisions
    public static CollisionInfo CircleToABBCollision(CollisionHull2D a, CollisionHull2D b)
    {

        // Find the closest point to the circle from the AABB
        Vector2 closestPointToCircle = new Vector2(Math.Max(b.GetMinimumCorner().x, Math.Min(a.GetPosition().x, b.GetMaximumCorner().x)), Math.Max(b.GetMinimumCorner().y, Math.Min(a.GetPosition().y, b.GetMaximumCorner().y)));

        // Get the distance between the closest point and the circle's position
        Vector2 distance = a.GetPosition() - closestPointToCircle;
        float distanceSquared = Vector2.Dot(distance, distance);

        // Calculate the penetration
        float penetration = a.GetDimensions().x - Mathf.Sqrt(distanceSquared);

        // Is the penetration a positive value
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





    // This function calculate Circle to ABB collisions
    public static CollisionInfo CircleToOBBCollision(CollisionHull2D a, CollisionHull2D b)
    {
        Vector2 closestPointToCircle = new Vector2(Math.Max(b.GetMinimumCorner().x, Math.Min(a.GetPosition().x, b.GetMaximumCorner().x)), Math.Max(b.GetMinimumCorner().y, Math.Min(a.GetPosition().y, b.GetMaximumCorner().y)));

        Vector2 distance = a.GetPosition() - closestPointToCircle;
        float distanceSquared = Vector2.Dot(distance, distance);
        float penetration = a.GetDimensions().x - Mathf.Sqrt(distanceSquared);

        // Does the check pass?
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





    // This function calculates OBB to OBB colisions
    public static CollisionInfo OBBToOBBCollision(CollisionHull2D a, CollisionHull2D b)
    {
        // Compute the R hat and U hat for both collision hulls
        Vector2 ARHat = new Vector2(Mathf.Abs(Mathf.Cos(a.GetRotation())), Mathf.Abs(-Mathf.Sin(a.GetRotation())));
        Vector2 BRHat = new Vector2(Mathf.Abs(Mathf.Cos(b.GetRotation())), Mathf.Abs(-Mathf.Sin(b.GetRotation())));
        Vector2 AUHat = new Vector2(Mathf.Abs(Mathf.Sin(a.GetRotation())), Mathf.Abs(Mathf.Cos(a.GetRotation())));
        Vector2 BUHat = new Vector2(Mathf.Abs(Mathf.Sin(b.GetRotation())), Mathf.Abs(Mathf.Cos(b.GetRotation())));

        // Create a list of all penetrations on each axis
        List<float> overlaps = new List<float>();

        // Do axis checks
        overlaps.Add(CheckOBBAxis(a, b, ARHat));

        // Does the check pass?
        if (overlaps[0] == Mathf.Infinity)
        {
            // If no, return nothing
            return null;
        }

        // Do axis checks
        overlaps.Add(CheckOBBAxis(a, b, AUHat));

        // Does the check pass?
        if (overlaps[1] == Mathf.Infinity)
        {
            // If no, return nothing
            return null;
        }

        // Do axis checks
        overlaps.Add(CheckOBBAxis(a, b, BRHat));

        // Does the check pass?
        if (overlaps[2] == Mathf.Infinity)
        {
            // If no, return nothing
            return null;
        }

        // Do axis checks
        overlaps.Add(CheckOBBAxis(a, b, AUHat));

        // Do the axis checks pass?
        if (overlaps[3] != Mathf.Infinity)
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
        return new CollisionInfo(a, b, CollisionResolution.GetFinalPenetration(overlaps));
    }





    // This function reports two sets of collision hulls to their respective parents (if possible)
    public static void ReportCollisionToParent(CollisionHull2D shapeA, CollisionHull2D shapeB)
    {
        // If yes, then inform the parents of the complex shape object (if applicable)
        if (shapeA.transform.parent != null)
        {
            shapeA.GetComponentInParent<ParentCollisionScript>().ReportCollisionToParent();
        }
        if (shapeB.transform.parent != null)
        {
            shapeB.GetComponentInParent<ParentCollisionScript>().ReportCollisionToParent();
        }
    }





    // This function checks for a collision between two objects by projecting onto a specific axis
    public static float CheckOBBAxis(CollisionHull2D shapeA, CollisionHull2D shapeB, Vector2 rotationAxis)
    {
        // Create a list of all points from the OBB hull for shape A
        List<Vector2> shapeAPoints = new List<Vector2>();
        shapeAPoints.Add(new Vector2(shapeA.GetPosition().x + shapeA.GetDimensions().x, shapeA.GetPosition().y + shapeA.GetDimensions().y));
        shapeAPoints.Add(new Vector2(shapeA.GetPosition().x - shapeA.GetDimensions().x, shapeA.GetPosition().y - shapeA.GetDimensions().y));
        shapeAPoints.Add(new Vector2(shapeA.GetPosition().x - shapeA.GetDimensions().x, shapeA.GetPosition().y + shapeA.GetDimensions().y));
        shapeAPoints.Add(new Vector2(shapeA.GetPosition().x + shapeA.GetDimensions().x, shapeA.GetPosition().y - shapeA.GetDimensions().y));

        // Create a list of all points from the OBB hull for shape B
        List<Vector2> shapeBPoints = new List<Vector2>();
        shapeBPoints.Add(new Vector2(shapeB.GetPosition().x + shapeB.GetDimensions().x, shapeB.GetPosition().y + shapeB.GetDimensions().y));
        shapeBPoints.Add(new Vector2(shapeB.GetPosition().x - shapeB.GetDimensions().x, shapeB.GetPosition().y - shapeB.GetDimensions().y));
        shapeBPoints.Add(new Vector2(shapeB.GetPosition().x - shapeB.GetDimensions().x, shapeB.GetPosition().y + shapeB.GetDimensions().y));
        shapeBPoints.Add(new Vector2(shapeB.GetPosition().x + shapeB.GetDimensions().x, shapeB.GetPosition().y - shapeB.GetDimensions().y));

        // Initialized shape minimums and maximums
        float shapeAMin = Mathf.Infinity;
        float shapeAMax = -Mathf.Infinity;
        float shapeBMin = Mathf.Infinity;
        float shapeBMax = -Mathf.Infinity;

        // Set the total minimum and maximums
        float totalMin;
        float totalMax;

        // Initialize all points for axis checks
        for (int i = 0; i < shapeAPoints.Count; i++)
        {
            // Rotate original point
            shapeAPoints[i] = new Vector2(Mathf.Cos(shapeA.GetRotation()) * (shapeAPoints[i].x - shapeA.GetPosition().x) - Mathf.Sin(shapeA.GetRotation()) * (shapeAPoints[i].y - shapeA.GetPosition().y) + shapeA.GetPosition().x,
                                          Mathf.Sin(shapeA.GetRotation()) * (shapeAPoints[i].x - shapeA.GetPosition().x) + Mathf.Cos(shapeA.GetRotation()) * (shapeAPoints[i].y - shapeA.GetPosition().y) + shapeA.GetPosition().y);

            // Project point
            float temp = Vector2.Dot(shapeAPoints[i], rotationAxis);

            // Is the point less than the minimum?
            if (temp < shapeAMin)
            {
                // If yes, set it to the new minimum
                shapeAMin = temp;
            }

            // Is the point greater than the maximum?
            if (temp > shapeAMax)
            {
                // If yes, set it to the new maximum
                shapeAMax = temp;
            }

            // Rotate original point
            shapeBPoints[i] = new Vector2(Mathf.Cos(shapeB.GetRotation()) * (shapeBPoints[i].x - shapeB.GetPosition().x) - Mathf.Sin(shapeB.GetRotation()) * (shapeBPoints[i].y - shapeB.GetPosition().y) + shapeB.GetPosition().x,
                                          Mathf.Sin(shapeB.GetRotation()) * (shapeBPoints[i].x - shapeB.GetPosition().x) + Mathf.Cos(shapeB.GetRotation()) * (shapeBPoints[i].y - shapeB.GetPosition().y) + shapeB.GetPosition().y);

            // Project point
            temp = Vector2.Dot(shapeBPoints[i], rotationAxis);

            // Is the point less than the minimum?
            if (temp < shapeBMin)
            {
                // If yes, set it to the new minimum
                shapeBMin = temp;
            }

            // Is the point greater than the maximum
            if (temp > shapeBMax)
            {
                // If yes, set it to the new maximum
                shapeBMax = temp;
            }
        }


        // Is the B shape min greater than the A shape maximum
        if (shapeBMin > shapeAMin)
        {
            // If yes, set A to the new minimum
            totalMin = shapeAMin;
        }
        else
        {
            // If no, set B to the new minimum
            totalMin = shapeBMin;
        }

        // Is the A shape maximum greater than the B shape maximum 
        if (shapeBMax < shapeAMax)
        {
            // If yes, set A to the new maximum
            totalMax = shapeAMax;
        }
        else
        {
            // If no, set B to the new maximum
            totalMax = shapeBMax;
        }

        // Do axis checks
        bool axisCheck = shapeAMin <= shapeBMax && shapeBMin <= shapeAMax;

        // Does the check pass?
        if (axisCheck)
        {
            // If yes, then return the penetration
            return totalMin - totalMax;
        }
        else
        {
            // If no, return nothing
            return Mathf.Infinity;
        }
    }
}
