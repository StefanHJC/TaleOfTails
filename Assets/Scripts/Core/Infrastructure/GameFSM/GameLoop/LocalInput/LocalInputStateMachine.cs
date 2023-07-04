using System;
using System.Collections.Generic;
using Utils.TypeBasedFSM;

namespace Core.Infrastructure.GameFSM.GameLoop.LocalInput
{
    public class LocalInputStateMachine : TypeBasedStateMachine
    {
        private readonly Dictionary<Type, IExitableState> _states;

        public override IReadOnlyDictionary<Type, IExitableState> States => _states;

        public LocalInputStateMachine(IGameFactory gameFactory)
        {
            _states = new Dictionary<Type, IExitableState>
            {
                [typeof(MeleeAttackCommandState)] = gameFactory.CreateState<MeleeAttackCommandState>(),
                [typeof(RangeAttackCommandState)] = gameFactory.CreateState<RangeAttackCommandState>(),
                [typeof(MoveCommandState)] = gameFactory.CreateState<MoveCommandState>(),
                [typeof(NullCommandState)] = gameFactory.CreateState<NullCommandState>(),
                [typeof(AbilityInputState)] = gameFactory.CreateState<AbilityInputState>(this),
            };
        }
    }
}