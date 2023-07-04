using System.Collections.Generic;
using Core.Unit;

namespace Core.Services
{
    public interface IUnitSelector
    {
        public void Select(BaseUnit unit);
        public void Deselect();
        public void RenderOutline(BaseUnit unit, OutlinePreset mode);
        public void RenderOutline(IEnumerable<BaseUnit> units, OutlinePreset mode);
        public void RenderOffOutline(BaseUnit unit);
        public void RenderOffAllOutlines();
        public void ResetAll();
    }
}