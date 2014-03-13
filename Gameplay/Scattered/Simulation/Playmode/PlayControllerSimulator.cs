using System;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Selecting;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.Simulation.Playmode;
using MHGameWork.TheWizards.Scattered.Simulation.Sandbox;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.HotbarCore;
using MHGameWork.TheWizards.SkyMerchant._Engine;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using System.Linq;
using SlimDX;
using SlimDX.DirectInput;
using MHGameWork.TheWizards.Scattered._Engine;

namespace MHGameWork.TheWizards.Scattered.Simulation
{
    /// <summary>
    /// Simulator processes inputs in such a way that the user can play the game!
    /// </summary>
    public class PlayControllerSimulator : ISimulator
    {
        private Level level;
        private RoundState roundState;
        private readonly InterIslandMovementSimulator movementSimulator;
        private HotbarController hotbarController;
        private float buildPhaseStart;
        private Textarea debugText;
        private int buildModeDuration = 15;

        public PlayControllerSimulator(Level level, EditorConfiguration configuration, RoundState roundState, InterIslandMovementSimulator movementSimulator)
        {
            this.level = level;
            this.roundState = roundState;
            this.movementSimulator = movementSimulator;


            var bar = new Hotbar();
            var view = new HotbarTextView(bar, new Rendering2DComponentsFactory());
            hotbarController = new HotbarController(bar, view);

            bar.SetHotbarItem(0, new ControllerStateItem("Bridge", new BridgeBuilderState(level, configuration)));

            buildPhaseStart = TW.Graphics.TotalRunTime;
            createDebugtext();

            initIslandSelector();
        }

        /// <summary>
        /// TODO: move to rendering layer?
        /// </summary>
        private void createDebugtext()
        {
            debugText = new Textarea();
            debugText.Position = new Vector2(10, 10);
            debugText.Size = new Vector2(400, 50);
        }
        /// <summary>
        /// TODO: move to rendering layer?
        /// </summary>
        private void updateDebugText()
        {
            debugText.Text = "";
            if (roundState.CombatPhase)
                debugText.Text += "Combat phase! " + level.Travellers.Count(t => t.Type.IsEnemy) + " enemies left";
            else
                debugText.Text += "Build phase. Attack commences in " + Math.Floor(buildPhaseStart + buildModeDuration - TW.Graphics.TotalRunTime) + " seconds.";
        }


        public void Simulate()
        {
            simulateHotbar();

            simulateFlyButton();
            simulateClusterFlight();

            //roundState.ExecuteDuringBuildPhase(() =>
            //    {
            //        if (buildPhaseStart + buildModeDuration < TW.Graphics.TotalRunTime)
            //        {
            //            hotbarController.Bar.GetHotbarItem(hotbarController.Bar.SelectedSlot).OnDeselected();
            //            roundState.CombatPhase = true;

            //            buildPhaseStart = TW.Graphics.TotalRunTime;
            //            return;
            //        }

            //        hotbarController.Update();
            //        hotbarController.UpdateSelectedItem();
            //    });

            //roundState.ExecuteDuringCombatPhase(() =>
            //    {
            //        if (buildPhaseStart + 3 > TW.Graphics.TotalRunTime) return;
            //        if (level.Travellers.Any(t => t.Type.IsEnemy)) return;

            //        roundState.CombatPhase = false;
            //        buildPhaseStart = TW.Graphics.TotalRunTime;

            //        hotbarController.Bar.GetHotbarItem(hotbarController.Bar.SelectedSlot).OnSelected();
            //    });

            //updateDebugText();

            //if (TW.Graphics.Keyboard.IsKeyDown(Key.N)) movementSimulator.MovementSpeed = 100;
            //else movementSimulator.MovementSpeed = 10;
        }




        private Island flyIsland = null;

        private CameraInfo camInfo = TW.Data.Get<CameraInfo>();
        private Entity camEntity = new Entity();

        private WorldSelector selector = new WorldSelector();

        private void initIslandSelector()
        {
            selector = new WorldSelector();
            selector.AddProvider(BoundingBoxSelectableProvider.Create(
                level.Islands.Where(i => i.Type == Island.IslandType.Normal),
                i => i.GetBoundingBox(),
                _ => {}
                ));
        }

        private void simulateFlyButton()
        {
            selector.UpdateTarget(camInfo.GetCenterScreenRay());
            selector.RenderSelection();

            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.F)) return;

            if (flyIsland != null)
                flyIsland = null;
            else
            {
                flyIsland = selector.GetTargeted() as Island;// level.Islands.Where(i => i.Type == Island.IslandType.Normal).Raycast(i => i.GetBoundingBox(), camInfo.GetCenterScreenRay());
            }




        }
      

        private ClusterFlightController clusterFlightController = new ClusterFlightController(TW.Graphics.Keyboard);

        private void simulateClusterFlight()
        {
            if (flyIsland == null) return;
            clusterFlightController.SimulateFlightStep(flyIsland);
        }


        private void simulateHotbar()
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