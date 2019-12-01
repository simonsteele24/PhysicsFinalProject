using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InertiaTensor : MonoBehaviour
{

    // This function attempts to find an intertia tensor for a given shape type
    public static Matrix4x4 GetInertiaTensor(Particle3D particle, ThirdDimensionalShapeType shape, bool isHollow)
    {
        // Go through shape types and get the inertia type based on if the shape is hollow or not
        switch (shape)
        {
            case ThirdDimensionalShapeType.Cube:
                if (isHollow) { return GetHollowCubeInertiaTensor(particle); }
                else { return GetSolidCubeInertiaTensor(particle); }

            case ThirdDimensionalShapeType.Sphere:
                if (isHollow) { return GetHollowSphereInertiaTensor(particle); }
                else { return GetSolidSphereInertiaTensor(particle); }

            case ThirdDimensionalShapeType.Cylinder:
                return GetSolidCylinderInertiaTensor(particle);

            case ThirdDimensionalShapeType.Cone:
                return GetSolidConeIntertiaTensor(particle);

            default:
                // If nothing is found, return an empty matrix
                return Matrix4x4.zero;
        }
    }





    // This function gets the inertia tensor for a solid cube
    public static Matrix4x4 GetSolidCubeInertiaTensor(Particle3D particle)
    {
        return new Matrix4x4(new Vector4(1.0f/12.0f * particle.mass * ((particle.height * particle.height) + (particle.depth * particle.depth)), 0, 0, 0), 
                             new Vector4(0, 1.0f / 12.0f * particle.mass * ((particle.depth * particle.depth) + (particle.width * particle.width)), 0, 0), 
                             new Vector4(0, 0, 1.0f / 12.0f * particle.mass * ((particle.width * particle.width) + (particle.height * particle.height)), 0), 
                             new Vector4(0, 0, 0, 1));
    }





    // This function gets the inertia tensor for a hollow cube
    public static Matrix4x4 GetHollowCubeInertiaTensor(Particle3D particle)
    {
        return new Matrix4x4(new Vector4(5.0f / 3.0f * particle.mass * ((particle.height * particle.height) + (particle.depth * particle.depth)), 0, 0, 0),
                             new Vector4(0, 5.0f / 3.0f * particle.mass * ((particle.depth * particle.depth) + (particle.width * particle.width)), 0, 0),
                             new Vector4(0, 0, 5.0f / 3.0f * particle.mass * ((particle.width * particle.width) + (particle.height * particle.height)), 0),
                             new Vector4(0, 0, 0, 1));
    }





    // This function gets the inertia tensor for a solid sphere
    public static Matrix4x4 GetSolidSphereInertiaTensor(Particle3D particle)
    {
        return new Matrix4x4(new Vector4(2.0f / 5.0f * particle.mass * particle.radius, 0, 0, 0),
                             new Vector4(0, 2.0f / 5.0f * particle.mass * particle.radius, 0, 0),
                             new Vector4(0, 0, 2.0f / 5.0f * particle.mass * particle.radius, 0),
                             new Vector4(0, 0, 0, 1));
    }





    // This function gets the inertia tensor for a hollow cube
    public static Matrix4x4 GetHollowSphereInertiaTensor(Particle3D particle)
    {
        return new Matrix4x4(new Vector4(2.0f / 3.0f * particle.mass * particle.radius, 0, 0, 0),
                             new Vector4(0, 2.0f / 3.0f * particle.mass * particle.radius, 0, 0),
                             new Vector4(0, 0, 2.0f / 3.0f * particle.mass * particle.radius, 0),
                             new Vector4(0, 0, 0, 1));
    }





    // This function gets the inertia tensor for a solid cone
    public static Matrix4x4 GetSolidConeIntertiaTensor(Particle3D particle)
    {
        return new Matrix4x4(new Vector4((3.0f/5.0f * particle.mass * (particle.height * particle.height)) + (3.0f/20.0f * particle.mass * (particle.radius * particle.radius)), 0, 0, 0),
                             new Vector4(0, (3.0f / 5.0f * particle.mass * (particle.height * particle.height)) + (3.0f / 20.0f * particle.mass * (particle.radius * particle.radius)), 0, 0),
                             new Vector4(0, 0, (3.0f / 10.0f) * particle.mass * (particle.radius * particle.radius), 0),
                             new Vector4(0, 0, 0, 1));
    }





    // This function gets the inertia tensor for a solid cylinder
    public static Matrix4x4 GetSolidCylinderInertiaTensor(Particle3D particle)
    {
        return new Matrix4x4(new Vector4(-(1.0f / 12.0f) * particle.mass * (3 * (particle.radius * particle.radius) + (particle.height * particle.height)), 0, 0, 0),
                             new Vector4(0, -(1.0f / 12.0f) * particle.mass * (3 * (particle.radius * particle.radius) + (particle.height * particle.height)), 0, 0),
                             new Vector4(0, 0, -(1.0f / 2.0f) * particle.mass * (particle.radius * particle.radius), 0),
                             new Vector4(0, 0, 0, 1));
    }
}
