using System;
using Core.Database;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.UI.HUD
{
    public enum AbilityButtonStateId
    {
        Active,
        Empty,
        Selected,
        Cooldown
    }

    public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button _button;
        [SerializeField] private Sprite _default;
        [SerializeField] private GameObject _background;
        [SerializeField] private Animation _backgroundAnimation;
        [SerializeField] private TMP_Text _textField;
        [SerializeField] private ButtonBehaviour _behaviour;

        private AbilityStaticData _abilityData;
        private AbilityButtonStateId _stateId;
        private int _roundToUnlock;

        public AbilityStaticData AbilityData => _abilityData;
        public AbilityButtonStateId State
        {
            get => _stateId;
            set
            {
                _stateId = value;
                _textField.text = string.Empty;

                switch (value)
                {
                    case AbilityButtonStateId.Active:
                    {
                        _background.SetActive(false);
                        //_behaviour.enabled = true;
                        _button.image.color = _button.colors.pressedColor;
                        break;
                    }
                    case AbilityButtonStateId.Empty:
                    {
                        _background.SetActive(false);
                        //_behaviour.enabled = false;
                        _button.image.color = _button.colors.normalColor;
                        break;
                    }
                    case AbilityButtonStateId.Selected:
                    {
                        _backgroundAnimation.Play();
                        //_behaviour.enabled = true;
                        _button.image.color = _button.colors.normalColor;
                        break;
                    }
                    case AbilityButtonStateId.Cooldown:
                    {
                        _background.SetActive(false);
                        //_behaviour.enabled = false;
                        _button.image.color = _button.colors.pressedColor;
                        _textField.text = _roundToUnlock.ToString();

                        break;
                    }
                }
            }
        }

        public event Action<AbilityButton> Clicked;

        public void SetAbility(AbilityStaticData abilityData, int roundsToUnlock)
        {
            _button.image.sprite = abilityData.UISprite;
            _abilityData = abilityData;
            _roundToUnlock = roundsToUnlock;
        }

        public void ResetAbility()
        {
            _button.image.sprite = _default;
            _abilityData = null;
            _roundToUnlock = 0;
            State = AbilityButtonStateId.Empty;
        }

        private void Start() => _button.onClick.AddListener(OnClick);

        private void OnDestroy() => _button.onClick.RemoveListener(OnClick);

        private void OnClick()
        {
            Clicked?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (State != AbilityButtonStateId.Empty && State != AbilityButtonStateId.Cooldown)
                _background.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (State != AbilityButtonStateId.Selected)
                _background.SetActive(false);
        }
    }
}