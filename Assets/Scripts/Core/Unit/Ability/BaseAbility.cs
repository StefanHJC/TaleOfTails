using System;
using System.Collections.Generic;
using Core.Database;
using Core.Logic;
using UnityEngine;

namespace Core.Unit.Ability
{
    public abstract class BaseAbility : MonoBehaviour
    {
        public abstract AbilityId Id { get; }
        public abstract AbilityTypeId TypeId { get; }
        public abstract BaseUnit Caster { get; }
        public abstract int Cooldown { get; }
        public abstract void Perform(Cell target,  Action onExecuted);
        public abstract void OnAnimatorPerform();
    }

    public abstract class TargetAbility : BaseAbility
    {
        public override AbilityTypeId TypeId => AbilityTypeId.Target;
        public abstract IEnumerable<Cell> GetActionableTargets();
    }

    public abstract class AreaOfEffectAbility : BaseAbility
    {
        public override AbilityTypeId TypeId => AbilityTypeId.AreaOfEffect;
        public abstract IEnumerable<Cell> GetAffectedCells(Cell target);
    }
}