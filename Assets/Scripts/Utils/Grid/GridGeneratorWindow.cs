/*using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Utils.Grid
{
    public class GridGeneratorWindow : EditorWindow
    {
        private readonly List<string> _generationMethodsNames = new();
        private HexagonalGridGenerator _gridGenerator;
        private Dictionary<string, object> _parameterValues = new Dictionary<string, object>();
        private MemberInfo[] _generationMethods;
        private int _selectedMethodId;
        private int _radius;
        private int _width;
        private int _height;

        [MenuItem("Tools/Grid Generator")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow(typeof(GridGeneratorWindow));
            window.titleContent.text = "Grid Generator";
        }

        public void OnEnable()
        {
            _gridGenerator = InstantiateGridGenerator();
            GetGenerationMethods();
        }

        public void OnGUI()
        {
            RenderGridGeneratorGUI();
        }

        public void OnDestroy()
        {
            if (_gridGenerator != null)
                DestroyImmediate(_gridGenerator.gameObject);
        }

        private void RenderGridGeneratorGUI()
        {
            GUILayout.Label("Grid generation", EditorStyles.boldLabel);
            GUILayout.Label("Grid", EditorStyles.boldLabel);
            _selectedMethodId = EditorGUILayout.Popup("Generation method", _selectedMethodId,
                _generationMethodsNames.ToArray());
            HexGridType selectedHexGridType = (HexGridType)_selectedMethodId;
            switch (selectedHexGridType)
            {
                case HexGridType.RECTANGLE:
                    _width = EditorGUILayout.IntField(new GUIContent("Width"), _width);
                    _height = EditorGUILayout.IntField(new GUIContent("Height"), _height);
                    _gridGenerator.Height = _height;
                    _gridGenerator.Width = _width;
                    break;
                case HexGridType.HEX:
                    _radius = EditorGUILayout.IntField(new GUIContent("Radius"), _radius);
                    _gridGenerator.Radius = _radius;
                    break;
            }

            if (GUILayout.Button("Generate grid"))
                GenerateGrid(selectedHexGridType);
        }

        private HexagonalGridGenerator InstantiateGridGenerator() =>
            Instantiate(Resources.Load("Prefabs/HexagonalGridGenerator"),
                Vector3.zero, Quaternion.identity).GetComponent<HexagonalGridGenerator>();

        private void GenerateGrid(HexGridType selectedHexGridType)
        {
            _gridGenerator.GetGrid(OnGridGenerated, selectedHexGridType);
        }

        private void OnGridGenerated()
        {
        }

        private void GetGenerationMethods()
        {
            _generationMethods = typeof(IGridGenerator).GetMembers();
            foreach (var generationMethod in _generationMethods)
            {
                if (generationMethod.Name != "GetGrid")
                    _generationMethodsNames.Add(generationMethod.Name.Substring("InstantiateHexagons".Length));
            }
        }
    }
}*/