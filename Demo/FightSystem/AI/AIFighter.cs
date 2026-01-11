using CoreGame.FightSystem;
using CoreGame.FightSystem.Abilities;
using CoreGame.FightSystem.AI;
using CoreGame.FightSystem.Models;
using System.Collections.Generic;

namespace CoreGame.FightSystem.AI
{
    public class AIFighter
    {
        private FightCharacter _self;
        private IFightComponent _selfComponent;
        private IFightComponent _targetComponent;
        private AIEntity _aiEntity;

        public AIFighter(FightCharacter self, IFightComponent selfComponent,
                         IFightComponent targetComponent, ScriptableAI scriptableAI)
        {
            _self = self;
            _selfComponent = selfComponent;
            _targetComponent = targetComponent;
            _aiEntity = new AIEntity(scriptableAI);
        }

        public FightCharacter Self => _self;
        public IFightComponent SelfComponent => _selfComponent;
        public IFightComponent TargetComponent => _targetComponent;
        public AIEntity AIEntity => _aiEntity;

        public AIDecision DecideAction(IReadOnlyList<AbilityEntity> availableAbilities, float currentEnergy)
        {
            return _aiEntity.DecideAction(_selfComponent, _targetComponent, _self, availableAbilities, currentEnergy);
        }
    }
}