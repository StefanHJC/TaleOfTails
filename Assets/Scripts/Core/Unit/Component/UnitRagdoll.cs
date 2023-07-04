using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Unit.Component
{
    public class UnitRagdoll : MonoBehaviour
    {
        private const string LayerName = "Unit Mesh";

        [SerializeField] private BaseUnit _unit;

        private Rigidbody[] _rigidbody;

        private Animator[] _animators;

        private void Start()
        {
            _rigidbody = _unit.GetComponentsInChildren<Rigidbody>();
            _animators = _unit.GetComponentsInChildren<Animator>();
        }

        [Button]
        public void EnableRagdoll(Vector3 force = new())
        {
            foreach (var rb in _rigidbody)
            {
                rb.isKinematic = false;
                rb.AddForce(force);
            }

            foreach (var animator in _animators)
                animator.enabled = false;
        }

        public void DisableRagdoll()
        {
            foreach (var rb in _rigidbody)
                rb.isKinematic = true;
        }
    }
}