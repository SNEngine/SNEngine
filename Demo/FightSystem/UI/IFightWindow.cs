using SNEngine;
using System;

namespace CoreGame.FightSystem.UI
{
    public interface IFightWindow : IResetable, IShowable, IHidden
    {
        event Action<PlayerAction> OnTurnExecuted;
        void SetData(IFightComponent fightComponentPlayer, IFightComponent fightComponentEnemy, FightCharacter plaerData, FightCharacter enemyData);
        void ShowPanelAction();
        void HidePanelAction();
        void HidePanelSkills();
        void UpdateHealthAndEnergyDisplay(IFightComponent fightComponentPlayer, IFightComponent fightComponentEnemy, FightCharacter playerData, FightCharacter enemyData);
    }
}