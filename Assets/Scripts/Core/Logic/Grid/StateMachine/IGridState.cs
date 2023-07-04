namespace Core.Logic.Grid.StateMachine
{
    public interface IGridState
    {
        public void Enter();

        public void Exit();
    }
}