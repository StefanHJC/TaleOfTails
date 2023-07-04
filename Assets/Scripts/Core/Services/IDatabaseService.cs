using Core.Database;
using Core.UI;

namespace Core.Services
{
    public interface IDatabaseService
    {
        public string MainMenuScene { get; }

        public void LoadUnits();
        public void LoadGameSettings();

        public UnitStaticData TryGetUnitData(UnitTypeId typeId);
        public AbilityStaticData TryGetAbilityData(AbilityId id);
        public WindowData TryGetWindowData(WindowId id);
        public LevelData TryGetLevelData(LevelId id);
        public DialogueSettings TryGetDialogueSettings();
        public MusicSettings TryGetMusicData();
        public CursorSpriteSettings TryGetCursorData();
    }
}