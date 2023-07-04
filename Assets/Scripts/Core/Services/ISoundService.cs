using System.Collections.Generic;
using Core.Database;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Zenject;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Core.Services
{
    public interface ISoundService
    {
        public float ThemeState { get; set; }
        public MusicThemeTypeId StateTypeId { get; }

        public void Init();
        public void InitEmitter(EventReference eventReference, GameObject emitterGameObject); // temp
        public EventInstance InstantiateEvent(EventReference eventReference);
        public void PlayMusic();
        public void StopMusic();
        public void PlayOnce(EventReference sfx, Vector3 at);
        public void PlayOnce(EventReference sfx);
    }

    public class FMODSoundService : ISoundService
    {
        private readonly Bus _master;
        private readonly Bus _music;
        private readonly Bus _sfx;
        private readonly IDatabaseService _data;

        private EventInstance _musicEvent;
        private List<EventInstance> _events;
        private List<StudioEventEmitter> _emitters;
        private MusicSettings _settings;
        private float _themeState;

        public float MasterVolume { get; set; }
        public float MusicVolume { get; set; }
        public float SFXVolume { get; set; }

        public float ThemeState
        {
            get => _themeState;
            set
            {
                _themeState = value;
                _musicEvent.setParameterByName("State", value);
            }
        }

        public MusicThemeTypeId StateTypeId { get; }

        [Inject]
        public FMODSoundService(IDatabaseService data)
        {
            // _master = GetBus("bus:/");
            // _music = GetBus("bus:/Music");
            // _sfx = GetBus("bus:/SFX");
            //TODO 

            _data = data;
        }

        public void Init()
        {
            _settings = _data.TryGetMusicData();
            _musicEvent = InstantiateEvent(_settings.MusicEvent);
            _musicEvent.setVolume(_settings.MusicVolume);
            _musicEvent.start();
        }

        public void InitEmitter(EventReference eventReference, GameObject emitterGameObject)
        {
        }

        public EventInstance InstantiateEvent(EventReference eventReference) => 
            RuntimeManager.CreateInstance(eventReference);

        public void PlayMusic()
        {
        }

        public void StopMusic()
        {
            _musicEvent.stop(STOP_MODE.IMMEDIATE);
            _musicEvent.release();
        }

        public void PlayOnce(EventReference sfx, Vector3 at) => RuntimeManager.PlayOneShot(sfx, at);
        public void PlayOnce(EventReference sfx) => RuntimeManager.PlayOneShot(sfx);

        private Bus GetBus(string path) => RuntimeManager.GetBus(path);
    }
}


public enum MusicThemeTypeId
{
    Battle = 1,
    Menu = 2 ,
    Dialogue = 3,
    Loading = 4,
    Win = 5,
    Lose = 6,
}