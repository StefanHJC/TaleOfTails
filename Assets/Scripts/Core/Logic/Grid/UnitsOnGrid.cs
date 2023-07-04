using System;
using System.Collections.Generic;
using Core.Unit;
using Core.Unit.Component;
using UnityEngine;

namespace Core.Logic.Grid
{
    [RequireComponent(typeof(AbstractGrid))]
    public class UnitsOnGrid : MonoBehaviour
    {
        [SerializeField] private AbstractGrid _grid;

        private readonly List<BaseUnit> _unitList = new();
        private readonly Dictionary<Cell, BaseUnit> _dictionary = new();

        public IReadOnlyList<BaseUnit> List
        {
            get
            {
                if (_unitList.Count < _dictionary.Count)
                {
                    GetUnits();
                }

                return _unitList;
            }
        }

        public event Action<BaseUnit> UnitDied;

        public bool TryGetUnit(Cell from, out BaseUnit unit)
        {
            if (_dictionary.ContainsKey(from))
            {
                unit = _dictionary[from];

                return true;
            }

            unit = null;

            return false;
        }

        private void Start()
        {
            SubscribeCellEvents();
            GetUnits();
        }

        private void OnDestroy() => UnsubscribeCellEvents();

        private void GetUnits()
        {
            foreach (BaseUnit unit in _dictionary.Values)
            {
                if (_unitList.Contains(unit))
                {
                    continue;
                }

                _unitList.Add(unit);
                UnitDeath deathComponent = unit.GetComponent<UnitDeath>();
                deathComponent.Died += OnUnitDeath;
            }
        }

        private void OnUnitDeath(BaseUnit unit)
        {
            _dictionary.Remove(unit.CurrentCell);
            _unitList.Remove(unit);

            UnitDied?.Invoke(unit);
            unit.CurrentCell.Corpse = unit;
            unit.CurrentCell.IsTaken = false;
        }

        private void SubscribeCellEvents()
        {
            foreach (var cell in _grid.Cells)
            {
                cell.Entered += OnUnitEnterCell;
                cell.Exited += OnUnitExitCell;
            }
        }

        private void UnsubscribeCellEvents()
        {
            foreach (Cell cell in _grid.Cells)
            {
                cell.Entered -= OnUnitEnterCell;
                cell.Exited -= OnUnitExitCell;
            }
        }

        private void OnUnitEnterCell(BaseUnit unit, Cell cell)
        {
            _dictionary.Add(cell, unit);
            cell.IsTaken = true;
            ValidateUnitList(unit);
        }

        private void OnUnitExitCell(BaseUnit unit, Cell cell)
        {
            _dictionary.Remove(cell, out _);
            cell.IsTaken = false;
            ValidateUnitList(unit);
        }

        private void ValidateUnitList(BaseUnit unit)
        {
            if (_unitList.Contains(unit) == false)
                GetUnits();
        }
    }
}