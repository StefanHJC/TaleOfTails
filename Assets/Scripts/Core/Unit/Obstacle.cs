using EPOOutline;
using UnityEngine;

namespace Core.Unit
{
    public class Obstacle : GridEntity
    {
        //public override IHealthComponent Health { get; }
        public override Outlinable Outline { get; }
        public override GameObject Mesh { get; }
        public IDeathComponent<Obstacle> Death { get; }
    }
}