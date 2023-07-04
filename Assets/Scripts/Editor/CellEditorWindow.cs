using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Logic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using Core.Database;
using Core.Logic.Cells;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Utils;

namespace Editor
{
    public class CellEditorWindow : OdinEditorWindow
    {
        private const string Title = "Cell Editor";

        [SerializeField, InlineEditor(Expanded = true)]
        [ListDrawerSettings(Expanded = true, CustomRemoveElementFunction = nameof(RemoveBehaviour),
            HideAddButton = true)]
        private List<CellBehaviour> _cellBehaviours = new();

        [HorizontalGroup("Unit")]
        [SerializeField]
        [ReadOnly] 
        [PropertySpace(SpaceAfter = 10f, SpaceBefore = 10f)]
        private UnitTypeId _selectedUnit;

        [SerializeField]
        [ReadOnly]
        private int _selectedUnitTeamId = 0;

        [ReadOnly, PropertyOrder(-10)] public Cell SelectedCell;

        public bool IsCellReachable
        {
            get
            {
                if (SelectedCell == null)
                    return false;

                return SelectedCell.gameObject.activeInHierarchy;
            }
        }

        public bool IsCellSelected => Selection.activeGameObject.TryGetComponent<Cell>(out var _);

        public static void OpenWindow()
        {
            EditorWindow window = GetWindow(typeof(CellEditorWindow));
            window.titleContent.text = Title;
        }

        private void OnSelectionChange()
        {
            if (Selection.activeGameObject == null)
                return;

            if (Selection.activeGameObject.TryGetComponent(out Cell selectedCell))
            {
                SelectedCell = selectedCell;
                _cellBehaviours = GetCellComponents();
                if (SelectedCell.TryGetComponent<UnitSpawnTag>(out UnitSpawnTag unitSpawnTag))
                    _selectedUnit = unitSpawnTag.UnitType;
                else
                    _selectedUnit = UnitTypeId.Unknown;
                IsBehaviourIsAlreadyAttached = false;
            }
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
        }

        private List<Type> GetAttachedToCellComponentTypes()
        {
            if (SelectedCell == null)
                return null;

            List<Type> attachedComponents = new List<Type>();
            IEnumerable<Type> components = GetInheritors(typeof(CellBehaviour));

            foreach (CellBehaviour cellBehaviour in GetCellComponents())
                if (components.Contains(cellBehaviour.GetType()))
                    attachedComponents.Add(cellBehaviour.GetType());

            return attachedComponents;
        }

        private void RemoveBehaviour(CellBehaviour behaviour)
        {
            if (SelectedCell == null)
                return;
            _cellBehaviours.Remove(behaviour);
            DestroyImmediate(behaviour);
        }

        private List<CellBehaviour> GetCellComponents()
        {
            if (SelectedCell == null)
                return null;

            return SelectedCell
                .GetComponents<CellBehaviour>()
                .ToList();
        }

        private IEnumerable<Type> GetInheritors(Type targetType)
        {
            return Assembly
                .GetAssembly(targetType)
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(targetType));
        }

        #region OdinButtons

        [PropertyOrder(-10)]
        [GUIColor(1, .6f, .4f)]
        [Button(ButtonSizes.Medium)]
        [ShowIf("IsCellReachable")]
        private void MakeUnreachable()
        {
            SelectedCell.gameObject.SetActive(false); //TODO disable pointer collider and sprite renderer
            SelectedCell.IsTaken = true;
        }

        [PropertyOrder(-10)]
        [GUIColor(0, 1, 0)]
        [Button(ButtonSizes.Medium)]
        [ShowIf("@IsCellReachable == false")]
        private void MakeReachable()
        {
            SelectedCell.gameObject.SetActive(true);
        }

        [PropertySpace]
        [ValueDropdown("@GetInheritors(typeof(CellBehaviour))")]
        [FoldoutGroup("Attach Behaviour")]
        [InfoBox("Component is already attached", InfoMessageType.Error, "IsBehaviourIsAlreadyAttached")]
        public Type SelectBehaviour;

        [PropertySpace]
        [Button(ButtonSizes.Medium)]
        [FoldoutGroup("Attach Behaviour")]
        private void AttachBehaviour()
        {
            if (SelectBehaviour == null)
                return;

            if (GetAttachedToCellComponentTypes().Contains(SelectBehaviour) == false)
            {
                SelectedCell.gameObject.AddComponent(SelectBehaviour);
                IsBehaviourIsAlreadyAttached = false;
                OnSelectionChange();
            }
            else
            {
                IsBehaviourIsAlreadyAttached = true;
            }
        }

        [PropertySpace] [FoldoutGroup("Attach Unit")]
        public UnitTypeId SelectUnit;

        [ValueDropdown("_teamIds")] [FoldoutGroup("Attach Unit")]
        public int SelectUnitTeamId;

        [PropertySpace]
        [Button(ButtonSizes.Medium)]
        [FoldoutGroup("Attach Unit")]
        private void AttachUnit()
        {
            if (SelectedCell == null || SelectUnit == UnitTypeId.Unknown)
                return;

            if (SelectedCell.TryGetComponent<UnitSpawnTag>(out var unitSpawnTag))
            {
                unitSpawnTag.UnitType = SelectUnit;
                unitSpawnTag.TeamId = SelectUnitTeamId;
            }
            else
            {
                UnitSpawnTag spawnTag = SelectedCell.gameObject.AddComponent<UnitSpawnTag>();
                spawnTag.UnitType = SelectUnit;
                spawnTag.TeamId = SelectUnitTeamId;
            }

            EditorUtility.SetDirty(SelectedCell);

            _selectedUnit = SelectUnit;
        }

        [PropertySpace(SpaceAfter = 10f, SpaceBefore = 10f)]
        [HorizontalGroup("Unit")]
        [Button]
        private void DeleteUnit()
        {
            if (SelectedCell == null)
                return;
            if (SelectedCell.TryGetComponent<UnitSpawnTag>(out var unitSpawnTag))
            {
                DestroyImmediate(unitSpawnTag);
                _selectedUnit = UnitTypeId.Unknown;
            }

            EditorUtility.SetDirty(SelectedCell);
        }

        public bool IsBehaviourIsAlreadyAttached { get; private set; }
        private int[] _teamIds = new int[] { 0, 1, 2 };
        #endregion
    }
}