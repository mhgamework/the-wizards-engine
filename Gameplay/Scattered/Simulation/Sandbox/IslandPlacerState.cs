using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Placing;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Simulation.Sandbox
{
    public class IslandPlacerState : ISandboxControllerState
    {
        private readonly Level level;
        private readonly EditorConfiguration config;

        public IslandPlacerState(Level level, EditorConfiguration config)
        {
            this.level = level;
            this.config = config;
        }

        public void Init()
        {
            config.Placer = new WorldPlacer(
                () => level.Islands,
                o => ((Island)o).Position,
                (o, newPos) => ((Island)o).Position = newPos,
                o => ((Island)o).GetBoundingBox(),
                () => level.CreateNewIsland(new Vector3(0, 0, 0)),
                o => level.RemoveIsland((Island)o)
                );
        }

        public void Update()
        {
        }

        public void Exit()
        {
            config.Placer = null;
        }
    }
}