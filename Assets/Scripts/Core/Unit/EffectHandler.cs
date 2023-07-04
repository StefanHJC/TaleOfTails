using Core.Unit.Component;
using UnityEngine;

namespace Core.Unit
{
    public class EffectHandler : MonoBehaviour
    {
        [SerializeField] private BaseUnit _unit;
        [SerializeField] private UnitMove _unitMovement;

        private int _roundsRemained;
        private bool _hasEffect;

        public bool HasEffect => _hasEffect;

        public void ReduceSpeed(int durationInRounds)
        {
            _roundsRemained = durationInRounds;
            _unit.Speed = 0;
            _unit.ActionPoints = 0;
            _hasEffect = true;
        }

        public void OnTurnEnd()
        {
            if (_hasEffect)
            {
                _roundsRemained--;
                _unit.Speed = 0;
                _unit.ActionPoints = 0;

                if (_roundsRemained <= 0)
                {
                    _hasEffect = false;
                }
            }

            if (_roundsRemained <= 0)
            {
                _hasEffect = false;
                _unit.Speed = _unit.Config.Speed;
                _unit.ActionPoints = _unit.Config.Speed;
            }
            else
            {
                _unit.Speed = 0;
                _unit.ActionPoints = 0;
                _hasEffect = true;
            }
        }
    }
}