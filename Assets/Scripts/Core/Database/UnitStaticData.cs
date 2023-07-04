using System.Collections.Generic;
using Core.Logic.Projectile;
using Core.Unit.Component;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Database
{
    [CreateAssetMenu(fileName = "Unit data", menuName = "Static data/Unit")]
    public class UnitStaticData : ScriptableObject
    {
        public UnitTypeId TypeId;

        [AssetsOnly]
        public GameObject Prefab;

        [MinValue(1)]
        public int MaxHealth = 1;

        [MinValue(1)]
        public int Damage = 1;
        
        [MinValue(1)]
        public int Speed = 1;
        
        [MinValue(0)]
        public int Initiative = 1;

        [ShowIf("@HasRangeAttack")] 
        public TargetProjectile Projectile;

        [MinValue(1), ShowIf("@HasRangeAttack")]
        public int RangeDamage;

        public Sprite UiImage;
        public string DialogueLabel;

        public List<AbilityStaticData> AvailableAbilities;


        [FoldoutGroup("Sound settings")]
        public EventReference FootstepSFX;

        [FoldoutGroup("Sound settings")]
        public EventReference MeleeAttackStartSFX;

        [FoldoutGroup("Sound settings")]
        public EventReference MeleeAttackHitSFX;

        [FoldoutGroup("Sound settings")]
        public EventReference HurtSFX;

        [FoldoutGroup("Sound settings"), ShowIf("@HasRangeAttack")]
        public EventReference RangeAttackStartSFX;

        [FoldoutGroup("Sound settings"), ShowIf("@HasRangeAttack")]
        public EventReference ProjectileFlightSFX;

        [FoldoutGroup("Sound settings"), ShowIf("@HasRangeAttack")]
        public EventReference ProjectileHitSFX;

#if UNITY_EDITOR

        [ReadOnly, PreviewField(300)]
        public Object Preview;

        private bool HasRangeAttack => Prefab.TryGetComponent<UnitRangeAttack>(out var _);

        [OnInspectorInit]
        private void GetPreviewMesh()
        {
            if (Prefab == null)
                return;
            Preview = Prefab;
        }

#endif
    }
}