using System;
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
        private readonly Action<Island> afterCreate;

        public IslandPlacerState(Level level, EditorConfiguration config)
            : this(level, config, _ => { })
        {

        }
        public IslandPlacerState(Level level, EditorConfiguration config, Action<Island> afterCreate)
        {
            this.level = level;
            this.config = config;
            this.afterCreate = afterCreate;
        }

        public void Init()
        {
            config.Placer = new WorldPlacer(
                () => level.Islands,
                o => ((Island)o).Position,
                (o, newPos) => ((Island)o).Position = newPos,
                o => ((Island)o).GetBoundingBox(),
                () =>
                    {
                        var isl = level.CreateNewIsland(new Vector3(0, 0, 0));
                        afterCreate(isl);
                        return isl;
                    },
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