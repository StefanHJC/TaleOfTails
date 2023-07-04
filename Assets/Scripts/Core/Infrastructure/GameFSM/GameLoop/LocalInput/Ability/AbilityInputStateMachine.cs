using System;
using System.Collections.Generic;
using Utils.TypeBasedFSM;

namespace Core.Infrastructure.GameFSM.GameLoop.LocalInput
{
    public class AbilityInputStateMachine : TypeBasedStateMachine
    {
        private readonly Dictionary<Type, IExitableState> _states;

        public override IReadOnlyDictionary<Type, IExitableState> States => _states;

        public AbilityInputStateMachine(IGameFactory gameFactory)
        {
            _states = new Dictionary<Type, IExitableState>
            {
                [typeof(AreaOfEffectState)] = gameFactory.CreateState<AreaOfEffectState>(),
                [typeof(TargetState)] = gameFactory.CreateState<TargetState>(),
                [typeof(NullAbilityState)] = gameFactory.CreateState<NullAbilityState>(),
            };
        }
    }
}