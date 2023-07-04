using System;
using Core.Logic;
using Core.Unit;
using Core.Unit.Ability;
using UnityEngine;

namespace Core.Services.Input
{
    public interface IInputService : IService
    {
        public BaseUnit SelectedUnit { get; set; }
        public Cell FocusedCell { get; set; }
        public Vector3 PointerPositionOnGrid { get; }

        public event Action SelectButtonPressed;
        public event Action CommandButtonPressed;
        public event Action<BaseAbility> AbilitySelected;
        public event Action<Cell> CellFocused;
        public event Action UnitSelected;

        public void Init();

        public void EnableRaycaster();
        public void DisableRaycaster();
    }
}