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
    public ChainchompAnimationStates currentAnimationState;

    public GameObject upperMouth;
    public GameObject lowerMouth;

    public float angleOfWalk = 10.0f;
    public float mouthOffset = 0.5f;
    public float mouthSpeed = 10.0f;

    Transform upperMouthOrientation;
    Transform lowerMouthOrientation;

    // Start is called before the first frame update
    void Start()
    {
        upperMouthOrientation = upperMouth.transform;
        lowerMouthOrientation = lowerMouth.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponentInParent<Chainchomp>().isAttacking)
        {
            currentAnimationState = ChainchompAnimationStates.Attacking;
        }
        else
        {
            currentAnimationState = ChainchompAnimationStates.Idle;
        }


        switch (currentAnimationState)
        {
            case ChainchompAnimationStates.Idle:
                upperMouth.transform.localEulerAngles = new Vector3(Mathf.Sin((Time.time * mouthSpeed) - mouthOffset) * angleOfWalk, transform.localEulerAngles.y, transform.localEulerAngles.z);
                lowerMouth.transform.localEulerAngles = new Vector3(Mathf.Cos((Time.time * mouthSpeed) + mouthOffset) * angleOfWalk, transform.localEulerAngles.y, transform.localEulerAngles.z);
                break;
            default:
                upperMouth.transform.position = upperMouthOrientation.position;
                upperMouth.transform.rotation = upperMouthOrientation.rotation;
                lowerMouth.transform.position = lowerMouthOrientation.position;
                lowerMouth.transform.rotation = lowerMouthOrientation.rotation;
                break;
        }
    }
}
