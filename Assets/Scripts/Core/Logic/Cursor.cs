using System.Collections.Generic;
using Core.Database;
using Core.Services;
using UnityEngine;
using Zenject;

namespace Core.Logic
{
    public class Cursor
    {
        private const int RotationOffset = 90;

        private readonly IDatabaseService _data;

        private Dictionary<CursorState, Texture2D> _sprites = new Dictionary<CursorState, Texture2D>();
        private CursorState _state;
        private float _rotation;

        public CursorState State
        {
            get => _state;
            set => SetState(value);
        }

        public float Rotation
        {
            get => _rotation;
            set => SetRotation(value);
        }

        public Texture2D Sprite => _sprites[State];
        public bool IsReady => _sprites.Count > 0;

        [Inject]
        public Cursor(IDatabaseService database)
        {
            _data = database;
        }

        public void Init()
        {
            _sprites = _data
                .TryGetCursorData()
                .Sprites;

            _state = CursorState.Default;
            UnityEngine.Cursor.visible = false;
            SetDefaultCursor();
        }

        private void SetState(CursorState state)
        {
            _state = state;
            UnityEngine.Cursor.SetCursor(_sprites[state], Vector2.zero, CursorMode.Auto);
            UnityEngine.Cursor.visible = false;
        }

        private void SetRotation(float value)
        {
            _rotation = value;
        }

        private void SetDefaultCursor()
        {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.SetCursor(_sprites[CursorState.Default], Vector2.zero, CursorMode.Auto);
        }
    }

    public enum CursorState
    {
        Default,
        InterfaceClick,
        Move,
        MeleeAttack,
        RangeAttack,
        UseAbility
    }
}