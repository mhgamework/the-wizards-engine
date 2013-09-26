using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.Engine.Debugging;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.Testing;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.TestRunner;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Responsible for setting up the engine. (Bootstrapper/ used in Plugin.cs)
    /// </summary>
    public class EngineInitializer
    {
        private EngineTestState engineTestState;

        public EngineInitializer(EngineTestState engineTestState)
        {
            this.engineTestState = engineTestState;
        }

        public void SetupEngine(TWEngine engine)
        {
            TW.Graphics.EscapeExists = false;
            TW.Graphics.AllowF3InputToggle = false;

            //checkLoadPreviousState();

            if (isTestSelected())
            {
                loadTest(engine);
            }
            else
            {
                loadEngine(engine);
            }
        }

        private void checkLoadPreviousState()
        {
            // TODO: this is a hack that is used to determine if this is the first time that the gameplaydll is loaded in the engine, since 
            //      the gameplay dll, by design cannot destinguish by itself whether this is the first run or not
            //      maybe a variable should be added that can be used to check if this is the initial run.
            //      Perhaps move this to the engine? or add a method to the plugin 'onEngineStartup'
            if (TW.Data.Objects.Count != 0) return; // Engine state not empty
            // ???
            //if (engineData.PreviousStateLoaded)
            //    return;

            DI.Get<Persistence.EngineStatePersistor>().LoadEngineState();

        }

        private void loadTest(TWEngine engine)
        {
            engine.AddSimulator(new EngineUISimulator());
            //engine.AddSimulator(new GraphVisualizerSimulator());
            try
            {
                var test = new NUnitTest(engineTestState.GetActiveTest(), engineTestState.GetActiveTest().ReflectedType);
                var runner = new EngineTestRunner();
                runner.RunTestInEngine(engine, test);
            }
            catch (Exception ex)
            {
                //TODO: this does not work since the error is caught in the NUnit Test Runner!!
                DI.Get<IErrorLogger>().Log(ex, "When starting test (EngineInitializer)");
            }

            loadBare(engine);

            //engine.AddSimulator(DI.Get<ModelObjectGraphSimulator>());

        }

        private bool isTestSelected()
        {
            return engineTestState.GetActiveTest() != null;
        }

        private void loadBare(TWEngine engine)
        {
            engine.AddSimulator(new TestUISimulator());
            engine.AddSimulator(new UISimulator());
        }
        private void loadEngine(TWEngine engine)
        {
            engine.AddSimulator(new EngineUISimulator());
            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new FirstPersonCameraSimulator());
            engine.AddSimulator(new FlashlightSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new TestUISimulator());
            engine.AddSimulator(new UISimulator());
        }
    }
}
