/*using System;
using Core.Data;
using Core.Logic;
using Core.Logic.Grid;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

namespace Utils.Grid
{
    [ExecuteInEditMode]
    public class HexagonalGridGenerator : MonoBehaviour, IGridGenerator
#if UNITY_EDITOR
    {
        private const float XOffset = 470;
        private const float ZOffset = 404;

        private float _xOffset;
        private float _zOffset;

        [SerializeField] private Hexagon _hexagon;
        [SerializeField] private HexagonalGrid _grid;
        [SerializeField] private Sprite _hexagonSprite;

        #region GridEditorFields

        public int Width = 10;
        public int Height = 10;
        public int Radius = 10;

        #endregion

        [Button]
        public GridData GetGrid(Action onGenerated, HexGridType selectedHexGridType)
        {
            _xOffset = XOffset / _hexagonSprite.pixelsPerUnit;
            _zOffset = ZOffset / _hexagonSprite.pixelsPerUnit;
            Debug.Log("Generate grid!");
            DestroyPreviousGridIfExists();
            InstantiateGrid(selectedHexGridType);
            onGenerated.Invoke();
            return null;
        }

        private void DestroyPreviousGridIfExists()
        {
            HexagonalGrid previousGrid = FindObjectOfType<HexagonalGrid>();

            if (previousGrid != null)
                DestroyImmediate(previousGrid?.gameObject, allowDestroyingAssets: true);
        }

        private void InstantiateGrid(HexGridType selectedHexGridType)
        {
            switch (selectedHexGridType)
            {
                case HexGridType.RECTANGLE:
                    InstantiateHexagonsRectangle(Instantiate(_grid.gameObject, Vector3.zero, Quaternion.identity));
                    break;
                case HexGridType.HEX:
                    InstantiateHexagonsHex(Instantiate(_grid.gameObject, Vector3.zero, Quaternion.identity));
                    break;
            }
        }

        public void InstantiateHexagonsRectangle(GameObject gridParent)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    float xPos = i * _xOffset;

                    if (j % 2 == 1)
                    {
                        xPos += _xOffset / 2f;
                    }

                    GameObject hexagonPrefab = Instantiate(_hexagon.gameObject, new Vector3(xPos, .1f, j * _zOffset),
                        Quaternion.Euler(90f, 0, 90f), gridParent.transform);
                    hexagonPrefab.name = $"hexagon[{i},{j}]";

                    if (i == Height / 2 && j == Width / 2)
                    {
                        gridParent.GetComponent<AbstractGrid>().GridCenter = hexagonPrefab;
                    }

                    SetHexagonOffsetCoords(hexagonPrefab, x: i, y: j);
                }
            }
        }

        public void InstantiateHexagonsHex(GameObject gridParent)
        {
            for (int i = 0; i < Radius * 2 + 1; i++)
            {
                for (int j = 0; j < Radius * 2 + 1; j++)
                {
                    if (i + j < Radius || i + j > 3 * Radius)
                    {
                        continue; //null hex                        
                    }

                    float xPos = (i + j / 2 - 1) * _xOffset;

                    if (j % 2 == 1)
                    {
                        xPos += _xOffset / 2f;
                    }

                    GameObject hexagonPrefab = Instantiate(_hexagon.gameObject, new Vector3(xPos, .1f, j * _zOffset),
                        Quaternion.Euler(90f, 0, 90f), gridParent.transform);
                    hexagonPrefab.name = $"hexagon[{i},{j}]";

                    if (i == Radius && j == Radius)
                    {
                        gridParent.GetComponent<AbstractGrid>().GridCenter = hexagonPrefab;
                    }

                    SetHexagonOffsetCoords(hexagonPrefab, x: i, y: j);
                }
            }
        }

        private void SetHexagonOffsetCoords(GameObject hexagonPrefab, int x, int y) =>
            hexagonPrefab.GetComponent<Hexagon>().OffsetCoordinates = new Vector2Int(x, y);
    }
#endif
    public enum HexGridType
    {
        RECTANGLE,
        HEX
    }
}*/