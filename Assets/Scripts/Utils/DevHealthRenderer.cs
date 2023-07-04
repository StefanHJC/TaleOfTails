using Core.Unit.Component;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class DevHealthRenderer : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private UnitHealth _health;

        private float _maxHealth;

        private void Start()
        {
            _health.HealthChanged += OnHealthChanged;
            _maxHealth = (float)_health.Health;
        }

        private void Update()
        {
            transform.LookAt(Camera.main.transform);
        }

        private void OnHealthChanged()
        {
            _fillImage.fillAmount = (float)_health.Health / _maxHealth;
        }
    }
}