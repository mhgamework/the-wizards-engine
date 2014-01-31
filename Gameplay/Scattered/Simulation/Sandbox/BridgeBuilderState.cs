using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Simulation.Sandbox
{
    public class BridgeBuilderState : ISandboxControllerState
    {
        private readonly Level level;
        private readonly EditorConfiguration configuration;

        private Island start = null;

        public BridgeBuilderState(Level level, EditorConfiguration configuration)
        {
            this.level = level;
            this.configuration = configuration;
        }

        public void Init()
        {
            start = null;
            configuration.SelectableProvider = SandboxControllerSimulator.CreateIslandSelectableProvider(level, onClickIsland);
        }

        private void onClickIsland(Island obj)
        {
            if (start == null)
            {
                start = obj;
                return;
            }

            start.AddBridgeTo(obj);
            start = null;
        }

        public void Update()
        {
            if (start == null || configuration.SelectableProvider.Targeted == null) return;

            TW.Graphics.LineManager3D.AddLine(start.Position, ((Island)configuration.SelectableProvider.Targeted).Position, new Color4(1, 1, 0));
        }

        public void Exit()
        {
            configuration.SelectableProvider = null;
        }
    }
}