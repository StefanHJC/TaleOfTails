using System.Collections.Generic;
using Core.Unit;
using Core.Unit.Component;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class UnitStatsCard : BaseWindow
    {
        [SerializeField] private List<Image> _skills;
        [SerializeField] private TMP_Text _values;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _label;
        
        private int _rangeAttackValue;
        private UnitHealth _health;
        private BaseUnit _unit;

        public void Construct(BaseUnit unit)
        {
            _health = unit.GetComponent<UnitHealth>();
            _health.HealthChanged += OnHealthChanged;
            _unit = unit;
            _rangeAttackValue = unit.IsRanged ? unit.Config.RangeDamage : 0;

            SetIcon();
        }

        private void SetIcon()
        {
            _values.text = $"{_unit.Config.Damage}\n" +
                           $"{_rangeAttackValue}\n" +
                           $"{_unit.Config.Initiative}\n" +
                           $"{_unit.Config.Speed}\n" +
                           $"{_unit.Config.MaxHealth}\n" +
                           $"{_health.Health}";
            
            _label.text = _unit.Config.DialogueLabel;

            for (int i = 0; i < _unit.Config.AvailableAbilities.Count; i++)
                _skills[i].sprite = _unit.Config.AvailableAbilities[i].UISprite;

            _icon.sprite = _unit.Config.UiImage;
        }

        private void OnHealthChanged()
        {

            _values.text = $"{_rangeAttackValue}\n" +
                           $"{_unit.Config.Damage}\n" +
                           $"{_unit.Config.Initiative}\n" +
                           $"{_unit.Config.Speed}\n" +
                           $"{_unit.Config.MaxHealth}\n" +
                           $"{_health.Health}";
        }

        private void OnDestroy() => _health.HealthChanged -= OnHealthChanged;
    }
}