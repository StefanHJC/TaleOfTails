using System;

namespace Core.Unit
{
    public interface IHealthComponent
    {
        public float Health { get; }

        public event Action HealthChanged;
    }
}