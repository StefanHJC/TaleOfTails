using Core.Services.Pathfinder;

namespace Core.Services
{
    public interface IPathRenderer
    {
        public void RenderPath(Path path);

        public void RenderOff();
    }
}