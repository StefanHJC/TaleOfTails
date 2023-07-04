using System.Collections.Generic;
using UnityEngine;

namespace Core.Database
{
    public enum LevelId
    {
        None,
        First,
        Second,
        Third,
    }

    [CreateAssetMenu(fileName = "Level data", menuName = "Static data/Level")]
    public class LevelSettings : ScriptableObject
    {
        public string MainMenuSceneName;

        public List<LevelData> Data;
    }
}