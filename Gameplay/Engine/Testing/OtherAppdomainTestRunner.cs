using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MHGameWork.TheWizards.TestRunner;

namespace MHGameWork.TheWizards.Engine.Testing
{
    /// <summary>
    /// Responsible for invoking another testrunner in another appdomain
    /// </summary>
    public class OtherAppdomainTestRunner :ITestRunner
    {
        private ITestRunner runner;

        public OtherAppdomainTestRunner(ITestRunner runner)
        {
            this.runner = runner;
            
        }


        public void Run(ITest test)
        {
            var appDomain = AppDomain.CreateDomain("TestRunnerNew");
            appDomain.Load(new AssemblyName(runner.GetType().Assembly.FullName)); //TODO: move this to the TestCommand
            //appDomain.Load(test.TestAssembly.FullName);

            var callback = TestCommand.Create(runner,test);

            appDomain.DoCallBack(callback.Run);


            try
            {
                AppDomain.Unload(appDomain);// this causes crashes in character controllers?

            }
            catch (Exception unloadEx)
            {
                Console.WriteLine(unloadEx);
            }


        }
    }
}
