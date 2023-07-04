using System.Collections;
using Core.Unit;
using Core.Unit.Component;
using DamageNumbersPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Comparers;

namespace Core.UI.HUD
{
    public static class HealthRendererSettings
    {
        public static DamageMessageStyle Heal = new DamageMessageStyle { TextColor = Color.green };
        public static DamageMessageStyle Damage = new DamageMessageStyle { TextColor = Color.red };
        public static DamageMessageStyle Effect = new DamageMessageStyle { TextColor = Color.yellow };
        public static DamageMessageStyle SkipTurn = new DamageMessageStyle { TextColor = Color.blue };
    }

    public struct DamageMessageStyle
    {
        public Color TextColor;
    }

    public class HealthRenderer : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private DamageNumber _damageMessagePrefab;

        private DamageNumber _numberInstance;
        private UnitHealth _health;
        private float _maxHealth;
        private BaseUnit _unit;
        private Camera _camera;
        private float _lastValue;

        public void Construct(BaseUnit unit)
        {
            _health = unit.GetComponent<UnitHealth>();
            _unit = unit;

            _health.HealthChanged += OnHealthChanged;
            _maxHealth = _unit.Config.MaxHealth;
            _lastValue = _unit.Config.MaxHealth;
            _camera =  Camera.main;
            _text.text = $"{_maxHealth}/{_maxHealth}";
        }

        public void RenderStatusMessage(string message, DamageMessageStyle style)
        {
            DamageNumber instance = _damageMessagePrefab.Spawn(transform.position, message);
            instance.transform.parent = transform;
            TMP_Text textMesh = instance.GetTextMesh();
            textMesh.color = style.TextColor;
        }

        private void Update()
        {
            transform.position = _camera.WorldToScreenPoint(_unit.transform.position.WithY(3f).WithX(-.5f));
        }

        private void OnHealthChanged()
        {
            if (_lastValue > _health.Health)
            {
                StartCoroutine(ReduceHealthRoutine(_health.Health));
                SpawnDamageMessage((_health.Health - _lastValue));

                if (_health.Health <= 0)
                {
                    if (_numberInstance != null)
                        _numberInstance.transform.parent = transform.parent.transform.parent;
                }
            }
            else
            {
                StartCoroutine(AddHealthRoutine(_health.Health));
                SpawnDamageMessage((_health.Health - _lastValue));
            }
        }

        private void SpawnDamageMessage(float value)
        {
            if (value == 0)
                return;

            _numberInstance = _damageMessagePrefab.Spawn(transform.position, value);
            _numberInstance.transform.parent = transform;
            _numberInstance.numberSettings.alpha = 1;

            if (value > 0)
            {
                TMP_Text textMesh = _numberInstance.GetTextMesh();
                textMesh.color = HealthRendererSettings.Heal.TextColor;
                _numberInstance.leftText = "+";
            }
            else
            {
                TMP_Text textMesh = _numberInstance.GetTextMesh();
                textMesh.color = HealthRendererSettings.Damage.TextColor;
            }
        }

        private void SpawnDamageMessage(string message, DamageMessageStyle style)
        {
            _numberInstance = _damageMessagePrefab.Spawn(transform.position, message);
            _numberInstance.transform.parent = transform;
            TMP_Text textMesh = _numberInstance.GetTextMesh();
            textMesh.color = style.TextColor;
        }

        private IEnumerator ReduceHealthRoutine(float targetValue)
        {
            float currentValue = _lastValue;

            while (_fillImage.fillAmount + Constants.Epsilon >= targetValue / _maxHealth && currentValue > targetValue)
            {
                currentValue -= 1 * (1 + Time.deltaTime);

                _fillImage.fillAmount = Mathf.MoveTowards(_fillImage.fillAmount, targetValue / _maxHealth, .1f);
                _text.text = $"{(int)currentValue}/{_maxHealth}";

                yield return null;
            }
            _fillImage.fillAmount = (float)_health.Health / _maxHealth;
            _text.text = $"{(int)_health.Health}/{_maxHealth}";
            _lastValue = _health.Health;
        }

        private IEnumerator AddHealthRoutine(float targetValue)
        {
            float currentValue = _lastValue;

            while (_fillImage.fillAmount + Constants.Epsilon <= targetValue / _maxHealth && currentValue < targetValue)
            {
                currentValue += 1 * (1 + Time.deltaTime);

                _fillImage.fillAmount = Mathf.MoveTowards(_fillImage.fillAmount, targetValue / _maxHealth, .1f);
                _text.text = $"{(int)currentValue}/{_maxHealth}";

                yield return null;
            }
            _fillImage.fillAmount = (float)_health.Health / _maxHealth;
            _text.text = $"{(int)_health.Health}/{_maxHealth}";
            _lastValue = _health.Health;
        }
    }
}