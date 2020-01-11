using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoombaAnimationStates
{
    Idle,
    Walking,
    Death
}

public class GoombaAnimationController : MonoBehaviour
{
    // Animation States
    public GoombaAnimationStates currentAnimationState;

    // Gameobjects
    public GameObject rightFoot;
    public GameObject leftFoot;

    // Floats
    public float angleOfWalk = 10.0f;
    public float speedMagnifier = 2.0f;
    public float footOffset = 0.5f;
    public float deathCycle = 0.99f;

    // Transforms
    Transform rightFootOrientation;
    Transform leftFootOrientation;

    // Start is called before the first frame update
    void Start()
    {
        // Get the current right and left foot orientations of the Object
        rightFootOrientation = rightFoot.transform;
        leftFootOrientation = leftFoot.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Is the Gameobject currently dying?
        if (GetComponentInParent<Goomba>().isDying)
        {
            // If yes, then set the animation state to dead
            currentAnimationState = GoombaAnimationStates.Death;
        }
        // Is the velocity non-zero?
        else if (GetComponentInParent<Particle3D>().velocity != Vector3.zero)
        {
            // Then update the animation state to walking
            currentAnimationState = GoombaAnimationStates.Walking;
        }
        else
        {
            // Otherwise, update the animation state to idle
            currentAnimationState = GoombaAnimationStates.Idle;
        }
        

        switch (currentAnimationState)
        {
            case GoombaAnimationStates.Idle:
                // Keep everything to its original orientation
                rightFoot.transform.position = rightFootOrientation.position;
                rightFoot.transform.rotation = rightFootOrientation.rotation;
                leftFoot.transform.position = leftFootOrientation.position;
                leftFoot.transform.rotation = leftFootOrientation.rotation;
                break;
            case GoombaAnimationStates.Death:
                // Scrunch the object to look like it has been squished
                transform.localScale = new Vector3(transform.localScale.x,transform.localScale.y * deathCycle, transform.localScale.z);
                break;
            default:
                // Continously move each foot in a wave like movement based on velocity
                rightFoot.transform.localEulerAngles = new Vector3(Mathf.Sin((Time.time *  speedMagnifier)) * angleOfWalk, transform.localEulerAngles.y, transform.localEulerAngles.z);
                leftFoot.transform.localEulerAngles = new Vector3(Mathf.Cos((Time.time * new Vector3(GetComponentInParent<Particle3D>().velocity.x, 0, GetComponentInParent<Particle3D>().velocity.z).magnitude * speedMagnifier) + footOffset) * angleOfWalk, transform.localEulerAngles.y, transform.localEulerAngles.z);
                break;
        }
    }
}
