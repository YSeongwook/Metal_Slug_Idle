public interface IFollowerState
{
    void EnterState(FollowerController follower);
    void UpdateState();
    void PhysicsUpdateState();
    void ExitState();
}