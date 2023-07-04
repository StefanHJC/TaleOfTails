using Core.Database;
using Core.Logic;
using Core.Unit;
using UnityEngine;
using Zenject;

namespace Core.Services
{
    public interface IUnitFactory
    {
        public void Init(SceneContext sceneContext);

        public BaseUnit CreateUnit(Cell at, UnitTypeId unitType, int teamId, Quaternion rotation = new());
    }
}