using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.UI
{
    public class ButtonBehaviour :  MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        public event Action Hovered;
        public event Action Clicked;

        public void OnPointerEnter(PointerEventData eventData) => Hovered?.Invoke();

        public void OnPointerClick(PointerEventData eventData) => Clicked?.Invoke();
    }
}