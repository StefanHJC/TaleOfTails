using System;
using Core.Logic.UnitAnimator;
using UnityEngine;

namespace Core.Unit
{
    public class BaseUnitAnimator : MonoBehaviour, IAnimatorStateReader
    {
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int MeleeAttack = Animator.StringToHash("MeleeAttack");
        private static readonly int RangeAttack = Animator.StringToHash("RangeAttack");
        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int Ability1 = Animator.StringToHash("Ability1");
        private static readonly int Ability2 = Animator.StringToHash("Ability2");
        private static readonly int Dance = Animator.StringToHash("IsDancing");

        private readonly int _idleStateHash = Animator.StringToHash("Idle");
        private readonly int _moveStateHash = Animator.StringToHash("Move");
        private readonly int _meleeAttackStateHash = Animator.StringToHash("MeleeAttack");
        private readonly int _rangeAttackStateHash = Animator.StringToHash("RangeAttack");
        private readonly int _hitStateHash = Animator.StringToHash("Hit");
        private readonly int _ability1StateHash = Animator.StringToHash("Ability1");
        private readonly int _ability2StateHash = Animator.StringToHash("Ability2");
        private readonly int _danceStateHash = Animator.StringToHash("Dance");
        
        private Animator _animator;

        public AnimatorState State { get; private set; }

        public event Action<AnimatorState> StateEntered;
        public event Action<AnimatorState> StateExited;

        public void PlayHit() => _animator.SetTrigger(Hit);

        public void PlayMeleeAttack() => _animator.SetTrigger(MeleeAttack);

        public void PlayRangeAttack() => _animator.SetTrigger(RangeAttack);
        
        public void PlayAbility1() => _animator.SetTrigger(Ability1); // EXTRA SHIT
        
        public void PlayAbility2() => _animator.SetTrigger(Ability2);
        public void PlayDance() => _animator.SetBool(Dance, true);

        public void StartMoving() => _animator.SetBool(IsMoving, true);

        public void StopMoving() => _animator.SetBool(IsMoving, false);

        public void OnEnteredState(int stateHash)
        {
            State = GetStateFor(stateHash);
            StateEntered?.Invoke(State);
        }

        public void OnExitedState(int stateHash) => StateExited?.Invoke(GetStateFor(stateHash));

        private void Awake() => _animator = GetComponent<Animator>();

        private AnimatorState GetStateFor(int stateHash)
        {
            AnimatorState state;

            if (stateHash == _idleStateHash)
                state = AnimatorState.Idle;
            else if (stateHash == _moveStateHash)
                state = AnimatorState.Move;
            else if (stateHash == _rangeAttackStateHash)
                state = AnimatorState.RangeAttack;
            else if (stateHash == _meleeAttackStateHash)
                state = AnimatorState.MeleeAttack;
            else if (stateHash == _hitStateHash)
                state = AnimatorState.Hit;
            else if (stateHash == _ability1StateHash)
                state = AnimatorState.Ability1;
            else if (stateHash == _ability2StateHash)
                state = AnimatorState.Ability2;
            else if (stateHash == _danceStateHash)
                state = AnimatorState.Dance;
            else
                state = AnimatorState.Idle;

            return state;
        }
    }
}