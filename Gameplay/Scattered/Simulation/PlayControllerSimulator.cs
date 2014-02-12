using System;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.Simulation.Constructions;
using MHGameWork.TheWizards.Scattered.Simulation.Sandbox;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.HotbarCore;
using MHGameWork.TheWizards.SkyMerchant._Engine;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using System.Linq;
using SlimDX;
using SlimDX.DirectInput;

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
            bar.SetHotbarItem(1, new ControllerStateItem("Warehouse", new ConstructionPlacerState(level, configuration, level.createWarehouseConstruction)));
            bar.SetHotbarItem(2, new ControllerStateItem("Crystal Cliffs", new ConstructionPlacerState(level, configuration, level.createCrysalCliffsConstruction)));
            bar.SetHotbarItem(3, new ControllerStateItem("Camp", new ConstructionPlacerState(level, configuration, level.createCampConstruction)));
            bar.SetHotbarItem(4, new ControllerStateItem("Workshop", new ConstructionPlacerState(level, configuration, level.createWorkshop)));
            bar.SetHotbarItem(5, new ControllerStateItem("Scrap station", new ConstructionPlacerState(level, configuration, level.createScrapStationConstruction)));
            bar.SetHotbarItem(6, new ControllerStateItem("Demolish!", new ConstructionPlacerState(level, configuration, level.createEmptyConstruction)));

            buildPhaseStart = TW.Graphics.TotalRunTime;
            createDebugtext();
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
            roundState.ExecuteDuringBuildPhase(() =>
                {
                    if (buildPhaseStart + buildModeDuration < TW.Graphics.TotalRunTime)
                    {
                        hotbarController.Bar.GetHotbarItem(hotbarController.Bar.SelectedSlot).OnDeselected();
                        roundState.CombatPhase = true;

                        buildPhaseStart = TW.Graphics.TotalRunTime;
                        return;
                    }

                    hotbarController.Update();
                    hotbarController.UpdateSelectedItem();
                });

            roundState.ExecuteDuringCombatPhase(() =>
                {
                    if (buildPhaseStart + 3 > TW.Graphics.TotalRunTime) return;
                    if (level.Travellers.Any(t => t.Type.IsEnemy)) return;

                    roundState.CombatPhase = false;
                    buildPhaseStart = TW.Graphics.TotalRunTime;

                    hotbarController.Bar.GetHotbarItem(hotbarController.Bar.SelectedSlot).OnSelected();
                });

            updateDebugText();

            if (TW.Graphics.Keyboard.IsKeyDown(Key.N)) movementSimulator.MovementSpeed = 100;
            else movementSimulator.MovementSpeed = 10;
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