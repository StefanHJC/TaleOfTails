using System.Collections.Generic;
using Core.Logic;
using Core.Unit;
using Core.Unit.Component;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.HUD
{
    public class TurnLine : MonoBehaviour
    {
        [SerializeField] private TurnLineElement _elementPrefab;
        [SerializeField] private HorizontalLayoutGroup _content;
        [SerializeField] private Image _currentUnit;
        [SerializeField] private int _maxElements = 8;

        private BaseUnit _current;
        private ITurnResolver _turnResolver;
        private List<TurnLineElement> _elements = new List<TurnLineElement>();
        private Queue<BaseUnit> _nextRoundTurnOrder = new Queue<BaseUnit>();

        private BaseUnit NextUnit
        {
            get
            {
                if(_nextRoundTurnOrder.Count == 0)
                    _nextRoundTurnOrder = new Queue<BaseUnit>(_turnResolver.GetTurnOrderForNextRound());

                return _nextRoundTurnOrder.Dequeue();
            }
        }

        public void Construct(ITurnResolver turnResolver)
        {
            _turnResolver = turnResolver;
        }

        private void Start()
        {
            AddElementsForFirstRound();
            FillLine();
            SetCurrentUnit(_turnResolver.ActiveUnit);

            _turnResolver.TurnStarted += OnNewTurn;
        }

        private void OnDestroy() => _turnResolver.TurnStarted -= OnNewTurn;

        private void AddElementsForFirstRound()
        {
            foreach (BaseUnit unit in _turnResolver.UnitTurnOrder)
            {
                if (_elements.Count >= _maxElements)
                    return;

                TurnLineElement element = Instantiate(_elementPrefab, _content.transform);
                element.Construct(unit);
                unit.GetComponent<UnitDeath>().Died += OnUnitDeath;
                _elements.Add(element);
            }
        }

        private void FillLine()
        {
            while (_elements.Count < _maxElements)
            {
                TurnLineElement element = Instantiate(_elementPrefab, _content.transform);
                element.Construct(NextUnit);
                _elements.Add(element);
            }
        }

        private void OnNewTurn()
        {
            if (_current != _turnResolver.ActiveUnit)
                SetCurrentUnit(_turnResolver.ActiveUnit);

            if (_elements[0] != null)
                Destroy(_elements[0].gameObject);
            _elements.RemoveAt(0);
            FillLine();
            SetCurrentUnit(_turnResolver.ActiveUnit);
        }

        private void OnUnitDeath(BaseUnit unit)
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                if (_elements[i].Unit == unit)
                {
                    _elements[i].AnimateFadeOut();
                    Destroy(_elements[i].gameObject, 1f);
                }
            }
            for (int i = 0; i < _elements.Count; i++)
            {
                if (_elements[i] == null)
                {
                    _elements.Remove(_elements[i]);
                }
            }
            FillLine();
        }

        private void SetCurrentUnit(BaseUnit currentUnit)
        {
            _currentUnit.sprite = currentUnit.Config.UiImage;
            _current = currentUnit;
        }
    }
}