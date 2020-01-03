using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum States
{
    Idle,
    Chase,
    CompleteIdle,
    Lunge,
    Return,
    IdleBeforeReturn,
    Prone,
    Throw
}

public class GoombaController : MonoBehaviour
{
    public State[] states;
    public States currentState;

    // Start is called before the first frame update
    void Start()
    {
        states[(int)currentState].OnEnterState();
    }

    // Update is called once per frame
    void Update()
    {
        states[(int)currentState].UpdateState();
        States nextState = states[(int)currentState].CheckForTransition();

        if (nextState != currentState)
        {
            states[(int)currentState].OnExitState();
            currentState = nextState;
            states[(int)currentState].OnEnterState();
        }
    }
}
