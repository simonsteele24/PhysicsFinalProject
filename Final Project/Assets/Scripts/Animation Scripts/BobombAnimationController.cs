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
    public BobombAnimationStates currentAnimationState;

    public GameObject rightFoot;
    public GameObject leftFoot;

    public float angleOfWalk = 10.0f;
    public float footOffset = 0.5f;
    public float deathCycle = 0.99f;

    Transform rightFootOrientation;
    Transform leftFootOrientation;

    // Start is called before the first frame update
    void Start()
    {
        rightFootOrientation = rightFoot.transform;
        leftFootOrientation = leftFoot.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponentInParent<Particle3D>().velocity != Vector3.zero)
        {
            currentAnimationState = BobombAnimationStates.Walking;
        }
        else
        {
            currentAnimationState = BobombAnimationStates.Idle;
        }


        switch (currentAnimationState)
        {
            case BobombAnimationStates.Idle:
                rightFoot.transform.position = rightFootOrientation.position;
                rightFoot.transform.rotation = rightFootOrientation.rotation;
                leftFoot.transform.position = leftFootOrientation.position;
                leftFoot.transform.rotation = leftFootOrientation.rotation;
                break;
            default:
                rightFoot.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.Abs(Mathf.Sin((Time.time * new Vector3(GetComponentInParent<Particle3D>().velocity.x, 0, GetComponentInParent<Particle3D>().velocity.z).magnitude) - footOffset)) * angleOfWalk);
                leftFoot.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.Abs(Mathf.Cos((Time.time * new Vector3(GetComponentInParent<Particle3D>().velocity.x, 0, GetComponentInParent<Particle3D>().velocity.z).magnitude) + footOffset)) * angleOfWalk);
                break;
        }
    }
}
