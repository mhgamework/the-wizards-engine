﻿using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
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
        private ISandboxControllerState state;
        private Dictionary<Key, ISandboxControllerState> keyMap = new Dictionary<Key, ISandboxControllerState>();
        public SandboxControllerSimulator(Level level, EditorConfiguration configuration)
        {

            keyMap.Add(Key.D1, new IslandPlacerState(level, configuration));

            keyMap.Add(Key.D2, new ConstructionPlacerState(level, configuration, createEmptyConstruction));
            keyMap.Add(Key.D3, new ConstructionPlacerState(level, configuration, createCrysalCliffsConstruction));
            keyMap.Add(Key.D4, new ConstructionPlacerState(level, configuration, createWarehouseConstruction));

            keyMap.Add(Key.D5, new BridgeBuilderState(level,configuration));

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
                UpdateAction = new CrystalCliffsAction(arg)
            };
        }


        public void Simulate()
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