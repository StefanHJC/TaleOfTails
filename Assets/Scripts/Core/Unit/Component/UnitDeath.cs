using System;
using System.Collections;
using Core.UI.HUD;
using UnityEngine;

namespace Core.Unit.Component
{
    [RequireComponent(typeof(UnitHealth))]
    public class UnitDeath : MonoBehaviour, IDeathComponent<BaseUnit>
    {
        [SerializeField] private BaseUnit _unit;
        [SerializeField] private UnitHealth _unitHealth;
        [SerializeField] private UnitRagdoll _unitRagdoll;
        [SerializeField] private Collider _unitTrigger;
        [SerializeField] private HealthRenderer _healthRenderer;

        public event Action<BaseUnit> Died;

        private void Start()
        {
            _unitHealth.HealthChanged += Die;
            _healthRenderer = _unit.HealthRenderer;
        }

        private void Die()
        {
            if (_unitHealth.Health <= 0)
            {
                Died?.Invoke(GetComponentInParent<BaseUnit>());
                _unitRagdoll.EnableRagdoll(Vector3.up * 300);
                _unitTrigger.gameObject.SetActive(false);
                _healthRenderer.gameObject.SetActive(false);
                StartCoroutine(DisableRagdollRoutine()); // temp may be :)
            }
        }

        private IEnumerator DisableRagdollRoutine()
        {
            yield return new WaitForSeconds(5);
            _unitRagdoll.DisableRagdoll();
        }

        private IEnumerator DisableHealthRendererRoutine()
        {
            yield return new WaitForSeconds(3);
            _healthRenderer.gameObject.SetActive(false);
        }
    }
}