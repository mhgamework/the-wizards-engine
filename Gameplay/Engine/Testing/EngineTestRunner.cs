using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Reflection;
using MHGameWork.TheWizards.TestRunner;

namespace MHGameWork.TheWizards.Engine.Testing
{
    /// <summary>
    /// Responsible for running NUnit tests integrated with The Wizards Engine
    /// Incompatible tests run in a seperate process, the engine gets hidden for the duration of the test
    /// Tests 
    /// </summary>
    public class EngineTestRunner
    {
        private EngineTestState testState;

        public EngineTestRunner(EngineTestState testState)
        {
            this.testState = testState;
        }


        public void RunTest( NUnitTest test)
        {
            // Hack due to some wierd hotloading bug (attribute types are not from same loaded assembly as executing assembly)
            // Maybe fixed, doesn't matter
            var count = test.TestClass.GetCustomAttributes(true).Count(t => t.GetType().FullName == typeof (EngineTestAttribute).FullName);
            if (count > 0) 
            {
                testState.SetActiveTest(test.TestMethod);
                TW.Debug.NeedsReload = true;
            }
            else
            {
                var other = new OtherProcessTestRunner(new NUnitTestRunner());
                other.Run(test);
            }
        }

        public void RunTestDataTest(TWEngine engine)
        {
            var method = testState.GetActiveTest();

            RunTestInEngine(engine,new NUnitTest(method,method.ReflectedType));

        }
        //TODO: revise the use of persistancescope here
        [PersistanceScope] // Data created when starting a test should persist!
        public void RunTestInEngine(TWEngine engine,NUnitTest test)
        {
            var oldContext = EngineFactory.Instance;
            EngineFactory.Instance = new TWEngineContext(engine);

            //TW.Graphics.Form.Hide();

            var runner = new NUnitTestRunner();
            //TODO: runner.Timeout = 10000;
            runner.Run(test);

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
