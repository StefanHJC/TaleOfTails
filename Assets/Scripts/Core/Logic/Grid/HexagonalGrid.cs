using System;
using System.Collections.Generic;
using Core.Logic.Grid.StateMachine;
using Core.Services;
using Core.Services.Input;
using UnityEngine;
using Zenject;

namespace Core.Logic.Grid
{
    public class HexagonalGrid : AbstractGrid
    {
        private IPathRenderer _pathRenderer;
        private Cell[,] _cells;
        private Vector2Int _gridSize;
        private GridStateMachine _stateMachine;
        private ICellHighlighter _highlighter;
        private IUnitSelector _selector;
        private IInputService _input;
        private ITurnResolver _turnResolver;
        private IUnitActionsResolver _unitActions;

        public override GridStateMachine StateMachine => _stateMachine;
        public override Cell[,] Cells => _cells;

        [Inject]
        public void Construct(ICellHighlighter highlighter, IUnitSelector selector, IInputService input,
            IPathRenderer pathRenderer, ITurnResolver turnResolver, IUnitActionsResolver unitActions)
        {
            _pathRenderer = pathRenderer;
            _highlighter = highlighter;
            _selector = selector;
            _input = input;
            _turnResolver = turnResolver;
            _unitActions = unitActions;
        }

        private void Awake() => Init();

        private void Init() // TODO rework to zenject injection via gameFactory
        {
            _stateMachine = new GridStateMachine(_input, _highlighter, _selector, Units, _pathRenderer, _turnResolver, _unitActions);
            _cells = GetCells();
        }

        public override List<Cell> GetCellNeighbours(Cell origin)
        {
            var neighbours = new List<Cell>();
            var axialDirectionVectors = new List<Vector2Int>
            {
                new(1, 0),
                new Vector2Int(-1, 1) * (int)Math.Pow(-1, origin.OffsetCoordinates.y & 1),
                new(0, -1),
                new Vector2Int(-1, -1) * (int)Math.Pow(-1, origin.OffsetCoordinates.y & 1),
                new(-1, 0),
                new(0, 1)
            };

            foreach (var directionVector in axialDirectionVectors)
            {
                int x = origin.OffsetCoordinates.x + directionVector.x;
                int y = origin.OffsetCoordinates.y + directionVector.y;

                if (x >= 0 && y >= 0 && x < _gridSize.x && y < _gridSize.y && _cells[x, y] != null)
                {
                    neighbours.Add(_cells[x, y]);
                }
            }

            return neighbours;
        }

        public override List<Cell> GetCellsInRange(Cell center, int range)
        {
            Vector2Int centerVector = OffsetToAxis(center.OffsetCoordinates);
            var cells = new List<Cell>();
            for (int i = -range; i <= range; i++)
            {
                for (int j = Math.Max(-range, -i - range); j <= Math.Min(range, -i + range); j++)
                {
                    Vector2Int cell = AxisToOffset(new Vector2Int(centerVector.x + i, centerVector.y + j));
                    if (cell is { x: >= 0, y: >= 0 } && cell.x < _gridSize.x && cell.y < _gridSize.y &&
                        _cells[cell.x, cell.y] != null)
                    {
                        cells.Add(_cells[cell.x, cell.y]);
                    }
                }
            }

            return cells;
        }

        private Cell[,] GetCells()
        {
            int maxX = 0, maxY = 0;

            for (int i = 0; i < transform.childCount; i++)
            {
                var hexagon = transform.GetChild(i).gameObject.GetComponent<Hexagon>();
                maxX = Math.Max(maxX, hexagon.OffsetCoordinates.x);
                maxY = Math.Max(maxY, hexagon.OffsetCoordinates.y);
            }

            _gridSize = new Vector2Int(maxX + 1, maxY + 1);
            var cells = new Cell[_gridSize.x, _gridSize.y];

            for (int i = 0; i < transform.childCount; i++)
            {
                var hexagon = transform.GetChild(i).gameObject.GetComponent<Hexagon>();
                cells[hexagon.OffsetCoordinates.x, hexagon.OffsetCoordinates.y] = hexagon;
            }

            return cells;
        }

        private Vector2Int OffsetToAxis(Vector2Int offsetCoordinate)
        {
            int q = offsetCoordinate.x - (offsetCoordinate.y - (offsetCoordinate.y & 1)) / 2;
            return new Vector2Int(q, offsetCoordinate.y);
        }

        private Vector2Int AxisToOffset(Vector2Int axisCoordinate)
        {
            int col = axisCoordinate.x + (axisCoordinate.y - (axisCoordinate.y & 1)) / 2;
            return new Vector2Int(col, axisCoordinate.y);
        }
    }
}