using System.Collections.Generic;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Database
{
    [CreateAssetMenu(fileName = "Music theme settings", menuName = "Game settings/Music themes")]
    public class MusicSettings : SerializedScriptableObject
    {
        public EventReference MusicEvent;

        [Range(0f, 1f)]
        public float MusicVolume = 1;

        [Range(0f, 1f)]
        public float SFXVolume = 1;

        public EventReference AbilityButtonHovered;
        public EventReference AbilityButtonSelected;
        public EventReference MainMenuButtonHovered;
        public EventReference MainMenuButtonClicked;
        public EventReference MenuWindowOpened;
        public EventReference MenuWindowClosed;
        public EventReference NewGameStarted;
    }
}