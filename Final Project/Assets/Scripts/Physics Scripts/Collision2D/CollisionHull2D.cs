using UnityEngine;

public abstract class CollisionHull2D : MonoBehaviour
{
    // Collision hull types
    [HideInInspector] public CollisionHullType2D collisionType;

    // Vector 2's
    protected bool isAlreadyColliding;
    protected float rotation;
    protected Vector2 minCorner;
    protected Vector2 maxCorner;
    [HideInInspector] protected Vector2 position;

    public Vector2 GetMinimumCorner() { return minCorner; }

    public Vector2 GetMaximumCorner() { return maxCorner; }

    public Vector2 GetPosition() { return position; }

    public void SetPosition(Vector2 newPos) { position = newPos; }

    public float GetRotation() { return rotation; }

    public void ToggleCollidingChecker() { isAlreadyColliding = true; }

    public void ResetCollidingChecker() { isAlreadyColliding = false; }

    public bool GetCollidingChecker() { return isAlreadyColliding; }

    public abstract Vector2 GetDimensions();
}
