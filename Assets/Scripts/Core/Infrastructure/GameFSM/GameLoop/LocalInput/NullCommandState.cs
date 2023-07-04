using Core.Logic;
using Core.Logic.Grid;
using Core.Services;
using Core.Services.Input;
using Core.UI;
using Core.Unit;
using UnityEngine;
using Utils.TypeBasedFSM;
using Cursor = Core.Logic.Cursor;

namespace Core.Infrastructure.GameFSM.GameLoop.LocalInput
{
    public class NullCommandState : IState
    {
        private readonly Cursor _cursor;

        public NullCommandState(Cursor cursor)
        {
            _cursor = cursor;

        }

        public void Enter()
        {
            _cursor.State = CursorState.Default;
        }
        
        public void Exit()
        {

        }
    }
}