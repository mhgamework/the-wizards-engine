using System;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.Scattered.Simulation.Sandbox
{
    public class ConstructionPlacerState : ISandboxControllerState
    {
        private readonly Level level;
        private readonly EditorConfiguration configuration;
        private readonly Func<Island,Construction> constructionFactory;

        public ConstructionPlacerState(Level level, EditorConfiguration configuration, Func<Island, Construction> constructionFactory)
        {
            this.level = level;
            this.configuration = configuration;
            this.constructionFactory = constructionFactory;
        }

        public void Init()
        {
            configuration.SelectableProvider = SandboxControllerSimulator.CreateIslandSelectableProvider(level,onClickIsland);
        }

        public void Update()
        {

        }

        public void Exit()
        {
            configuration.SelectableProvider = null;
        }
        private void onClickIsland(Island obj)
        {
            obj.Construction = constructionFactory(obj);
            obj.Inventory.Clear();
        }
    }
}