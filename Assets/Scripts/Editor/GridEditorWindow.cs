using System;
using Core.Logic;
using Core.Logic.Grid;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class GridEditorWindow : OdinEditorWindow
    {
        private const string TitleLabel = "Grid Editor";

        private AbstractGrid _grid;
        private Cell[,] _gridCells;
        private Vector2Int _gridSize;

        [TableMatrix(
            HorizontalTitle = "Matrix grid view",
            RowHeight = 40,
            IsReadOnly = true,
            DrawElementMethod = nameof(DrawCellMatrixView)
        )]
        public GridCellWrapper[,] _gridMatrixView;

        [MenuItem("Tools/Grid Editor")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow(typeof(GridEditorWindow));
            window.titleContent.text = TitleLabel;
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
        }

        protected override void OnEnable()
        {
            _grid = GetGrid();
            _gridCells = GetGridCells();
            ConstructGridView();
        }

        private static GridCellWrapper DrawCellMatrixView(Rect rect, GridCellWrapper cell)
        {
            EditorGUI.DrawRect(rect.Padding(1), cell.IsReachable ? new Color(.05f, .5f, .05f) : new Color(0.6f, .3f, .3f));

            if (cell.HasBehaviour)
            {
                Rect temp = rect.AlignTop(15f);
                EditorGUI.DrawRect(temp, new Color(.1f, .1f, .6f));
                EditorGUI.LabelField(temp.AlignMiddle(10f), "Behaviour");
            }

            if (cell.HasUnit)
            {
                Rect temp = rect.AlignBottom(15f);
                EditorGUI.DrawRect(temp, GetTeamColor(cell));
                EditorGUI.LabelField(temp.AlignMiddle(10f), cell.UnitName);
            }

            if (Selection.activeGameObject == cell.Cell.gameObject)
            {
                EditorGUI.DrawRect(rect.AlignCenter(10f).AlignMiddle(10f), new Color (.8f, .8f, 0));
            }

            if (rect.Contains(Event.current.mousePosition))
            {
                EditorGUI.LabelField(rect.AlignCenter(10f), "*");
                GUI.changed = true;
            }
           
            

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                Selection.SetActiveObjectWithContext(cell.Cell, null);
                Selection.selectionChanged.Invoke();
                GUI.changed = true;
                Event.current.Use();
                CellEditorWindow.OpenWindow();
            }

            return cell;
        }

        private static Color GetTeamColor(GridCellWrapper cell)
        {
            Color teamColor = Color.green;

            switch (cell.UnitTeamId)
            {
                case 0:
                    teamColor = new Color(0, .2f, 0);
                    break;
                case 1:
                    teamColor = Color.red;
                    break;
                case 2:
                    teamColor = Color.black;
                    break;
            }

            return teamColor;
        }

        private void ConstructGridView()
        {
            _gridMatrixView = new GridCellWrapper[_gridSize.x, _gridSize.y];

            for (int i = 0; i < _gridSize.x; i++)
            {
                for (int j = 0; j < _gridSize.y; j++)
                {
                    _gridMatrixView[i, j] = new GridCellWrapper(_gridCells[i, j]);
                }
            }
        }

        private AbstractGrid GetGrid() => FindObjectOfType<AbstractGrid>();

        private Cell[,] GetGridCells()
        {
            if (_grid == null)
            {
                Debug.Log("В сцене отсутствует объект с AbstractGrid");
                return null;
            }

            int maxX = 0, maxY = 0;

            for (int i = 0; i < _grid.transform.childCount; i++)
            {
                var hexagon = _grid.transform.GetChild(i).gameObject.GetComponent<Hexagon>();
                maxX = Math.Max(maxX, hexagon.OffsetCoordinates.x);
                maxY = Math.Max(maxY, hexagon.OffsetCoordinates.y);
            }

            _gridSize = new Vector2Int(maxX + 1, maxY + 1);
            var cells = new Cell[_gridSize.x, _gridSize.y];

            for (int i = 0; i < _grid.transform.childCount; i++)
            {
                var hexagon = _grid.transform.GetChild(i).gameObject.GetComponent<Hexagon>();
                cells[hexagon.OffsetCoordinates.x, hexagon.OffsetCoordinates.y] = hexagon;
            }

            return cells;
        }
    }
}