using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionResolution3D : MonoBehaviour
{
    // This function gets the seperating velocity of two particles
    public static float CalculateSeparatingVelocity(CollisionHull3D shapeA, CollisionHull3D shapeB, Vector3 contactNormal)
    {
        // Find all required values for calculation
        Vector3 differenceOfVelocity = (shapeB.gameObject.GetComponent<Particle3D>().velocity - shapeA.gameObject.GetComponent<Particle3D>().velocity);

        // Return the dot product of both velocity and position
        return Vector3.Dot(differenceOfVelocity, contactNormal);
    }





    // This function goes through all collisions and resovlves each and every one of them
    public static void ResolveCollisions(List<CollisionManager3D.CollisionInfo> collisions, float dt)
    {
        for (int i = 0; i < collisions.Count; i++)
        {
            // Are the two particles moving towards each other
            if (collisions[i].separatingVelocity > 0)
            {
                if (collisions[i].a.GetHasResolution() == false && collisions[i].b.GetHasResolution() == false)
                {
                    // If yes, then resolve collision
                    ResolvePenetration(collisions[i]);
                }
                
                ResolveVelocities(collisions[i], dt);
            }
        }
    }





    // This function resolves the velocities with two particles in a collision
    public static void ResolveVelocities(CollisionManager3D.CollisionInfo collision, float dt)
    {
        // Get the new seperating velocity
        float newSeperatingVelocity = -collision.separatingVelocity * CollisionManager.UNIVERSAL_COEFFICIENT_OF_RESTITUTION;
        //newSeperatingVelocity = Mathf.Abs(newSeperatingVelocity);

        // Check the velocity buildup due to acceleration only.
        Vector3 accCausedVelocity = collision.a.GetComponent<Particle3D>().acceleration - collision.b.GetComponent<Particle3D>().acceleration;
        float accCausedSepVelocity = Vector3.Dot(accCausedVelocity, collision.normal) * Time.fixedDeltaTime;


        // If we’ve got a closing velocity due to aceleration buildup,
        // remove it from the new separating velocity.
        if (accCausedSepVelocity < 0)
        {
            newSeperatingVelocity *= accCausedSepVelocity;
            if (newSeperatingVelocity < 0) newSeperatingVelocity = 0;
        }


        // Get the delta velocity between the new and old seperating velocity
        float deltaVelocity = newSeperatingVelocity - collision.separatingVelocity;

        // Get the total inverse mass
        float totalInverseMass = collision.a.GetComponent<Particle3D>().invMass;
        totalInverseMass += collision.b.GetComponent<Particle3D>().invMass;

        // Do both particles have an infinite mass?
        if (totalInverseMass <= 0)
        {
            // If yes, exit the function
            return;
        }
        

        // Get the impulse of the collision
        float impulse = deltaVelocity / totalInverseMass;
        Vector3 impulsePerIMass = collision.normal * impulse;

        collision.a.GetComponent<Particle3D>().velocity = collision.a.GetComponent<Particle3D>().velocity + impulsePerIMass * collision.a.GetComponent<Particle3D>().invMass;
        collision.b.GetComponent<Particle3D>().velocity = collision.b.GetComponent<Particle3D>().velocity + impulsePerIMass * -collision.b.GetComponent<Particle3D>().invMass;


        collision.a.CheckColliding();
        collision.a.GetComponent<Particle3D>().collidingGameObject = collision.b.gameObject;
        collision.b.CheckColliding();
        collision.b.GetComponent<Particle3D>().collidingGameObject = collision.a.gameObject;
        // Apply the new velocities to both particles



    }





    // This function resolves the penetration of a collision
    public static void ResolvePenetration(CollisionManager3D.CollisionInfo collision)
    {
        Debug.Log("Resolved penetration");

        // Vector3's
        Vector3 particleMovementA;
        Vector3 particleMovementB;

        // If the penetration is non-positive, do not attempt to resolve the penetration
        if (collision.penetration <= 0) { return; }

        // Find the total inverse mass
        float totalInvMass = collision.a.GetComponent<Particle3D>().invMass;
        totalInvMass += collision.b.GetComponent<Particle3D>().invMass;

        // Do both particles have an infinite mass?
        if (totalInvMass <= 0)
        {
            // If yes, then exit the function
            return;
        }

        // Calculate amount each object needs to move to resolve penetration
        Vector2 movePerIMass = collision.normal * (collision.penetration / totalInvMass);

        // Is particle A's position less than B's position
        if (collision.a.GetPosition().y < collision.b.GetPosition().y)
        {
            // Determine the amount both object needs to move
            particleMovementA = movePerIMass * collision.a.GetComponent<Particle3D>().invMass;
            particleMovementB = -movePerIMass * collision.b.GetComponent<Particle3D>().invMass;
        }
        else
        {
            // Determine the amount both object needs to move

            if (collision.a.GetComponent<Particle3D>().isCharacterController)
            {
                particleMovementA = -movePerIMass * collision.a.GetComponent<Particle3D>().invMass * collision.a.GetComponent<Particle3D>().controllerLevitationValue;
            }
            else
            {
                particleMovementA = -movePerIMass * collision.a.GetComponent<Particle3D>().invMass;
            }
            
            if (collision.b.GetComponent<Particle3D>().isCharacterController)
            {
                particleMovementB = movePerIMass * collision.b.GetComponent<Particle3D>().invMass * collision.b.GetComponent<Particle3D>().controllerLevitationValue;
            }
            else
            {
                particleMovementB = movePerIMass * collision.b.GetComponent<Particle3D>().invMass;
            }

            
        }

        // Apply movement to particle A
        collision.a.transform.position += particleMovementA;
        collision.a.GetComponent<Particle3D>().position += particleMovementA;
        collision.a.SetPosition(collision.a.transform.position);

        // Apply movement to particle B
        collision.b.transform.position += particleMovementB;
        collision.b.GetComponent<Particle3D>().position += particleMovementB;
        collision.b.SetPosition(collision.b.transform.position);
    }





    // This function gets the penetration based on overlap values
    public static float GetFinalPenetration(List<float> overlaps)
    {
        // Initialize values
        float penetration = -Mathf.Infinity;


        for (int i = 0; i < overlaps.Count; i++)
        {
            // Is the overlap less than the current penetration?
            if (overlaps[i] < penetration && overlaps[i] >= 0)
            {
                // If yes, then set the penetration to this value
                penetration = overlaps[i];
            }
        }

        // Return the result
        return penetration;
    }
}
