using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public enum WindowId
    {
        None,
        SystemMenu,
        LoseWindow,
        WinWindow,
        DialogueWindow,
        EducationWindow,
        StatsCard,
        Authors
    }

    public abstract class BaseWindow : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;

        public Action<GameLoopButtonAction, BaseWindow> ButtonClicked;
        public Action Closed;

        private void Awake() =>
            OnAwake();

        private void Start()
        {
            Init();
            SubscribeUpdates();
        }

        private void OnDestroy()
        {
            Closed?.Invoke();
            Cleanup();
        }

        protected virtual void OnAwake()
        {
            if (_closeButton != null)
                _closeButton.onClick.AddListener(() => Destroy(gameObject));
        }

        protected virtual void Init() { }
        protected virtual void SubscribeUpdates() { }
        protected virtual void Cleanup() { }
    }
}