using Core.Database;
using Core.Services;
using Core.Services.AssetManagement;
using Core.UI;
using Core.UI.HUD;
using Core.Unit;
using UnityEngine;
using Utils;
using Zenject;

namespace Core.Infrastructure.Factories
{
    public class UIFactory : IUIFactory
    {
        private readonly IAssets _assets;
        private readonly IDatabaseService _data;

        private DiContainer _diContainer;
        private Canvas _root;
        private HUD _hud;
        private Canvas _barRoot;

        public Canvas Root => _root;
        public HUD HUD => _hud;

        [Inject]
        public UIFactory(DiContainer diContainer, IAssets assets, IDatabaseService data)
        {
            _diContainer = diContainer;
            _assets = assets;
            _data = data;
        }

        public void Init(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public HUD CreateHUD()
        {
            HUD hud = _assets.Instantiate(AssetPath.HUD).GetComponent<HUD>();
            _diContainer.Inject(hud);
            _hud = hud;
            return hud;
        }

        public Canvas CreateUiRoot()
        {
            if (_root !=  null) 
                return _root;

            GameObject uiRootGameObject = _assets.Instantiate(AssetPath.UiRoot);
            Canvas uiRoot = uiRootGameObject.GetComponent<Canvas>();
            _root = uiRoot;

            return uiRoot;
        }

        public HealthRenderer CreateHealthRenderer()
        {
            if (_barRoot == null)
            {
                GameObject uiRootGameObject = _assets.Instantiate(AssetPath.UiRoot);
                _barRoot = uiRootGameObject.GetComponent<Canvas>();
                _barRoot.transform.parent = _root.transform;
            }

            GameObject healthBarInstance = Object.Instantiate(_assets.Load(AssetPath.HealthBar), _barRoot.transform);

            return healthBarInstance.GetComponent<HealthRenderer>();
        }

        public SystemMenuWindow CreateSystemMenu()
        {
            WindowData windowData = _data.TryGetWindowData(WindowId.SystemMenu);
            BaseWindow windowInstance = Object.Instantiate(windowData.Template, Root.transform);
            _diContainer.Inject(windowInstance);

            return windowInstance as SystemMenuWindow;
        }

        public WinWindow CreateWinScreen()
        {
            WindowData windowData = _data.TryGetWindowData(WindowId.WinWindow);
            BaseWindow windowInstance = Object.Instantiate(windowData.Template, Root.transform);
            _diContainer.Inject(windowInstance);

            return windowInstance as WinWindow;
        }

        public LoseWindow CreateLoseScreen()
        {
            WindowData windowData = _data.TryGetWindowData(WindowId.LoseWindow);
            BaseWindow windowInstance = Object.Instantiate(windowData.Template, Root.transform);
            _diContainer.Inject(windowInstance as LoseWindow);

            return windowInstance as LoseWindow;
        }

        public DialogueWindow CreateDialogueWindow()
        {
            WindowData windowData = _data.TryGetWindowData(WindowId.DialogueWindow);
            DialogueWindow windowInstance = (DialogueWindow)Object.Instantiate(windowData.Template, Root.transform);
            _diContainer.Inject(windowInstance);

            return windowInstance;
        }

        public EducationWindow CreateEducationalWindow()
        {
            WindowData windowData = _data.TryGetWindowData(WindowId.EducationWindow);
            BaseWindow windowInstance = Object.Instantiate(windowData.Template, Root.transform);
            _diContainer.Inject(windowInstance);

            return windowInstance as EducationWindow;
        }

        public UnitStatsCard CreateUnitStatsWindow(BaseUnit unit)
        {
            WindowData windowData = _data.TryGetWindowData(WindowId.StatsCard);
            UnitStatsCard windowInstance = (UnitStatsCard)Object.Instantiate(windowData.Template, _barRoot.transform);
            windowInstance.transform.position = Camera.main.WorldToScreenPoint(unit.transform.position.WithY(2).WithX(2));
            windowInstance.Construct(unit);

            return windowInstance;
        }

        public AuthorsWindow CreateAuthorsWindow()
        {
            WindowData windowData = _data.TryGetWindowData(WindowId.EducationWindow);
            BaseWindow windowInstance = Object.Instantiate(windowData.Template, Root.transform);
            _diContainer.Inject(windowInstance);

            return windowInstance as AuthorsWindow;
        }
    }
}