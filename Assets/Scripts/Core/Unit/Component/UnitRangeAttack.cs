using System;
using Core.Logic;
using Core.Logic.Projectile;
using Core.Unit.Command;
using UnityEngine;
using Utils;

namespace Core.Unit.Component
{
    public class UnitRangeAttack : MonoBehaviour, IAttackCommandHandler
    {
        [SerializeField] private Transform _projectileParent;
        [SerializeField] private BaseUnitAnimator _animator;
        [SerializeField] private BaseUnit _unit;

        private TargetProjectile _projectile;
        
        private bool _isAttacking;
        
        public Transform ProjectileParent => _projectileParent;
        public TargetProjectile Projectile => _projectile;
        public CommandTypeId HandlerType => CommandTypeId.Attack;
        public AttackTypeId AttackType => AttackTypeId.Ranged;
        public BaseUnit Target { get; set; }

        public event Action Executed;

        private Vector3 _temp;

        public void Execute(IUnitCommand command, Action onExecuted)
        {
            AttackCommand attackCommand = (AttackCommand)command;

            Executed = onExecuted;
            Attack(attackCommand.Target);
        }

        public void Attack(BaseUnit target)
        {
            _isAttacking = true;
            Target = target;
            _animator.PlayRangeAttack();
            _unit.Sound.PlayOnce(_unit.Config.MeleeAttackStartSFX, transform.position);
        }

        public bool CanAttack(Cell from, Cell to, BaseUnit estimatedTarget, out BaseUnit obstacle)
        {
            Vector3 estimatedTargetPosition = to.transform.position.WithY(1.5f);
            Vector3 estimatedProjectileParent = from.transform.position.WithY(1.5f) + ProjectileParent.transform.forward * 3;

            Vector3 directionToTarget = estimatedTargetPosition - estimatedProjectileParent;
            _temp = directionToTarget;
            LayerMask meshLayer = 1 << LayerMask.NameToLayer("Unit Mesh");

            obstacle = null;

            if (Physics.SphereCast(estimatedProjectileParent, .3f,directionToTarget,
                    out RaycastHit hitInfo, Mathf.Infinity, meshLayer))
            {
                BaseUnit hittedUnit = hitInfo.collider.gameObject.GetComponentInParent<BaseUnit>();

                if (hittedUnit != null)
                {
                    if (hittedUnit == estimatedTarget)
                    {
                        return true;
                    }
                    else
                    {
                        obstacle = hittedUnit;
                        return false;
                    }
                }
            }
            return true;
        }

        public bool CanAttack(Cell from, Cell to, BaseUnit estimatedTarget)
        {
            Vector3 estimatedTargetPosition = to.transform.position.WithY(1.5f);
            ProjectileParent.LookAt(to.transform);
            Vector3 estimatedProjectileParent = from.transform.position.WithY(1.5f) + ProjectileParent.transform.forward * 3;

            Vector3 directionToTarget = estimatedTargetPosition - estimatedProjectileParent;
            _temp = directionToTarget;
            LayerMask meshLayer = 1 << LayerMask.NameToLayer("Unit Mesh");

            if (Physics.SphereCast(estimatedProjectileParent, .3f, directionToTarget,
                    out RaycastHit hitInfo, Mathf.Infinity, meshLayer))
            {
                BaseUnit hittedUnit = hitInfo.collider.gameObject.GetComponentInParent<BaseUnit>();
                
                if (hittedUnit != null)
                {
                    if (hittedUnit == _unit)
                    {
                        Vector3 directionToTarget1 = estimatedTargetPosition - estimatedProjectileParent.WithY(1f);
                        if (Physics.SphereCast(estimatedProjectileParent.WithY(1f), .3f, directionToTarget1,
                                out RaycastHit hitInfo1, Mathf.Infinity, meshLayer))
                        {
                            BaseUnit hittedUnit1 = hitInfo1.collider.gameObject.GetComponentInParent<BaseUnit>();

                            if (hittedUnit1 == estimatedTarget)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }

                    if (hittedUnit == estimatedTarget)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void Update()
        {
            Debug.DrawRay(_unit.CurrentCell.transform.position.WithY(1.5f).WithZ(.5f), _temp, Color.magenta, 100f);
        }

        private void Start()
        {
            _projectile = _unit.Config.Projectile;
        }

        private void ThrowProjectile()
        {
            if (Target != null)
                _projectileParent.LookAt(Target.Torso);

            TargetProjectile projectile = Instantiate(_projectile, _projectileParent.transform.position, _projectileParent.rotation);
            projectile.Construct(_unit.Config.ProjectileFlightSFX, _unit.Config.ProjectileHitSFX, _unit);
            projectile.TargetReached += OnTargetHit;
        }

        private void OnTargetHit(BaseUnit hittedUnit)
        {
            int damage = 0;

            if (_isAttacking)
            {
                hittedUnit.GetComponent<UnitHealth>().TakeDamage(_unit.Config.RangeDamage, Executed);
                _unit.ActionPoints = 0;
            }
            _isAttacking = false;
            Target = null;
        }
    }
}