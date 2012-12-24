using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.TestRunner;

namespace MHGameWork.TheWizards.Engine.Testing
{
    /// <summary>
    /// Responsible for running NUnit tests integrated with The Wizards Engine
    /// Incompatible tests run in a seperate appdomain, the engine gets hidden for the duration of the test
    /// Tests 
    /// </summary>
    public class EngineTestRunner
    {
        public void RunTest(TWEngine engine, NUnitTest test)
        {

            var other = new OtherAppdomainTestRunner(new NUnitTestRunner());

            var appDomain = AppDomain.CreateDomain("TestRunnerNew");
            //appDomain.Load(new AssemblyName(TestsAssembly.FullName));

           

            try
            {
                AppDomain.Unload(appDomain);// this causes crashes in character controllers?

            }
            catch (Exception unloadEx)
            {
                Console.WriteLine(unloadEx);
            }

            //var oldContext = EngineFactory.Instance;
            //EngineFactory.Instance = new TWEngineContext(engine);


            //var runner = new NUnitTestRunner();
            //runner.Run(test);

            ////var inst = Activator.CreateInstance(test.TestClass);
            ////test.TestMethod.Invoke(inst,null);

            //EngineFactory.Instance = oldContext;

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
