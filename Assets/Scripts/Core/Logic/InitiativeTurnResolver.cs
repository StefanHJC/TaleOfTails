using System;
using System.Collections.Generic;
using System.Linq;
using Core.Logic.Grid;
using Core.Logic.Player;
using Core.Services.Pathfinder;
using Core.Unit;
using Core.Unit.Command;
using Core.Unit.Component;
using UnityEngine;
using Utils.DataStructs;
using Zenject;

namespace Core.Logic
{
    public class InitiativeTurnResolver : ITurnResolver
    {
        private readonly IPathfinderService _pathfinder;

        private List<BaseUnit> _units;
        private IEnumerable<IPlayer> _players;
        private PriorityQueue<BaseUnit, int> _unitTurnQueue;
        private AbstractGrid _grid;
        private int _currentRound;

        public BaseUnit ActiveUnit { get; private set; }
        public IPlayer CurrentPlayer => _players.First(player => player.TeamId == ActiveUnit.TeamId);
        public IReadOnlyList<BaseUnit> UnitTurnOrder => _unitTurnQueue.GetOrder();
        public IReadOnlyList<BaseUnit> Units => _units;
        public int CurrentRound => _currentRound;

        public event Action TurnStarted;
        public event Action RoundStarted;

        [Inject]
        public InitiativeTurnResolver(IPathfinderService pathfinder, AbstractGrid grid)
        {
            _pathfinder = pathfinder;

            _unitTurnQueue = new PriorityQueue<BaseUnit, int>();
            _grid = grid;
        }

        public void Start(IEnumerable<IPlayer> players)
        {
            List<BaseUnit> activeUnits = GetSpawnedUnits(players);
            _players = players;

            Init(activeUnits);

            _currentRound = 1;
            ActiveUnit = _unitTurnQueue.Dequeue();
            ActiveUnit.CommandReceiver.CommandExecuted += TryEndTurn; // TODO rework
            _grid.Units.UnitDied += OnUnitDied;

            TurnStarted?.Invoke();
            RoundStarted?.Invoke();
        }

        public IReadOnlyList<BaseUnit> GetTurnOrderForNextRound()
        {
            PriorityQueue<BaseUnit, int> nextTurnOrder = new PriorityQueue<BaseUnit, int >();
            if (Units != null) 
                foreach (BaseUnit baseUnit in Units) 
                    nextTurnOrder.Enqueue(baseUnit, ref baseUnit.Config.Initiative);

            return nextTurnOrder.GetOrder();
        }

        private List<BaseUnit> GetSpawnedUnits(IEnumerable<IPlayer> activePlayers)
        {
            List<BaseUnit> spawnedUnits = new List<BaseUnit>();

            foreach (IPlayer activePlayer in activePlayers)
                spawnedUnits.AddRange(activePlayer.ControllableUnits);

            return spawnedUnits;
        }

        private void Init(IEnumerable<BaseUnit> units)
        {
            foreach (BaseUnit unit in units)
            {
                _unitTurnQueue.Enqueue(unit, ref unit.Initiative);
                unit.GetComponent<UnitDeath>().Died += OnUnitDied;
            }

            _units = units.ToList();
        }

        private void TryEndTurn(ICommandReceiver _)
        {
            if (ActiveUnit.ActionPoints > 0)
            {
                TurnStarted?.Invoke();
                return;
            }

            if (_unitTurnQueue.Count <= 0)
            {
                _units = (List<BaseUnit>)_grid.Units.List; // TODO end round

                foreach (BaseUnit unit in _units)
                {
                    _unitTurnQueue.Enqueue(unit, ref unit.Initiative);
                }
                _currentRound++;
                RoundStarted?.Invoke();
            }
            EndTurn();
        }

        private void EndTurn()
        {
            ActiveUnit.OnUnitTurnEnd();
            ActiveUnit.CommandReceiver.CommandExecuted -= TryEndTurn;
            ActiveUnit = _unitTurnQueue.Dequeue();
            ActiveUnit.CommandReceiver.CommandExecuted += TryEndTurn;

            TurnStarted?.Invoke();
        }

        private void OnUnitDied(BaseUnit unit)
        {
            _unitTurnQueue.TryRemove(unit);
            unit.GetComponent<UnitDeath>().Died -= OnUnitDied;
            _units.Remove(unit);
        }
    }
}