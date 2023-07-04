using System;
using Core.Logic;

namespace Core.Database
{
    [Serializable]
    public class LevelData
    {
        public LevelId LevelId;
        public string SceneName;
        public BaseScenario ScenarioScript;
    }
}