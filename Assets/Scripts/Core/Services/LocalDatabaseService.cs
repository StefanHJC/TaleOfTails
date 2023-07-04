using System;
using System.Collections.Generic;
using System.Linq;
using Core.Database;
using Core.Logic;
using Core.Services.AssetManagement;
using Core.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Services
{
    public class LocalDatabaseService : IDatabaseService
    {
        private readonly string _unitDataPath = AssetPath.UnitStaticData;
        private readonly string _gameDataPath = AssetPath.GameStaticData;
        private readonly string _abilityDataPath;

        private Dictionary<UnitTypeId, UnitStaticData> _units;
        private Dictionary<Type, SerializedScriptableObject> _gameData;
        private Dictionary<AbilityId, AbilityStaticData> _abilities;
        private Dictionary<WindowId, WindowData> _windows;
        private Dictionary<LevelId, LevelData> _levels;
        private MusicSettings _musicData;
        private DialogueSettings _dialogueData;
        private CursorSpriteSettings _cursors;
        private LevelSettings _levelSettings;

        public string MainMenuScene { get; private set; }

        public LocalDatabaseService()
        {
            LoadGameSettings();
        }

        public void LoadUnits()
        {
            _units = Resources
                .LoadAll<UnitStaticData>(_unitDataPath)
                .ToDictionary(data => data.TypeId, data => data);
        }

        private void LoadAbilities()
        {
            _abilities = Resources
                .LoadAll<AbilityStaticData>(_abilityDataPath)
                .ToDictionary(data => data.AbilityId, data => data);
        }

        public void LoadGameSettings()
        {
            _dialogueData = Resources.Load<DialogueSettings>(AssetPath.DialogueDataPath);

            _musicData = Resources
                .Load<MusicSettings>(AssetPath.MusicDataPath);

            _windows = Resources
                .Load<WindowStaticData>(AssetPath.WindowsStaticData)
                .Data
                .ToDictionary(x => x.WindowId, x => x);

            _levelSettings = Resources
                .Load<LevelSettings>(AssetPath.LevelStaticData);

            MainMenuScene = _levelSettings.MainMenuSceneName;

            _levels = _levelSettings
                .Data
                .ToDictionary(x => x.LevelId, x => x);

            _cursors = Resources
                .Load<CursorSpriteSettings>(AssetPath.CursorStaticData);
        }

        public WindowData TryGetWindowData(WindowId id) =>
        _windows.TryGetValue(id, out WindowData windowData)
        ? windowData
        : null;
        
        public UnitStaticData TryGetUnitData(UnitTypeId typeId) => 
            _units.TryGetValue(typeId, out UnitStaticData data) ? data : null;

        public AbilityStaticData TryGetAbilityData(AbilityId id) =>
            _abilities.TryGetValue(id, out AbilityStaticData data) ? data : null;

        public LevelData TryGetLevelData(LevelId id) =>
            _levels.TryGetValue(id, out LevelData data) ? data : null;

        public DialogueSettings TryGetDialogueSettings() => _dialogueData;
        public MusicSettings TryGetMusicData() => _musicData;
        public CursorSpriteSettings TryGetCursorData() => _cursors;
    }
}