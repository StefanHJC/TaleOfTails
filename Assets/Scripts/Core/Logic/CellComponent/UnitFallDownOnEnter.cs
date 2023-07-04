using Core.Unit;
using UnityEngine;

namespace Core.Logic.Cells
{
    public class UnitFallDownOnEnter : CellBehaviour
    {
        public override string EditorLabel => $"Unit down on enter";
        
        protected override void OnUnitEnter(BaseUnit unit, Cell cell)
        {
            unit.GetComponent<Rigidbody>().isKinematic=false;
        }
    }
}