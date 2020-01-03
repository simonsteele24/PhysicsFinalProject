using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    public abstract States CheckForTransition();
    public abstract void UpdateState();
    public abstract void OnEnterState();
    public abstract void OnExitState();
}
