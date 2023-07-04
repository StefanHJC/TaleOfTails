using System;
using Core.Logic;
using Core.Services;
using Core.Unit.Command;
using FMODUnity;
using UnityEngine;
using Zenject;

namespace Core.Unit.Component
{
    public class UnitMove : MonoBehaviour, IUnitCommandHandler
    {
        [SerializeField] private BaseUnit _unit;
        [SerializeField] private BaseUnitAnimator _animator;
        [SerializeField] private float _speed;

        private bool _isMoving;
        private Cell _destination;

        public event Action Executed;

        public CommandTypeId HandlerType => CommandTypeId.Move;
        
        public void Execute(IUnitCommand command, Action onExecuted)
        { 
            MoveCommand moveCommand = (MoveCommand)command;

            Move(moveCommand.Target);
            Executed = onExecuted;
        }

        public void Move(Cell to)
        {
            _isMoving = true;
            _destination = to;
            _animator.StartMoving();
        }

        private void Update()
        {
            if (_isMoving)
                _unit.transform.position = Vector3.MoveTowards(_unit.transform.position, _destination.transform.position, _speed * Time.deltaTime);

            if (IsDestinationReached())
            {
                _isMoving = false;
                _animator.StopMoving();
                _unit.ActionPoints -= 1;
                Executed?.Invoke();
            }
        }

        private bool IsDestinationReached() => _isMoving && _unit.transform.position == _destination.transform.position;

        private void OnFootstep() => _unit.Sound.PlayOnce(_unit.Config.FootstepSFX, transform.position);
    }
}