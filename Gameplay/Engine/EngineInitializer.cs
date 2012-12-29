using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.Testing;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Responsible for setting up the engine. (Bootstrapper/ used in Plugin.cs)
    /// </summary>
    public class EngineInitializer
    {
        private TestingData testingData;
        private EngineData engineData;

        public EngineInitializer()
        {
            testingData = TW.Data.GetSingleton<TestingData>();
            engineData = TW.Data.GetSingleton<EngineData>();
        }

        public void SetupEngine(TWEngine engine)
        {
            TW.Graphics.EscapeExists = false;

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
            if (engineData.PreviousStateLoaded)
                return;

            DI.Get<Persistence.EngineStatePersistor>().LoadEngineState();

            engineData.PreviousStateLoaded = true;
        }

        private void loadTest(TWEngine engine)
        {
            engine.AddSimulator(new EngineUISimulator());
            try
            {
                var runner = new EngineTestRunner();
                runner.RunTestDataTest(engine);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            loadBare(engine);
        }

        private bool isTestSelected()
        {
            return testingData.ActiveTestClass != null;
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
