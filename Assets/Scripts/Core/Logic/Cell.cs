using System;
using Core.Unit;
using Core.Unit.Component;
using UnityEngine;

namespace Core.Logic
{
    public abstract class Cell : MonoBehaviour
    {
        [SerializeField] private TriggerObserver _trigger;

        private SkinnedMeshRenderer _corpseMesh;
        private BaseUnit _corpse;

        public abstract bool IsTaken { get; set; }
        public abstract Vector2Int OffsetCoordinates { get; set; }

        public BaseUnit Corpse
        {
            get => _corpse;
            set
            {
                _corpse = value;
                _corpseMesh = _corpse.Mesh.GetComponentInChildren<SkinnedMeshRenderer>();
            }
        }

        public event Action<BaseUnit, Cell> Entered;
        public event Action<BaseUnit, Cell> Exited;

        private void Awake() => SubscribeTriggerEvents();

        private void OnDestroy() => UnsubscribeTriggerEvents();

        private void SubscribeTriggerEvents()
        {
            _trigger.Entered += OnUnitEnter;
            _trigger.Exited += OnUnitExit;
        }

        private void UnsubscribeTriggerEvents()
        {
            _trigger.Entered -= OnUnitEnter;
            _trigger.Exited -= OnUnitExit;
        }

        private void OnUnitEnter(Collider unitCollider)
        {
            Entered?.Invoke(unitCollider.GetComponentInParent<BaseUnit>(), this);

            if (_corpse != null)
                HideCorpse();
        }

        private void OnUnitExit(Collider unitCollider)
        {
            Exited?.Invoke(unitCollider.GetComponentInParent<BaseUnit>(), this);

            if (_corpse != null)
                ShowCorpse();
        }

        private void HideCorpse() => _corpse.gameObject.SetActive(false);

        private void ShowCorpse() => _corpse.gameObject.SetActive(true);
    }
}