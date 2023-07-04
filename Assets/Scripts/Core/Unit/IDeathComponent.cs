using System;

namespace Core.Unit
{
    public interface IDeathComponent<T>
    {
        public event Action<T> Died;
    }
}