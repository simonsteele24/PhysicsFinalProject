using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionResolution : MonoBehaviour
{
    // This function gets the seperating velocity of two particles
    public static float CalculateSeparatingVelocity(CollisionHull2D shapeA, CollisionHull2D shapeB)
    {
        // Find all required values for calculation
        Vector2 differenceOfVelocity = (shapeA.gameObject.GetComponent<Particle2D>().velocity - shapeB.gameObject.GetComponent<Particle2D>().velocity) * -1;
        Vector2 differenceOfPosition = (shapeA.GetPosition() - shapeB.GetPosition()).normalized;

        // Return the dot product of both velocity and position
        return Vector2.Dot(differenceOfVelocity, differenceOfPosition);
    }





    // This function goes through all collisions and resovlves each and every one of them
    public static void ResolveCollisions(List<CollisionManager.CollisionInfo> collisions, float dt)
    {
        for (int i = 0; i < collisions.Count; i++)
        {
            // Are the two particles moving towards each other
            if (collisions[i].separatingVelocity > 0)
            {
                // If yes, then resolve collision
                ResolvePenetration(collisions[i]);
                ResolveVelocities(collisions[i], dt);
            }
        }
    }





    // This function resolves the velocities with two particles in a collision
    public static void ResolveVelocities(CollisionManager.CollisionInfo collision, float dt)
    {
        // Get the new seperating velocity
        float newSeperatingVelocity = -collision.separatingVelocity * CollisionManager.UNIVERSAL_COEFFICIENT_OF_RESTITUTION;

        // Check the velocity buildup due to acceleration only.
        Vector2 accCausedVelocity = collision.a.GetComponent<Particle2D>().acceleration - collision.b.GetComponent<Particle2D>().acceleration;
        float accCausedSepVelocity = Vector2.Dot(accCausedVelocity, collision.normal) * Time.fixedDeltaTime;



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
        float totalInverseMass = collision.a.GetComponent<Particle2D>().invMass;
        totalInverseMass += collision.b.GetComponent<Particle2D>().invMass;

        // Do both particles have an infinite mass?
        if (totalInverseMass <= 0)
        {
            // If yes, exit the function
            return;
        }

        // Get the impulse of the collision
        float impulse = deltaVelocity / totalInverseMass;
        Vector2 impulsePerIMass = collision.normal * impulse;

        // Apply the new velocities to both particles
        collision.a.GetComponent<Particle2D>().velocity = collision.a.GetComponent<Particle2D>().velocity + impulsePerIMass * collision.a.GetComponent<Particle2D>().invMass;
        collision.b.GetComponent<Particle2D>().velocity = collision.b.GetComponent<Particle2D>().velocity + impulsePerIMass * -collision.b.GetComponent<Particle2D>().invMass;

    }





    // This function resolves the penetration of a collision
    public static void ResolvePenetration(CollisionManager.CollisionInfo collision)
    {
        // Vector3's
        Vector3 particleMovementA;
        Vector3 particleMovementB;

        // If the penetration is non-positive, do not attempt to resolve the penetration
        if (collision.penetration <= 0) { return; }

        // Find the total inverse mass
        float totalInvMass = collision.a.GetComponent<Particle2D>().invMass;
        totalInvMass += collision.b.GetComponent<Particle2D>().invMass;

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
            particleMovementA = movePerIMass * collision.a.GetComponent<Particle2D>().invMass;
            particleMovementB = -movePerIMass * collision.b.GetComponent<Particle2D>().invMass;
        }
        else
        {
            // Determine the amount both object needs to move
            particleMovementA = -movePerIMass * collision.a.GetComponent<Particle2D>().invMass;
            particleMovementB = movePerIMass * collision.b.GetComponent<Particle2D>().invMass;
        }

        // Apply movement to particle A
        collision.a.transform.position += particleMovementA;
        collision.a.GetComponent<Particle2D>().position = collision.a.transform.position;
        collision.a.SetPosition(collision.a.transform.position);

        // Apply movement to particle B
        collision.b.transform.position += particleMovementB;
        collision.b.GetComponent<Particle2D>().position = collision.b.transform.position;
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
