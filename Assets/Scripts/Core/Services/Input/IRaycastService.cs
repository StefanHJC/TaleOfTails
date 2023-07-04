using System;
using Core.Logic;
using UnityEngine;

namespace Core.Services.Input
{
    public interface IRaycastService
    {
        public event Action<Cell> NewCellFocused;
        public event Action<Cell> CellDefocused;

        public Cell FocusedCell { get; }

        public void Enable();
        public void Disable();
        public bool CastRayFromCamera(Vector3 to, LayerMask layer, out RaycastHit hitInfo);
    }
}