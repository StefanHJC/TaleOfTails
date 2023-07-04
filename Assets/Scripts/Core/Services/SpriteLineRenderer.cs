using System.Collections.Generic;
using Core.Services.AssetManagement;
using Core.Services.Pathfinder;
using ModestTree.Util;
using UnityEngine;
using Utils.Comparers;
using Zenject;
using Object = UnityEngine.Object;

namespace Core.Services
{
    public class SpriteLineRenderer : IPathRenderer
    {
        private readonly GameObject _tile;

        private readonly List<GameObject> _tiles = new();

        //private int _onTiles;
        private readonly float _tileSize = 0.45f;
        private readonly float _space = 0.45f;

        [Inject]
        public SpriteLineRenderer(IAssets assets)
        {
            _tile = assets.Load("Prefabs/LineStap");
        }

        public void RenderPath(Path path)
        {
            List<ValuePair<Vector3, Quaternion>> points = BrakeLinesToTransform(BrakeToLines(path.List));
            for (int i = 0; i < points.Count; i++)
            {
                PlaceTile(points[i].First, points[i].Second, i);
            }

            OffTiles(points.Count, _tiles.Count);
        }

        public void RenderOff()
        {
            OffTiles(0, _tiles.Count);
        }

        private List<Vector3> BrakeToLines(IReadOnlyList<MonoBehaviour> path)
        {
            if (path.Count < 2)
            {
                return new();
            }

            List<Vector3> points = new() { path[0].transform.position, path[1].transform.position };
            Vector3 line1 = points[1] - points[0];
            for (int i = 2; i < path.Count; i++)
            {
                Vector3 line2 = path[i].transform.position - points[^1];
                Quaternion rot = Quaternion.LookRotation(line2 - line1);
                if (QuaternionComparer.Compare(rot, Quaternion.identity) != 0)
                {
                    points.Add(path[i].transform.position);
                    line1 = line2;
                }
                else
                {
                    points[^1] = path[i].transform.position;
                }
            }

            return points;
        }

        private List<ValuePair<Vector3, Quaternion>> BrakeLinesToTransform(List<Vector3> lines)
        {
            List<ValuePair<Vector3, Quaternion>> points = new();
            for (int i = 0; i < lines.Count - 1; i++)
            {
                Vector3 line = lines[i + 1] - lines[i];
                Vector3 lineNormalized = line.normalized;
                int steps = (int)(line.magnitude / (_tileSize + _space));
                for (int j = 0; j < steps; j++)
                {
                    Vector3 point = lines[i] + lineNormalized * ((_tileSize + _space) * j);
                    points.Add(new(
                        point,
                        Quaternion.LookRotation(lineNormalized)));
                }
            }

            if (points.Count > 0)
            {
                points.Add(new ValuePair<Vector3, Quaternion>(lines[^1], points[^1].Second));
            }

            return points;
        }

        void PlaceTile(Vector3 point, Quaternion rotation, int index)
        {
            for (int i = _tiles.Count; i <= index; i++)
            {
                _tiles.Add(Object.Instantiate(_tile));
                _tiles[i].gameObject.SetActive(false);
            }

            _tiles[index].transform.SetPositionAndRotation(point, rotation);
            _tiles[index].gameObject.SetActive(true);
            //_onTiles = Mathf.Max(_onTiles, index);
        }

        void OffTiles(int startIndex, int end)
        {
            startIndex = Mathf.Max(startIndex, 0);
            end = Mathf.Min(end, _tiles.Count);

            for (int i = startIndex; i < end; i++)
            {
                if (_tiles[i].gameObject != null)
                    _tiles[i].gameObject.SetActive(false);
            }

            // if (end > _onTiles)
            // {
            //     _onTiles = startIndex - 1;
            // }
        }
    }
}