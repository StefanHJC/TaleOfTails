using System;
using UnityEngine;

namespace Core.Logic.Projectile
{
    [RequireComponent(typeof(Rigidbody))]
    public class AoeProjectile : BaseProjectile
    {
        public event Action TargetReached;

        protected override void OnHit()
        {
            base.OnHit();
            TargetReached?.Invoke();
        }
    }
}