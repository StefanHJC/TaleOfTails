using System;
using System.Collections.Generic;
using Core.Logic;
using UnityEngine;

namespace Core.Services.Pathfinder
{
    public class Tile : IEquatable<Tile>
    {
        private readonly Cell _cell;

        public int G { get; set; }
        public int H { get; set; }
        public int F => G + H;
        public bool IsObstacle => _cell.IsTaken;
        public Cell Cell => _cell;
        public List<Tile> AdjacentTiles { get; set; }
        public Vector3Int Position { get; set; }

        public Tile(Cell cell)
        {
            _cell = cell;
            Position = GetTilePosition(_cell);
            AdjacentTiles = new List<Tile>();
        }

        private Vector3Int GetTilePosition(Cell cell)
        {
            if (cell is Hexagon hex)
                return hex.CubeCoordinates;

            return new Vector3Int(cell.OffsetCoordinates.x, cell.OffsetCoordinates.y, 0);
        }
        
        public bool Equals(Tile other) => Cell == other.Cell;
    }
}
