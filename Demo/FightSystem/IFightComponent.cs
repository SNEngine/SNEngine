using CoreGame.FightSystem.HealthSystem;
using CoreGame.FightSystem.ManaSystem;

namespace CoreGame.FightSystem
{
    public interface IFightComponent
    {
        IHealthComponent HealthComponent { get; }
        IManaComponent ManaComponent { get; }
        bool IsGuarding { get; }
        FightCharacter FightCharacter { get; }

        void AddComponents();
        void SetFightCharacter(FightCharacter character);
        void StartGuard();
        void StopGuard();
    }
}