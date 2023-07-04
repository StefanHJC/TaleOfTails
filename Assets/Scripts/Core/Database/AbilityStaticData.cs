using Core.Unit.Ability;
using UnityEngine;

namespace Core.Database
{
    [CreateAssetMenu(fileName = "BaseAbility data", menuName = "Static data/BaseAbility")]
    public class AbilityStaticData : ScriptableObject
    {
        public AbilityId AbilityId;
        public BaseAbility Prefab;
        public int Value;
        public int Cooldown;

        public Sprite UISprite;
    }
}