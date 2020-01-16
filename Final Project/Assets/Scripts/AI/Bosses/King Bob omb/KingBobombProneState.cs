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

    public override void OnEnterState() { }

    public override void OnExitState() { }

    public override void UpdateState() { }
}
