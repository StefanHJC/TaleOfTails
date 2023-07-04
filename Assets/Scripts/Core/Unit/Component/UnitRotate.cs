using System;
using Core.Logic;
using Core.Unit.Command;
using UnityEngine;
using Utils.Comparers;

namespace Core.Unit.Component
{
    public class UnitRotate : MonoBehaviour, IUnitCommandHandler
    {
        [SerializeField] private BaseUnit _unit;
        [SerializeField] private float _rotationSpeed;

        private Quaternion _targetRotation;
        private bool _isRotate;
        private float _timeCount;

        public CommandTypeId HandlerType => CommandTypeId.Rotate;

        public event Action Executed;

        public void Execute(IUnitCommand command, Action onExecuted)
        {
            RotateCommand rotateCommand = (RotateCommand)command;

            Rotate(rotateCommand.Target);
            Executed = onExecuted;
        }

        public void Rotate(Cell to)
        {
            _targetRotation = Quaternion.LookRotation(to.transform.position - _unit.transform.position);
            _timeCount = 0;
            _isRotate = true;
        }

        private void Update()
        {
            if (!_isRotate) return;

            _unit.transform.rotation =
                Quaternion.Lerp(_unit.transform.rotation, _targetRotation, _timeCount * _rotationSpeed);
            _timeCount += Time.deltaTime;
            _isRotate = QuaternionComparer.Compare(_targetRotation, _unit.transform.rotation) != 0;

            if (!_isRotate)
                Executed?.Invoke();
        }
    }
}