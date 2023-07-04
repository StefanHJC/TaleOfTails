using Core.Infrastructure.Factories;
using Core.Logic;
using Core.Services;
using Core.Services.AssetManagement;
using Core.Services.Input;
using Core.UI;
using UnityEngine;
using Zenject;
using Cursor = Core.Logic.Cursor;

namespace Core.Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private GameBootstrapper _bootstrapper;

        public override void InstallBindings()
        {
            BindAssetService();
            BindInputService();
            BindRaycastService();
            BindDatabaseService();
            BindSceneLoader();
            BindUnitFactory(); //TODO
            BindSystemFactory();
            BindGameFactory();
            BindGameBootstrapper();
            BindLoadingCurtain();
            BindCursor();
            BindSoundService();
         //   BindWindowService();
        }

        private void BindSoundService()
        {
            Container
                .Bind<ISoundService>()
                .To<FMODSoundService>()
                .AsSingle()
                .NonLazy();
        }

        private void BindCursor()
        {
            Container
                .BindInterfacesAndSelfTo<Cursor>()
                .FromNew()
                .AsSingle();
        }

        private void BindGameFactory()
        {
            Container
                .Bind<IGameFactory>()
                .To<GameFactory>()
                .AsSingle();
        }


        private void BindSceneLoader()
        {
            Container
                .Bind<ISceneLoader>()
                .To<SceneLoader>()
                .AsSingle();
        }

        private void BindDatabaseService()
        {
            Container
                .Bind<IDatabaseService>()
                .To<LocalDatabaseService>()
                .AsSingle()
                .NonLazy();
        }

        private void BindLoadingCurtain()
        {
            Container
                .Bind<LoadingCurtain>()
                .FromInstance(TryGetLoadingCurtain())
                .AsSingle()
                .NonLazy();
        }

        private void BindGameBootstrapper()
        {
            Container
                .Bind<ICoroutineRunner>()
                .To<GameBootstrapper>()
                .FromComponentInNewPrefab(_bootstrapper)
                .AsSingle()
                .NonLazy();
        }

        private void BindSystemFactory()
        {
            Container
                .Bind<ISystemFactory>()
                .To<SystemFactory>()
                .FromNew()
                .AsSingle();
        }

        private void BindUnitFactory()
        {
            Container
                .Bind<IUnitFactory>()
                .To<UnitFactory>()
                .FromNew()
                .AsSingle();
        }

        private void BindRaycastService()
        {
            Container
                .Bind<IRaycastService>()
                .To<MouseRaycaster>()
                .FromComponentInNewPrefabResource(AssetPath.MouseRaycaster)
                .AsSingle();
            //TODO refactor this shit after menu impl
        }

        private void BindInputService()
        {
            Container
                .BindInterfacesTo<StandaloneInputService>()
                .FromNew()
                .AsSingle();
        }

        private void BindAssetService()
        {
            Container
                .Bind<IAssets>()
                .To<AssetProvider>()
                .FromNew()
                .AsSingle();
        }

        private LoadingCurtain TryGetLoadingCurtain()
        {
            LoadingCurtain curtain = FindObjectOfType<LoadingCurtain>();

            if (curtain == null)
                Debug.LogWarning("Cannot find loading curtain");

            return curtain;
        }
    }
}