/*
Author: Simon Steele
Class: GPR-350-101
Assignment: Lab 2
Certification of Authenticity: We certify that this
assignment is entirely our own work.
*/

using UnityEngine;

public class ForceGenerator : MonoBehaviour
{
    // The following function adds a gravitational force
    public static Vector2 GenerateForce_Gravtity(float particleMass, float gravitationalConstant, Vector2 worldUp)
    {
        Vector2 f_gravity = particleMass * gravitationalConstant * worldUp;
        return f_gravity;
    }

    public static Vector3 GenerateForce_Gravity3d(float particleMass, float gravitationalConstant, Vector3 worldUp)
    {
        Vector3 f_gravity = particleMass * gravitationalConstant * worldUp;
        return f_gravity;
    }

    // The following function adds a normalized force
    // from a gravitational force and a surface normal unt
    public static Vector2 GenerateForce_normal(Vector2 f_gravity, Vector2 surfaceNormal_unit)
    {
        Vector2 f_normal = Vector3.Project(f_gravity, surfaceNormal_unit);
        return f_normal;
    }

    public static Vector3 GenerateForce_normal3D(Vector3 f_gravity, Vector3 surfaceNormal_unit)
    {
        Vector3 f_normal = Vector3.Project(f_gravity, surfaceNormal_unit);
        return f_normal;
    }

    // The following function generates a sliding force based on a gravitional
    // force and a normalized surface direction 
    public static Vector2 GenerateForce_sliding(Vector2 f_gravity, Vector2 f_normal)
    {
        Vector2 f_sliding = f_gravity + f_normal;
        return f_sliding;
    }


    public static Vector3 GenerateForce_sliding(Vector3 f_gravity, Vector3 f_normal)
    {
        Vector3 f_sliding = f_gravity + f_normal;
        return f_sliding;
    }

    // The following function generates a static frictional force based on a normalized 
    // force, an opposing forcing, and a static friction coefficient
    public static Vector2 GenerateForce_friction_static(Vector2 f_normal, Vector2 f_opposing, float frictionCoefficient_static)
    {
        Vector2 f_friction_s = f_normal * frictionCoefficient_static;

        // Is the friction more than the opposing force?
        if (f_friction_s.magnitude < f_opposing.magnitude)
        {
            // Yes, then return the normal with the coefficient static
            return -frictionCoefficient_static * f_normal;
        }
        else
        {
            // No, then return the opposing force
            return -f_opposing;
        }
    }


    // The following function generates a kinetic frictional force based on a normalized force,
    // a particle's velocity, and kinetic friction coefficient
    public static Vector3 GenerateForce_friction_kinetic(Vector3 f_normal, Vector3 particleVelocity, float frictionCoefficient_kinetic)
    {
        Vector3 f_friction_k = -frictionCoefficient_kinetic * f_normal.magnitude * particleVelocity.normalized;
        return f_friction_k;
    }


    // The following force generates a drag force based on a particle's velocity, the fluid velocity
    //  of the thing that is applying the drag, the cross section of the particle, and a drag coefficient
    public static Vector3 GenerateForce_drag(Vector3 particleVelocity, Vector3 fluidVelocity, float fluidDensity, float objectArea_crossSection, float objectDragCoefficient)
    {
        Vector3 f_drag = (fluidDensity * Vector3.Scale(particleVelocity , fluidVelocity) * objectArea_crossSection * objectDragCoefficient) / 2.0f;
        return f_drag;
    }



    // The following function generates a spring force on a particle based on the particle's position, the anchor's
    // position, the spring's rest position, and the stiffness of the spring
    public static Vector3 GenerateForce_spring(Vector3 particlePosition, Vector3 anchorPosition, float springRestingLength, float springStiffnessCoefficient)
    {
        Vector3 force = (particlePosition - anchorPosition);
        float magnitude = force.magnitude;
        magnitude = (springRestingLength - magnitude) * springStiffnessCoefficient;

        force = force.normalized;

        return force * magnitude;
    }



    // The following functions adds a buoyancy force based on particle position, the water's height,
    // the max depth of the water, the volume of the water, and the liquid density of the water
    public static Vector2 GenerateForce_buoyancy(Vector2 particlePosition, float waterHeight, float maxDepth, float volume, float liquidDensity)
    {
        float depth = particlePosition.y;
        Vector2 force = new Vector2(0, 0);

        // Is the particle out of water?
        if (depth >= waterHeight + maxDepth)
        {
            // If yes, then don't add the force
            return force;
        }

        // Is the particle at the bottom of the water
        if (depth <= waterHeight - maxDepth)
        {
            // If yes, then apply a force as if its the bottom of the water
            force.y = liquidDensity * volume;
            return force;
        }

        // If all coniditons are returned false, then assume the cube is partially submerged
        force.y = liquidDensity * volume * (depth - maxDepth - waterHeight) / 2 * maxDepth;
        return force;
    }
}
