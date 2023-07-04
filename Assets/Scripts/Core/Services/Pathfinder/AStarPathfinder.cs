using System.Collections.Generic;
using System.Linq;
using Core.Logic;
using Core.Logic.Grid;
using UnityEngine;
using Zenject;

namespace Core.Services.Pathfinder
{
    public class AStarPathfinder : IPathfinderService
    {
        private AbstractGrid _grid;

        private Path _cachedPath;

        public Path CachedPath => _cachedPath;

        [Inject]
        public AStarPathfinder(AbstractGrid grid)
        {
            _grid = grid;
        }

        public Path GetPath(Cell from, Cell to)
        {
            List<Tile> openPathTiles = new List<Tile>();
            List<Tile> closedPathTiles = new List<Tile>();

            Tile startPoint = new Tile(from);
            Tile endPoint = new Tile(to);
            Tile currentTile = startPoint;

            currentTile.AdjacentTiles = GetAdjacentTiles(currentTile);

            currentTile.G = 0;
            currentTile.H = GetEstimatedPathCost(startPoint.Position, endPoint.Position);

            openPathTiles.Add(currentTile);

            while (openPathTiles.Count != 0)
            {
                openPathTiles = openPathTiles.OrderBy(x => x.F).ThenByDescending(x => x.G).ToList();
                currentTile = openPathTiles[0];
                currentTile.AdjacentTiles = GetAdjacentTiles(currentTile);
                openPathTiles.Remove(currentTile);
                closedPathTiles.Add(currentTile);

                int g = currentTile.G + 1;

                if (closedPathTiles.Contains(endPoint))
                {
                    var endPath = closedPathTiles.Find(tile => tile.Cell == endPoint.Cell);
                    endPoint = endPath;
                    break;
                }

                foreach (Tile adjacentTile in currentTile.AdjacentTiles)
                {
                    if (adjacentTile.IsObstacle)
                    {
                        continue;
                    }

                    if (closedPathTiles.Contains(adjacentTile))
                    {
                        continue;
                    }

                    if (!(openPathTiles.Contains(adjacentTile)))
                    {
                        adjacentTile.G = g;
                        adjacentTile.H = GetEstimatedPathCost(adjacentTile.Position, endPoint.Position);
                        openPathTiles.Add(adjacentTile);
                    }
                    else if (adjacentTile.F > g + adjacentTile.H)
                    {
                        adjacentTile.G = g;
                    }
                }
            }

            List<Tile> finalPathTiles = new List<Tile>();

            if (closedPathTiles.Contains(endPoint))
            {
                currentTile = endPoint;
                finalPathTiles.Add(currentTile);

                for (int i = endPoint.G - 1; i >= 0; i--)
                {
                    currentTile = closedPathTiles.Find(x => x.G == i && currentTile.AdjacentTiles.Contains(x));
                    finalPathTiles.Add(currentTile);
                }

                finalPathTiles.Reverse();
            }

            List<Cell> cells = new List<Cell>();

            foreach (Tile tile in finalPathTiles)
                cells.Add(tile.Cell);

            Path path = new Path(cells);
            _cachedPath = path;

            return path;
        }

        public int GetDistance(Cell from, Cell to)
        {
            List<Tile> openPathTiles = new List<Tile>();
            List<Tile> closedPathTiles = new List<Tile>();

            Tile startPoint = new Tile(from);
            Tile endPoint = new Tile(to);
            Tile currentTile = startPoint;

            currentTile.AdjacentTiles = GetAdjacentTiles(currentTile);

            currentTile.G = 0;
            currentTile.H = GetEstimatedPathCost(startPoint.Position, endPoint.Position);

            openPathTiles.Add(currentTile);

            while (openPathTiles.Count != 0)
            {
                openPathTiles = openPathTiles.OrderBy(x => x.F).ThenByDescending(x => x.G).ToList();
                currentTile = openPathTiles[0];
                currentTile.AdjacentTiles = GetAdjacentTiles(currentTile);
                openPathTiles.Remove(currentTile);
                closedPathTiles.Add(currentTile);

                int g = currentTile.G + 1;

                if (closedPathTiles.Contains(endPoint))
                {
                    var endPath = closedPathTiles.Find(tile => tile.Cell == endPoint.Cell);
                    endPoint = endPath;
                    break;
                }

                foreach (Tile adjacentTile in currentTile.AdjacentTiles)
                {
                    if (adjacentTile.IsObstacle)
                    {
                        continue;
                    }

                    if (closedPathTiles.Contains(adjacentTile))
                    {
                        continue;
                    }

                    if (!(openPathTiles.Contains(adjacentTile)))
                    {
                        adjacentTile.G = g;
                        adjacentTile.H = GetEstimatedPathCost(adjacentTile.Position, endPoint.Position);
                        openPathTiles.Add(adjacentTile);
                    }
                    else if (adjacentTile.F > g + adjacentTile.H)
                    {
                        adjacentTile.G = g;
                    }
                }
            }

            List<Tile> finalPathTiles = new List<Tile>();

            if (closedPathTiles.Contains(endPoint))
            {
                currentTile = endPoint;
                finalPathTiles.Add(currentTile);

                for (int i = endPoint.G - 1; i >= 0; i--)
                {
                    currentTile = closedPathTiles.Find(x => x.G == i && currentTile.AdjacentTiles.Contains(x));
                    finalPathTiles.Add(currentTile);
                }
            }

            return finalPathTiles.Count;
        }

        private List<Tile> GetAdjacentTiles(Tile currentTile)
        {
            List<Tile> neughbours = new List<Tile>();

            foreach (Cell cellNeighbour in _grid.GetCellNeighbours(currentTile.Cell))
                neughbours.Add(new Tile(cellNeighbour));

            return neughbours;
        }

        private static int GetEstimatedPathCost(Vector3Int startPosition, Vector3Int targetPosition)
        {
            return Mathf.Max(Mathf.Abs(startPosition.z - targetPosition.z),
                Mathf.Max(Mathf.Abs(startPosition.x - targetPosition.x),
                    Mathf.Abs(startPosition.y - targetPosition.y)));
        }
    }
}