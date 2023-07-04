using System.Collections.Generic;
using System.Linq;
using Core.Database;
using Core.Services;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Core.UI
{
    public class EducationWindow : BaseWindow
    {
        private ISoundService _sound;

        [SerializeField] private List<ButtonBehaviour> _buttonBehaviours;
        private EventReference _clickSfx;

        [Inject]
        public void Construct(ISoundService sound, IDatabaseService data)
        {
            _sound = sound;
            MusicSettings musicSettings = data.TryGetMusicData();
            _clickSfx = musicSettings.MenuWindowClosed;
        }

        public void Start()
        {
            foreach (ButtonBehaviour behaviour in _buttonBehaviours) 
                behaviour.Clicked += OnClicked;
        }

        private void OnDestroy()
        {
            _buttonBehaviours.Last().gameObject.SetActive(false);
            Closed?.Invoke();

            foreach (ButtonBehaviour behaviour in _buttonBehaviours)
                behaviour.Clicked -= OnClicked;
        }

        private void OnClicked()
        {
            _sound.PlayOnce(_clickSfx);
        }
    }
}