
namespace Core.Logic.UnitAnimator
{
    public interface IAnimatorStateReader
    {
        public AnimatorState State { get; }

        public void OnEnteredState(int stateInfoShortNameHash);
        public void OnExitedState(int stateInfoShortNameHash);
    }
}