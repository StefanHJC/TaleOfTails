using Core.Database;
using Core.Logic;
using Core.Logic.Cells;
using UnityEngine;
using Utils;

namespace Editor
{
    public class GridCellWrapper
    {
        [SerializeField] private Cell _cell;

        public Cell Cell => _cell;
        public bool IsReachable => _cell.gameObject.activeInHierarchy;
        public bool HasBehaviour => _cell.GetComponent<CellBehaviour>() != null;
        public int UnitTeamId => _cell.GetComponent<UnitSpawnTag>().TeamId;
        public bool HasUnit => _cell.GetComponent<UnitSpawnTag>() != null;

        public string UnitName
        {
            get
            {
                UnitTypeId unitType = Cell.GetComponent<UnitSpawnTag>().UnitType;
                if (unitType != UnitTypeId.Unknown)
                {
                    string unitName = unitType.ToString();

                    return unitName;
                }

                return null;
            }
        }

        public bool UnitInitialPoint => false; //TODO

        public GridCellWrapper(Cell cell)
        {
            _cell = cell;
        }
    }
}