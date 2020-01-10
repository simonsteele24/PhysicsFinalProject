using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BobombAnimationStates
{
    Idle,
    Walking,
}

public class BobombAnimationController : MonoBehaviour
{
    // Animation states
    public BobombAnimationStates currentAnimationState;

    // Game objects
    public GameObject rightFoot;
    public GameObject leftFoot;

    // Floats
    public float angleOfWalk = 10.0f;
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
        // Is the velocity non-zero?
        if (GetComponentInParent<Particle3D>().velocity != Vector3.zero)
        {
            // Then update the animation state to walking
            currentAnimationState = BobombAnimationStates.Walking;
        }
        else
        {
            // Otherwise, update the animation state to idle
            currentAnimationState = BobombAnimationStates.Idle;
        }


        switch (currentAnimationState)
        {
            case BobombAnimationStates.Idle:
                // Keep everything to its original orientation
                rightFoot.transform.position = rightFootOrientation.position;
                rightFoot.transform.rotation = rightFootOrientation.rotation;
                leftFoot.transform.position = leftFootOrientation.position;
                leftFoot.transform.rotation = leftFootOrientation.rotation;
                break;
            default:
                // Continously move each foot in a wave like movement based on velocity
                rightFoot.transform.localEulerAngles = new Vector3(Mathf.Abs(Mathf.Sin((Time.time * new Vector3(GetComponentInParent<Particle3D>().velocity.x, 0, GetComponentInParent<Particle3D>().velocity.z).magnitude) - footOffset)) * angleOfWalk, transform.localEulerAngles.y, transform.localEulerAngles.z);
                leftFoot.transform.localEulerAngles = new Vector3(Mathf.Abs(Mathf.Cos((Time.time * new Vector3(GetComponentInParent<Particle3D>().velocity.x, 0, GetComponentInParent<Particle3D>().velocity.z).magnitude) + footOffset)) * angleOfWalk, transform.localEulerAngles.y, transform.localEulerAngles.z);
                break;
        }
    }
}
