using System;
using UnityEngine;

namespace Core.Logic
{
    public class Hexagon : Cell
    {
        [HideInInspector] [SerializeField] private Vector2Int _offsetCoordinates;
        [HideInInspector] [SerializeField] private Vector3Int _cubeCoordinates;

        private CapsuleCollider _collider;

        public override bool IsTaken { get; set; }

        public override Vector2Int OffsetCoordinates
        {
            get => _offsetCoordinates;
            set
            {
                _cubeCoordinates = OffsetToCube(value);
                _offsetCoordinates = value;
            }
        }

        public Vector3Int CubeCoordinates
        {
            get => _cubeCoordinates;
            set
            {
                _offsetCoordinates = CubeToOffset(value);
                _cubeCoordinates = value;
            }
        }

        private Vector2Int CubeToOffset(Vector3Int cubeCoordinates)
        {
            var x = cubeCoordinates.x + (cubeCoordinates.y - (cubeCoordinates.y & 1)) / 2;
            var y = cubeCoordinates.y;

            return new Vector2Int(x, y);
        }

        public int GetDistance(Hexagon to)
        {
            return (Math.Abs(CubeCoordinates.x - to.CubeCoordinates.x) +
                    Math.Abs(CubeCoordinates.y - to.CubeCoordinates.y) +
                    Math.Abs(CubeCoordinates.z - to.CubeCoordinates.z)) / 2;
        }

        private Vector3Int OffsetToCube(Vector2Int offsetCoordinates)
        {
            var x = offsetCoordinates.x - (offsetCoordinates.y - (offsetCoordinates.y & 1)) / 2;
            var y = offsetCoordinates.y;

            return new Vector3Int(x, y, -x - y);
        }
    }
}