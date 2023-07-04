using System.Collections.Generic;
using Core.Logic.Grid;
using Core.Services.AssetManagement;
using Core.Unit;
using EPOOutline;
using UnityEngine;
using Zenject;

namespace Core.Services
{
    public static class SelectorSettings
    {
        public static OutlinePreset Red = new OutlinePreset(new Color(110, 3, 2, 168), .7f, false);
        public static OutlinePreset Green = new OutlinePreset(new Color(16, 141, 49, 168), .7f, false);
        public static OutlinePreset Unreachable = new OutlinePreset(new Color(16, 141, 49, 168), .7f, true);
    }

    public readonly struct OutlinePreset
    {
        public readonly Color OutlineColor;
        public readonly float Dilate;
        public readonly bool IsUnreachable;

        public OutlinePreset(Color outlineColor, float dilate, bool isUnreachable)
        {
            OutlineColor = new Color(outlineColor.r / 255, outlineColor.g / 255, outlineColor.b / 255, outlineColor.a / 255);
            Dilate = dilate;
            IsUnreachable = isUnreachable;
        }
    }

    public class UnitSelector : IUnitSelector
    {
        private readonly IAssets _assets;
        private readonly AbstractGrid _grid;

        private GameObject _selection;
        private BaseUnit _outlined; // TEMP
        private List<BaseUnit> _outlinedList = new List<BaseUnit>();

        [Inject]
        public UnitSelector(IAssets assets, AbstractGrid grid)
        {
            _grid = grid;
            _assets = assets;
            InstantiateSelectionPrefab();
        }

        public void ResetAll()
        {
            _outlinedList = new List<BaseUnit>();
            _outlined = null;
        }

        public void Select(BaseUnit unit)
        {
            if (_selection == null)
                return;

            _selection.SetActive(true);
            Move(to: unit);
        }

        public void Deselect()
        {
            if (_selection != null)
                _selection.SetActive(false);
        }

        public void RenderOutline(IEnumerable<BaseUnit> units, OutlinePreset mode)
        {
            foreach (BaseUnit unit in units)
            {
                if(_outlinedList.Contains(unit) == false)
                {
                    if (mode.IsUnreachable)
                    {
                        unit.UnreachableOutline.enabled = true;
                        _outlinedList.Add(unit);
                        
                        continue;
                    }
                    unit.Outline.enabled = true;
                    SetOutlinePreset(mode, unit);
                    _outlinedList.Add(unit);
                }
            }
        }

        public void RenderOutline(BaseUnit unit, OutlinePreset mode)
        {
            if (_outlined != null)
                RenderOffOutline(_outlined);
            
            if (mode.IsUnreachable)
            {
                unit.UnreachableOutline.enabled = true;
            }
            else
            {
                unit.Outline.enabled = true;
                SetOutlinePreset(mode, unit);
            }
            _outlined = unit;
        }

        public void RenderOffOutline(BaseUnit unit)
        {
            unit.Outline.enabled = false;
            _outlined = null;
        }

        public void RenderOffAllOutlines()
        { 
            if (_outlinedList.Count == 0)
                return;

            foreach (BaseUnit unit in _outlinedList)
            {
                unit.Outline.enabled = false;
                unit.UnreachableOutline.enabled = false;
            }

            _outlinedList.RemoveAll(unit => unit);
        }

        private void SetOutlinePreset(OutlinePreset mode, BaseUnit unit)
        {
            unit.Outline.OutlineParameters.Color = mode.OutlineColor;
            unit.Outline.BackParameters.Color = mode.OutlineColor;
            unit.Outline.OutlineParameters.DilateShift = mode.Dilate;
            unit.Outline.BackParameters.DilateShift = mode.Dilate;
        }

        private void Move(BaseUnit to)
        {
            if (to == null || _grid == null) 
                return;
            _selection.transform.position = new Vector3(to.transform.position.x, _grid.transform.position.y + .1f, to.transform.position.z);
            _selection.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Temp
            _selection.transform.parent = to.transform;
        }

        private void InstantiateSelectionPrefab()
        {
            _selection = _assets.Instantiate(AssetPath.SelectedHexagon);
            _selection.gameObject.SetActive(false);
        }
    }
}