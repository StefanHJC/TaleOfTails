using Core.Logic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Cell))]
    public class CellEditor : UnityEditor.Editor
    {
        private Cell _selectedCell;

        [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.InSelectionHierarchy)]
        public static void RenderGizmo(Cell cell, GizmoType type)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(cell.transform.position, .25f);
        }
    }
}