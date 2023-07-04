using Core.Unit;
using Core.Unit.Component;
using UnityEngine;

namespace Core.Logic.Cells
{
    public class UnitTakeDamageOnEnter : CellBehaviour
    {
        [SerializeField] private int _damage;
        [SerializeField] private GameObject _damageFx;

        public override string EditorLabel => $"Unit take {_damage} damage on enter";

        protected override void OnUnitEnter(BaseUnit unit, Cell cell)
        {
            var unitHealth = unit.GetComponent<UnitHealth>();
            //unitHealth.TakeDamage(_damage);
        }
    }
}