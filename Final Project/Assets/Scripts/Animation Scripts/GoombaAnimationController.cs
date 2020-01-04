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
    public GoombaAnimationStates currentAnimationState;

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
        if (GetComponentInParent<Goomba>().isDying)
        {
            currentAnimationState = GoombaAnimationStates.Death;
        }
        else if (GetComponentInParent<Particle3D>().velocity != Vector3.zero)
        {
            currentAnimationState = GoombaAnimationStates.Walking;
        }
        else
        {
            currentAnimationState = GoombaAnimationStates.Idle;
        }
        

        switch (currentAnimationState)
        {
            case GoombaAnimationStates.Idle:
                rightFoot.transform.position = rightFootOrientation.position;
                rightFoot.transform.rotation = rightFootOrientation.rotation;
                leftFoot.transform.position = leftFootOrientation.position;
                leftFoot.transform.rotation = leftFootOrientation.rotation;
                break;
            case GoombaAnimationStates.Death:
                transform.localScale = new Vector3(transform.localScale.x,transform.localScale.y * deathCycle, transform.localScale.z);
                break;
            default:
                rightFoot.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.Sin((Time.time *  new Vector3(GetComponentInParent<Particle3D>().velocity.x,0, GetComponentInParent<Particle3D>().velocity.z).magnitude) - footOffset) * angleOfWalk);
                leftFoot.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.Cos((Time.time * new Vector3(GetComponentInParent<Particle3D>().velocity.x, 0, GetComponentInParent<Particle3D>().velocity.z).magnitude) + footOffset) * angleOfWalk);
                break;
        }
    }
}
