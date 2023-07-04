using Core.Logic.Grid;
using Core.Services.Pathfinder;
using UnityEngine;
using Zenject;

namespace Core.Services
{
    public class PathRenderer : IPathRenderer
    {
        private readonly LineRenderer _lineRenderer;

        [Inject]
        public PathRenderer(AbstractGrid abstractGrid)
        {
            _lineRenderer = abstractGrid.GetComponent<LineRenderer>();
        }

        public void RenderPath(Path path)
        {
            _lineRenderer.positionCount = path.List.Count;

            for (var i = 0; i < path.List.Count; ++i)
            {
                _lineRenderer.SetPosition(i, path.List[i].transform.position);
            }
        }

        public void RenderOff()
        {
            _lineRenderer.positionCount = 0;
        }
    }
}