using EPOOutline;
using UnityEngine;

namespace Core.Unit
{
    public abstract class GridEntity : MonoBehaviour
    {
        //public abstract IHealthComponent Health { get; }
        public abstract Outlinable Outline { get; }
        public abstract GameObject Mesh { get; }
    }
}