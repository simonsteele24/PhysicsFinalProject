using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChainchompAnimationStates
{
    Idle,
    Attacking,
}

public class ChainchompAnimationController : MonoBehaviour
{
    // Animation States
    public ChainchompAnimationStates currentAnimationState;

    // Game objects
    public GameObject upperMouth;
    public GameObject lowerMouth;

    // Floats
    public float angleOfWalk = 10.0f;
    public float mouthOffset = 0.5f;
    public float mouthSpeed = 10.0f;

    public float upperMouthWideOpen;
    public float lowerMouthWideOpen;

    // Transforms
    Quaternion upperMouthOrientation;
    Quaternion lowerMouthOrientation;

    // Start is called before the first frame update
    void Start()
    {
        // Get the starting orientation of the upper and lower mouth
        upperMouthOrientation = upperMouth.transform.rotation;
        lowerMouthOrientation = lowerMouth.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        // Is the chain chomp currently attacking?
        if (GetComponentInParent<Chainchomp>().isAttacking)
        {
            // If yes, then set the state to attacking
            currentAnimationState = ChainchompAnimationStates.Attacking;
        }
        else
        {
            // Otherwise set it to idle
            currentAnimationState = ChainchompAnimationStates.Idle;
        }


        switch (currentAnimationState)
        {
            case ChainchompAnimationStates.Idle:
                // Continously move the mouths up and down to show chomping
                upperMouth.transform.localEulerAngles = new Vector3(Mathf.Sin((Time.time * mouthSpeed) - mouthOffset) * angleOfWalk, transform.localEulerAngles.y, transform.localEulerAngles.z);
                lowerMouth.transform.localEulerAngles = new Vector3(Mathf.Cos((Time.time * mouthSpeed) + mouthOffset) * angleOfWalk, transform.localEulerAngles.y, transform.localEulerAngles.z);
                break;
            default:
                // Set the mouth to be wide open, ready to bite
                upperMouth.transform.localEulerAngles = new Vector3(upperMouthWideOpen, transform.localEulerAngles.y, transform.localEulerAngles.z);
                lowerMouth.transform.localEulerAngles = new Vector3(lowerMouthWideOpen, transform.localEulerAngles.y, transform.localEulerAngles.z);
                break;
        }
    }
}
