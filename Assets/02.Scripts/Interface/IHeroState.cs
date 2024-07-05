public interface IHeroState
{
    void EnterState(HeroController hero);
    void UpdateState();
    void ExitState();
}