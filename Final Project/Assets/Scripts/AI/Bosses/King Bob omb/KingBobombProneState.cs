using UnityEngine;

public class KingBobombProneState : State
{
    // This function is called every frame to check if the state needs to be changed
    public override States CheckForTransition()
    {
        if (!GetComponentInParent<KingBobomb>().CheckIfProne())
        {
            return States.Chase;
        }

        return States.Prone;
    }

    public override void OnEnterState()
    {
        GetComponentInParent<KingBobomb>().bossAnimator.SetTrigger("Flailing");
        
    }

    public override void OnExitState() { }

    public override void UpdateState()
    {
        transform.parent.rotation = Quaternion.Euler(transform.parent.localEulerAngles.x, GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).eulerAngles.y, transform.parent.localEulerAngles.z);
        GetComponentInParent<Particle3D>().rotation = transform.parent.rotation;
    }
}
