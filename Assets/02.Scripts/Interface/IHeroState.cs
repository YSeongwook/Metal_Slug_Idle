public interface IHeroState
{
    void EnterState(HeroController hero);
    void UpdateState();
    void PhysicsUpdateState();
    void ExitState();
}