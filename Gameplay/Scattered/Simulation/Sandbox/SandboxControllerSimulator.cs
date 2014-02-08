using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Selecting;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.Simulation.Constructions;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Scattered.Simulation.Sandbox
{
    /// <summary>
    /// Translates user input so that the user can control the sandbox
    /// </summary>
    public class SandboxControllerSimulator : ISimulator
    {
        private readonly Level level;
        private readonly DistributionHelper distributionHelper;
        private ISandboxControllerState state;
        private Dictionary<Key, ISandboxControllerState> keyMap = new Dictionary<Key, ISandboxControllerState>();
        private RoundSimulator roundSimulator;

        public SandboxControllerSimulator(Level level, EditorConfiguration configuration, DistributionHelper distributionHelper)
        {
            this.level = level;
            this.distributionHelper = distributionHelper;
            keyMap.Add(Key.D1, new IslandPlacerState(level, configuration));

            keyMap.Add(Key.D2, new ConstructionPlacerState(level, configuration, createEmptyConstruction));
            keyMap.Add(Key.D3, new ConstructionPlacerState(level, configuration, createCrysalCliffsConstruction));
            keyMap.Add(Key.D4, new ConstructionPlacerState(level, configuration, createWarehouseConstruction));

            keyMap.Add(Key.D5, new BridgeBuilderState(level, configuration));
            
            keyMap.Add(Key.D6, new ConstructionPlacerState(level, configuration, createScrapStationConstruction));
            keyMap.Add(Key.D7, new ConstructionPlacerState(level, configuration, createEnergyNodeConstruction));

            


            roundSimulator = new RoundSimulator();

        }

        private Construction createEmptyConstruction(Island arg)
        {
            return new Construction()
            {
                Name = "Empty",
                UpdateAction = new NullConstructionAction()
            };
        }
        private Construction createWarehouseConstruction(Island arg)
        {
            return new Construction()
            {
                Name = "Warehouse",
                UpdateAction = new NullConstructionAction()
            };
        }
        private Construction createCrysalCliffsConstruction(Island arg)
        {
            return new Construction()
            {
                Name = "Crystal Cliffs",
                UpdateAction = new CrystalCliffsAction(arg, distributionHelper, roundSimulator)
            };
        }
        private Construction createEnergyNodeConstruction(Island arg)
        {
            return new Construction()
            {
                Name = "Energy Node",
                UpdateAction = new EnergyNodeAction(arg, distributionHelper, roundSimulator)
            };
        }
        private Construction createScrapStationConstruction(Island arg)
        {
            return new Construction()
            {
                Name = "Scrap Station",
                UpdateAction = new ScrapStationAction(arg, distributionHelper)
            };
        }

        public void Simulate()
        {
            simulateState();

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.U)) roundSimulator.CombatPhase = !roundSimulator.CombatPhase;
        }

        private void simulateState()
        {
            ISandboxControllerState newState = null;

            foreach (var el in keyMap)
            {
                if (!TW.Graphics.Keyboard.IsKeyPressed(el.Key)) continue;
                newState = el.Value;
            }

            if (newState != null)
            {
                if (state != null) state.Exit();
                state = newState;
                newState.Init();
            }

            if (state == null) return;
            state.Update();
        }


        public static BoundingBoxSelectableProvider CreateIslandSelectableProvider(Level level, Action<Island> onClickIsland)
        {
            return BoundingBoxSelectableProvider.Create(level.Islands, i => i.GetBoundingBox(), onClickIsland);

        }


    }
}