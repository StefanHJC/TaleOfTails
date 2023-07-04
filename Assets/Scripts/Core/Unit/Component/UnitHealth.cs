using System;
using System.Collections;
using UnityEngine;

namespace Core.Unit.Component
{
    public class UnitHealth : MonoBehaviour
    {
        [SerializeField] private BaseUnitAnimator _animator;
        [SerializeField] private BaseUnit _unit;

        public float Health { get; private set; }

        public event Action HealthChanged;

        public void TakeDamage(float damage, Action onHitAnimEnd=null)
        {
            Health -= damage;
            _animator.PlayHit();
            _unit.Sound.PlayOnce(_unit.Config.HurtSFX, transform.position);
            
            HealthChanged?.Invoke();
            StartCoroutine(WaitRoutine(onHitAnimEnd));
        }

        public void TakeHeal(float heal, Action onHitAnimEnd = null)
        {
            Health += Mathf.Clamp(heal, 0, _unit.Config.MaxHealth - Health);
            
            HealthChanged?.Invoke();
        }

        private void Start()
        {
            Health = _unit.Config.MaxHealth;
        }

        private IEnumerator WaitRoutine(Action onHitAnimEnd)
        {
            yield return new WaitForSeconds(1.5f);
            
            onHitAnimEnd?.Invoke();
        }
    }
}