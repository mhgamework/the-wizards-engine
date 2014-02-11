using System;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.Simulation.Constructions;
using MHGameWork.TheWizards.Scattered.Simulation.Sandbox;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.HotbarCore;
using MHGameWork.TheWizards.SkyMerchant._Engine;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.Scattered.Simulation
{
    /// <summary>
    /// Simulator processes inputs in such a way that the user can play the game!
    /// </summary>
    public class PlayControllerSimulator : ISimulator
    {
        private readonly Level level;
        private readonly RoundState roundState;
        private HotbarController hotbarController;

        public PlayControllerSimulator(Level level, EditorConfiguration configuration, RoundState roundState)
        {
            this.level = level;
            this.roundState = roundState;


            var bar = new Hotbar();
            var view = new HotbarTextView(bar, new Rendering2DComponentsFactory());
            hotbarController = new HotbarController(bar, view);

            bar.SetHotbarItem(0, new ControllerStateItem("Bridge",new BridgeBuilderState(level, configuration)));
            bar.SetHotbarItem(1, new ControllerStateItem("Warehouse",new ConstructionPlacerState(level, configuration, level.createWarehouseConstruction)));
        }


        public void Simulate()
        {
            hotbarController.Update();
            hotbarController.UpdateSelectedItem();
        }

        public class ControllerStateItem : IHotbarItem
        {
            private readonly ISandboxControllerState state;

            public ControllerStateItem(string name, ISandboxControllerState state)
            {
                Name = name;
                this.state = state;
            }
            public string Name { get; private set; }
            public void OnSelected()
            {
                state.Init();
            }

            public void OnDeselected()
            {
                state.Exit();
            }

            public void Update()
            {
                state.Update();
            }
        }
    }
}