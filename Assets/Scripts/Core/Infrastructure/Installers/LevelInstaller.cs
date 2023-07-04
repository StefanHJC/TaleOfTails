using Core.Logic;
using Core.Logic.Grid;
using Core.Services;
using Core.Services.Pathfinder;
using Core.UI;
using Zenject;

namespace Core.Infrastructure.Installers
{
    public class LevelInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindGrid();
            BindCellHighLighter();
            BindUnitSelector();
            BindPathRenderer();
            BindUnitCommandBuilder();
            BindPathfinding();
            BindCommandService();
            BindTurnResolver();
            BindActionResolver();
            BindWindowService();
            BindDialoguePlayer();
        }

        private void BindDialoguePlayer()
        {
            Container
                .Bind<DialoguePlayer>()
                .AsSingle();
        }

        private void BindWindowService()
        {
            Container
                .Bind<IWindowService>()
                .To<WindowService>()
                .AsSingle()
                .Lazy();
        }

        private void BindActionResolver()
        {
            Container
                .Bind<IUnitActionsResolver>()
                .To<UnitActionsResolver>()
                .AsSingle();
        }

        private void BindTurnResolver()
        {
            Container
                .Bind<ITurnResolver>()
                .To<InitiativeTurnResolver>()
                .FromNew()
                .AsSingle()
                .Lazy();
        }

        private void BindCommandService()
        {
            Container
                .Bind<ICommandInvoker>()
                .To<CommandInvoker>()
                .AsSingle()
                .Lazy();
        }

        private void BindPathfinding()
        {
            Container
                .Bind<IPathfinderService>()
                .To<AStarPathfinder>()
                .FromNew()
                .AsSingle()
                .Lazy();
        }

        private void BindUnitCommandBuilder()
        {
            Container
                .Bind<IUnitCommandBuilder>()
                .To<UnitCommandBuilder>()
                .AsSingle();
        }

        private void BindUnitSelector()
        {
            Container
                .Bind<IUnitSelector>()
                .To<UnitSelector>()
                .AsSingle();
        }

        private void BindCellHighLighter()
        {
            Container
                .Bind<ICellHighlighter>()
                .To<HexagonalCellHighlighter>()
                .FromNew()
                .AsSingle();
        }

        private void BindGrid()
        {
            Container
                .Bind<AbstractGrid>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();
        }

        private void BindPathRenderer()
        {
            Container
                .Bind<IPathRenderer>()
                .To<SpriteLineRenderer>()
                .AsSingle();
        }
    }
}