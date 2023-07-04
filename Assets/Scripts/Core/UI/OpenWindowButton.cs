using Core.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class OpenWindowButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private WindowId _windowId;
        
        private IWindowService _windowService;

        public void Init(IWindowService windowService) => _windowService = windowService;

        public void Awake() => _button.onClick.AddListener(Open);

        public void OnDestroy() => _button.onClick.RemoveListener(Open);

        public void Open() => _windowService.Open(_windowId);
    }
}