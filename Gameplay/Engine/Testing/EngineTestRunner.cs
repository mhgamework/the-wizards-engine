using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.TestRunner;

namespace MHGameWork.TheWizards.Engine.Testing
{
    public class EngineTestRunner
    {
        //TODO: revise the use of persistancescope here
        [PersistanceScope] // Data created when starting a test should persist!
        public void RunTestInEngine(TWEngine engine, NUnitTest test)
        {
            var oldContext = EngineFactory.Instance;
            EngineFactory.Instance = new TWEngineContext(engine);

            //TW.Graphics.Form.Hide();

            var runner = new NUnitTestRunner();
            //TODO: runner.Timeout = 10000;
            runner.RunInThisThread(test);

            DI.Get<TestSceneBuilder>().EnsureTestSceneLoaded();

            //TW.Graphics.Form.Show();

            EngineFactory.Instance = oldContext;
        }


        private class TWEngineContext : EngineFactory.EngineFactoryContext
        {
            private TWEngine engine;

            public TWEngineContext(TWEngine engine)
            {
                this.engine = engine;
            }

            public override TWEngine CreateEngine()
            {
                return engine;
            }
        }
    }
}