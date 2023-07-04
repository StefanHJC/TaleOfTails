using System.Collections.Generic;
using Core.Logic.Grid.StateMachine;
using UnityEngine;

namespace Core.Logic.Grid
{
    public abstract class AbstractGrid : MonoBehaviour
    {
        [SerializeField] private UnitsOnGrid _units;
        [SerializeField] private GameObject _gridCenter;

        public GameObject GridCenter
        {
            get => _gridCenter;
            set => _gridCenter = value;
        }

        public abstract GridStateMachine StateMachine { get; }
        public abstract Cell[,] Cells { get; }
        public UnitsOnGrid Units => _units;

        public abstract List<Cell> GetCellNeighbours(Cell origin);

        public abstract List<Cell> GetCellsInRange(Cell center, int range);
    }
}