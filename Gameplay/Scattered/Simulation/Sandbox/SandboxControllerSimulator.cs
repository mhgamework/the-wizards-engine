using System;
using System.Collections.Generic;
using System.IO;
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
        private RoundSimulator roundSimulator;
        private ISandboxControllerState state;
        private Dictionary<Key, ISandboxControllerState> keyMap = new Dictionary<Key, ISandboxControllerState>();
        

        public SandboxControllerSimulator(Level level, EditorConfiguration configuration, RoundSimulator roundSimulator)
        {
            this.level = level;
            this.roundSimulator = roundSimulator;
            keyMap.Add(Key.D1, new IslandPlacerState(level, configuration));

            keyMap.Add(Key.D2, new ConstructionPlacerState(level, configuration, level.createEmptyConstruction));
            keyMap.Add(Key.D3, new ConstructionPlacerState(level, configuration, level.createCrysalCliffsConstruction));
            keyMap.Add(Key.D4, new ConstructionPlacerState(level, configuration, level.createWarehouseConstruction));

            keyMap.Add(Key.D5, new BridgeBuilderState(level, configuration));

            keyMap.Add(Key.D6, new ConstructionPlacerState(level, configuration, level.createScrapStationConstruction));
            keyMap.Add(Key.D7, new ConstructionPlacerState(level, configuration, level.createEnergyNodeConstruction));

            



        }

      

        public void Simulate()
        {
            simulateState();

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.U)) roundSimulator.CombatPhase = !roundSimulator.CombatPhase;
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.O)) saveLevel();
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.I)) loadLevel();
        }

        private void saveLevel()
        {
            var s = new LevelSerializer();
            s.Serialize(level,new FileInfo(TWDir.GameData.CreateSubdirectory("Scattered") + "\\Level.txt"));
        }
        private void loadLevel()
        {
            var s = new LevelSerializer();
            s.Deserialize(level, new FileInfo(TWDir.GameData.CreateSubdirectory("Scattered") + "\\Level.txt"));
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