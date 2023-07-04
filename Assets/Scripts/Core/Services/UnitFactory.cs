using System;
using System.Collections.Generic;
using Core.Database;
using Core.Logic;
using Core.Services.AssetManagement;
using Core.Unit;
using Core.Unit.Ability;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Core.Services
{
    public class UnitFactory : IUnitFactory
    {
        private readonly IAssets _assets;
        private readonly IDatabaseService _data;
        private DiContainer _diContainer;

        [Inject]
        public UnitFactory(IAssets assets, DiContainer diContainer, IDatabaseService data)
        {
            _assets = assets;
            _diContainer = diContainer;
            _data = data;
        }

        public void Init(SceneContext sceneContext) => _diContainer = sceneContext.Container;

        public BaseUnit CreateUnit(Cell at, UnitTypeId unitType, int teamId, Quaternion rotation = new())
        {
            UnitStaticData data = _data.TryGetUnitData(unitType);

            if (data == null)
                throw new ApplicationException($"Cannot load unit data for unit type {unitType}");

            BaseUnit unit = _assets.Instantiate(data.Prefab, at.transform.position, rotation).GetComponent<BaseUnit>();
            _diContainer.Inject(unit, new object[]{data, InjectAbilities(unit, data)});
            unit.TeamId = teamId;

            return unit;
        }

        private IEnumerable<BaseAbility> InjectAbilities(BaseUnit unit, UnitStaticData data)
        {
            List<BaseAbility> unitAbilities = new List<BaseAbility>();

            foreach (AbilityStaticData ability in data.AvailableAbilities)
            {
                BaseAbility abilityPrefab = Object.Instantiate(ability.Prefab, unit.transform);
                unitAbilities.Add(abilityPrefab);
                _diContainer.Inject(abilityPrefab, new object[]{unit, ability});
            }

            return unitAbilities;
        }
    }
}