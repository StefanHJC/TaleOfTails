using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.UI
{
    public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField] private Image _image;
        [SerializeField] private MainMenuActionId _action;

        public event Action<MainMenuActionId> Clicked;
        public event Action Hovered;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Hovered?.Invoke();
            _image.enabled = true;
        }

        public void OnPointerClick(PointerEventData eventData) => Clicked?.Invoke(_action);

        public void OnPointerExit(PointerEventData eventData) => _image.enabled = false;
    }

    public enum MainMenuActionId
    {
        None,
        NewGame,
        ResumeGame,
        ShowSettings,
        ShowAuthors,
        Quit,
    }
}