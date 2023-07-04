using System;
using Core.Database;
using Core.Logic;
using Core.Unit;
using Core.Unit.Ability;
using UnityEngine;
using Zenject;
using UserInput = UnityEngine.Input; 
using Vector3 = UnityEngine.Vector3;

namespace Core.Services.Input
{
    public class StandaloneInputService : IInputService, ITickable, ILateDisposable
    {
        private const int LMB = 0;
        private const int RMB = 1;
        private const string GridLayerName = "Grid";

        private readonly IRaycastService _raycaster;
        
        private Cell _focusedCell;
        private BaseUnit _selectedUnit;
        private int _meshLayer;

        public BaseUnit SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                _selectedUnit = value;
                UnitSelected?.Invoke();
            }
        }
        public Cell FocusedCell
        {
            get => _raycaster.FocusedCell;
            set
            {
                _focusedCell = value;
                CellFocused?.Invoke(_focusedCell);
            }
        }

        public Vector3 PointerPositionOnGrid
        {
            get
            {
                if (_raycaster.CastRayFromCamera(UserInput.mousePosition, _meshLayer, out RaycastHit hitInfo))
                {
                    return hitInfo.point;
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }

        public event Action SelectButtonPressed;
        public event Action CommandButtonPressed;
        public event Action<BaseAbility> AbilitySelected; // TEMP
        public event Action<Cell> CellFocused;
        public event Action UnitSelected;

        [Inject]
        public StandaloneInputService(IRaycastService raycaster)
        {
            _raycaster = raycaster;
        }

        public void Init()
        {
            EnableRaycaster();
            SubscribeRaycastEvents();
            _meshLayer = 1 << LayerMask.NameToLayer(GridLayerName);
        }

        public void EnableRaycaster()
        {
            _raycaster.Enable();
            _focusedCell = null;
            SubscribeRaycastEvents();
        }

        public void DisableRaycaster()
        {
            _raycaster.Disable();
            _focusedCell = null;
            UnsubscribeRaycastEvents();
        }

        public void Tick()
        {
            HandleLeftMouseButton();
            HandleRightMouseButton();
        }

        public void LateDispose() => UnsubscribeRaycastEvents();

        private void HandleLeftMouseButton()
        {
            if (UserInput.GetMouseButtonDown(LMB))
            {
                SelectButtonPressed?.Invoke();
            }
        }

        private void HandleRightMouseButton()
        {
            if (UserInput.GetMouseButtonDown(RMB))
            {
                CommandButtonPressed?.Invoke();
            }
        }

        private void SubscribeRaycastEvents()
        {
            _raycaster.NewCellFocused += OnCellFocused;
            _raycaster.CellDefocused += OnCellDefocused;
        }

        private void UnsubscribeRaycastEvents()
        {
            _raycaster.NewCellFocused -= OnCellFocused;
            _raycaster.CellDefocused -= OnCellDefocused;
        }

        private void OnCellFocused(Cell cell) => FocusedCell = cell;

        private void OnCellDefocused(Cell cell) => FocusedCell = null;
    }
}