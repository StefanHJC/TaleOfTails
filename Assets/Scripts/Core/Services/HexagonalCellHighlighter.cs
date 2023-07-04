using System;
using System.Collections.Generic;
using System.Linq;
using Core.Logic;
using Core.Services.AssetManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Core.Services
{
    public class HexagonalCellHighlighter : ICellHighlighter
    {
        private readonly IAssets _assets;

        private List<GameObject> _activeHighlightPrefabs = new List<GameObject>();
        private List<GameObject> _highlightPrefabsPool = new List<GameObject>();

        public IReadOnlyList<GameObject> HighlightedCells => _activeHighlightPrefabs;

        [Inject]
        public HexagonalCellHighlighter(IAssets assets)
        {
            _assets = assets;
        }

        public void Highlight(IEnumerable<Cell> cells)
        {
            foreach (Cell cell in cells) 
                Highlight(cell);
        }

        public void Highlight(Cell cell)
        {
            if(cell == null)
                return;

            if (_highlightPrefabsPool.Count <= 0)
            {
                GameObject highlightPrefab = InstantiateNewPrefab();
                Move(highlightPrefab, to: cell.transform);
            }
            else if (_highlightPrefabsPool.Count > 0)
            {
                GameObject highlightPrefab = PopFromPool();
                Move(highlightPrefab, to: cell.transform);
            }
        }

        public void Dehighlight(IEnumerable<Cell> cells)
        {
            foreach (Cell cell in cells)
                Dehighlight(cell);
        }

        public void Dehighlight(Cell cell)
        {
            GameObject highlightPrefab = _activeHighlightPrefabs.FirstOrDefault(prefab => prefab.transform.position == cell.transform.position);
            
            if (highlightPrefab != null)
                PushToPool(highlightPrefab);
        }

        public void Reset()
        {
            for (int i = 0; i < _activeHighlightPrefabs.Count; i++)
            {
                if (_activeHighlightPrefabs[i] == null)
                    _activeHighlightPrefabs.Remove(_activeHighlightPrefabs[i]);
            }

            if(_activeHighlightPrefabs.Count <= 0)
                return;
            List<GameObject> bufList = new List<GameObject>();

            foreach (GameObject activeHighlightPrefab in _activeHighlightPrefabs)
            {
                bufList.Add(activeHighlightPrefab);
            }
            PushToPool(bufList);
            
        }

        public void ClearAll()
        {
            _activeHighlightPrefabs = new List<GameObject>();
            _highlightPrefabsPool = new List<GameObject>();
        }

        private void PushToPool(GameObject highlightPrefab)
        {
            highlightPrefab.gameObject.SetActive(false);
            _activeHighlightPrefabs.Remove(highlightPrefab);
            _highlightPrefabsPool.Add(highlightPrefab);
        }
        
        private void PushToPool(List<GameObject> prefabList)
        {

            foreach (GameObject prefab in prefabList)
            {
                if (prefab == null)
                    continue;

                prefab.gameObject.SetActive(false);
                _activeHighlightPrefabs.Remove(prefab);
                _highlightPrefabsPool.Add(prefab);
            }
        }

        private GameObject PopFromPool()
        {
            GameObject highlighPrefab = _highlightPrefabsPool.FirstOrDefault();

            _highlightPrefabsPool.Remove(highlighPrefab);
            highlighPrefab.gameObject.SetActive(true);
            _activeHighlightPrefabs.Add(highlighPrefab);

            return highlighPrefab;
        }

        private void Move(GameObject highlightPrefab, Transform to)
        {
            highlightPrefab.transform.position = new Vector3(to.transform.position.x, to.transform.position.y + .001f, to.transform.position.z);
            highlightPrefab.transform.rotation = to.transform.rotation;
            highlightPrefab.transform.parent = to.transform;

        }

        private GameObject InstantiateNewPrefab()
        {
            GameObject highlighPrefab = _assets.Instantiate(AssetPath.HighlightedHexagon);
            _activeHighlightPrefabs.Add(highlighPrefab);

            return highlighPrefab;
        }
    }
}