using System;
using System.Linq;
using Core.Unit;
using UnityEngine;

namespace Core.Logic.Projectile
{
    [RequireComponent(typeof(Rigidbody))]
    public class TargetProjectile : BaseProjectile
    {
        public event Action<BaseUnit> TargetReached;

        protected override void OnHit()
        {
            base.OnHit();
            TargetReached?.Invoke(Collisions.First().GetComponentInParent<BaseUnit>());
        }
    }
}