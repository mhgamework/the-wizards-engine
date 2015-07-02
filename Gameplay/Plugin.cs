using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using MHGameWork.TheWizards.Animation;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Debugging;
using MHGameWork.TheWizards.Engine.Files;
using MHGameWork.TheWizards.Engine.Persistence;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.Testing;
using MHGameWork.TheWizards.Engine.Tests.PhysX;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Persistence;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Serialization;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.TestRunner;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards
{
    public class Plugin : IGameplayPlugin
    {
        public void Initialize(TWEngine engine)
        {
            //testPickupSimulator(engine);
            //testTriggerSimulator(engine);
            //testWayPointTrigger(engine);
            //testWorldmatrixAnimation(engine);
            //testLoadLevel(engine);
            //testLevelBuilding(engine);

            //testAddStupidRedHelperMesh(engine);
            //testFirstPersonCamera(engine);

            TW.Graphics.MouseInputDisabled = true;
            TW.Graphics.Mouse.CursorEnabled = true;

            var fs = new EngineFileSystem(TWDir.Cache.FullName + "\\EngineFS");
            DI.Set<IEngineFilesystem>(fs);
            DI.Set<EngineTestState>(new EngineTestState(fs));
            DI.Set<TestSceneBuilder>(new TestSceneBuilder(DI.Get<EngineTestState>()));
            DI.Set<UISimulator>(new UISimulator());

            cleanData();
            var initializer = new EngineInitializer(DI.Get<EngineTestState>());

            initializer.SetupEngine(engine);
        }

        private void cleanData()
        {
            // Clear all objects
            TW.Data.Objects.Clear();
            TW.Debug.NeedsReload = true;


        }

    }




}
