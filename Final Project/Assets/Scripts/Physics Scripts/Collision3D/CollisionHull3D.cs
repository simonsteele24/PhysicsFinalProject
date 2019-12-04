using UnityEngine;

public abstract class CollisionHull3D : MonoBehaviour
{
    // Collision hull types
    [HideInInspector] public CollisionHullType3D collisionType;

    // Vector 2's
    protected bool isAlreadyColliding;
    protected bool isColliding;
    protected bool hasResolution;
    protected float rotation;
    protected Vector3 minCorner;
    protected Vector3 maxCorner;
    [HideInInspector] protected Vector3 position;

    public Vector3 GetMinimumCorner() { return minCorner; }

    public Vector3 GetMaximumCorner() { return maxCorner; }

    public Vector3 GetPosition() { return position; }

    public void SetPosition(Vector3 newPos) { position = newPos; }

    public float GetRotation() { return rotation; }

    public void ToggleCollidingChecker() { isAlreadyColliding = true; }

    public void ResetCollidingChecker() { isAlreadyColliding = false; }

    public bool GetCollidingChecker() { return isAlreadyColliding; }

    public void CheckColliding() { isColliding = true; }

    public void ResetColliding() { isColliding = false; }

    public abstract Vector3 GetDimensions();

    public abstract bool GetHasResolution();
}
