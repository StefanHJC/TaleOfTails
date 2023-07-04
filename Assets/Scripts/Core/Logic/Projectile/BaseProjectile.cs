using Core.Unit;
using FMODUnity;
using UnityEngine;
using Utils;

namespace Core.Logic.Projectile
{
    public abstract class BaseProjectile : MonoBehaviour
    {
        [SerializeField] private StudioEventEmitter _emitter;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private ParticleSystem _hitVfx;
        [SerializeField] private ParticleSystem _flashVfx;
        [SerializeField] private float _overlapSphereRadius;
        [SerializeField] private float _speed;
        [SerializeField] private string _layerName = "Unit Mesh";

        private LayerMask _layer;
        private EventReference _flightSfx;
        private EventReference _hitSfx;
        private bool _isHitted;
        private BaseUnit _caster;

        protected Collider[] Collisions = new Collider[1];

        public virtual void Construct(EventReference flightSFX, EventReference hitSfx, BaseUnit caster)
        {
            _flightSfx = flightSFX;
            _hitSfx = hitSfx;
            _caster = caster;

            _emitter.EventReference = _flightSfx;
        }

        protected virtual void OnHit()
        {
            ParticleSystem hitInstance = Instantiate(_hitVfx, transform.position, Quaternion.identity);
            Destroy(hitInstance.gameObject, _hitVfx.main.duration + 5);
            _caster.Sound.PlayOnce(_hitSfx, transform.position);
            Destroy(gameObject);
        }

        private void Start()
        {
            _layer = 1 << LayerMask.NameToLayer(_layerName);

            GameObject flashGameObject = Instantiate(_flashVfx.gameObject, transform.position, Quaternion.identity);
            Destroy(flashGameObject, _flashVfx.main.duration);
        }

        private void FixedUpdate()
        {
            if (_speed == 0)
                return;

            _rigidbody.velocity = transform.forward * _speed;
            int collisionCount = Physics.OverlapSphereNonAlloc(transform.position, _overlapSphereRadius, Collisions, _layer);

            if (collisionCount > 0 && _isHitted == false)
            {
                _isHitted = true;
                OnHit();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _overlapSphereRadius);
        }
    }
}