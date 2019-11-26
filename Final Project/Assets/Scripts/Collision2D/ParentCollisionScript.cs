using UnityEngine;

public class ParentCollisionScript : MonoBehaviour
{
    public void ReportCollisionToParent()
    {
        // Iterate through all children
        for (int i = 0; i < transform.childCount; i++)
        {
            // Change all of their colors to green to show that they are touching
            transform.GetChild(i).GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            transform.GetChild(i).GetComponent<CollisionHull2D>().ToggleCollidingChecker();
        }
    }
}
