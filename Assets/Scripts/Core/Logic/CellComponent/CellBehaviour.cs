using Core.Unit;
using UnityEngine;

namespace Core.Logic.Cells
{
    [RequireComponent(typeof(Cell))]
    public abstract class CellBehaviour : MonoBehaviour
    {
        private Cell _cell;

        public abstract string EditorLabel { get; }

        protected virtual void OnUnitEnter(BaseUnit unit, Cell cell) {}     

        protected virtual void OnUnitExit(BaseUnit unit, Cell cell) {}

        private void Start()
        {
            _cell = GetComponent<Cell>();
            SubscribeCellEvents();
        }

        private void OnDestroy() => UnsubscribeCellEvents();

        private void UnsubscribeCellEvents()
        {
            _cell.Entered -= OnUnitEnter;
            _cell.Exited -= OnUnitExit;
        }

        private void SubscribeCellEvents()
        {
            _cell.Entered += OnUnitEnter;
            _cell.Exited += OnUnitExit;
        }
    }
}