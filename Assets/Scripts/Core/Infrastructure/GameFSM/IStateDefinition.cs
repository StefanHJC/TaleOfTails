using Utils.TypeBasedFSM;

namespace Core.Infrastructure.GameFSM
{
    public interface IStateDefinition<out TState> where TState : class, IExitableState
    {
        public TState GetState();
    }
}