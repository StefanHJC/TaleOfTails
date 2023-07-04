using System.Collections.Generic;
using Core.Database;
using Core.Infrastructure;
using Core.Logic;
using Core.Services;
using Core.UI.HUD;
using Core.Unit.Ability;
using Core.Unit.Command;
using Core.Unit.Component;
using EPOOutline;
using UnityEngine;
using Zenject;

namespace Core.Unit
{
    public class BaseUnit : GridEntity
    {
        private const string MeshLayerName = "Unit Mesh";

        [SerializeField] private Transform _torso;
        [SerializeField] private Transform _head;
        [SerializeField] private GameObject _meshObject;
        [SerializeField] private Outlinable _outline;
        [SerializeField] private Outlinable _unreachableOutline;
        [SerializeField] private EffectHandler _effectHandler;
        [SerializeField] private UnitHealth _health;

        private HealthRenderer _healthRenderer;
        private List<BaseAbility> _abilities;
        private Cell _currentCell;
        private UnitStaticData _config;
        private ICommandReceiver _commandReceiver;
        private ISoundService _sound;
        private LayerMask _meshlayer;

        public UnitHealth Health => _health;
        public Transform Torso => _torso;
        public Transform Head => _head;
        public UnitStaticData Config => _config;
        public override Outlinable Outline => _outline;
        public  Outlinable UnreachableOutline => _unreachableOutline;
        public override GameObject Mesh => _meshObject;
        public HealthRenderer HealthRenderer => _healthRenderer;
        public IReadOnlyList<BaseAbility> Abilities => _abilities;
        public ISoundService Sound => _sound;
        public ICommandReceiver CommandReceiver => _commandReceiver;
        public bool IsRanged { get; private set; }
        public int ActionPoints { get; set; }

        public int TeamId;
        public int Initiative;
        public int Speed { get; set; }
        public Cell CurrentCell; // TEMP

        [Inject]
        public void Construct(ISoundService sound, UnitStaticData config, ITurnResolver turnResolver, IEnumerable<BaseAbility> abilities, IGameFactory gameFactory)
        {
            _commandReceiver = new BaseUnitCommandReceiver(this);
            _abilities = new List<BaseAbility>();
            _sound = sound;
            _config = config;
            Speed = _config.Speed;
            turnResolver.RoundStarted += OnNewRound;
            _healthRenderer = gameFactory.UI.CreateHealthRenderer();

            Initiative = _config.Initiative;
            ActionPoints = _config.Speed;
            InitAbilities(abilities);
            
            _healthRenderer.Construct(this);
        }

        public void OnUnitTurnEnd()
        {
            if (_effectHandler.HasEffect == false)
            {
                Speed = Config.Speed;
                ActionPoints = Config.Speed;
            }
            _effectHandler.OnTurnEnd();
        }

        private void Start()
        {
            IsRanged = TryGetComponent<UnitRangeAttack>(out _);
        }

        private void OnTriggerEnter(Collider other) // temp
        {
            Cell cell = other.gameObject.GetComponentInParent<Cell>();

            if (cell != null)
                CurrentCell = cell; // TODO BEWARE
        }

        private void InitAbilities(IEnumerable<BaseAbility> abilities)
        {
            foreach (BaseAbility ability in abilities)
                 _abilities.Add(ability);
        }

        private void OnNewRound() => _effectHandler.OnNewRound();
    }
}