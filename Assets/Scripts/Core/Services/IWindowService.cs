using System;
using Core.UI;

namespace Core.Services
{
    public interface IWindowService
    {
        public event Action<GameLoopButtonAction> GameLoopButtonClicked;
        public BaseWindow Open(WindowId windowId);
    }
}