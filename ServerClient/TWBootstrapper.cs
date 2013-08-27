using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.TestRunner;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// Responsible for running the correct startupcode using the TWConfig file
    /// </summary>
    public class TWBootstrapper
    {
        private TWConfig config;

        public void Run()
        {
            try
            {
                bootstrap();
            }
            catch (BootStrapException)
            {
                Console.WriteLine("Can't start, shutting down!");
            }
        }

        private void bootstrap()
        {
            config = TWConfig.Load();
            config.Save();
            if (config.RunTestUI)
            {
                runWinformsTestrunner();
                return;
            }
            if (config.TestMode)
            {
                var obj = createTestClass(config.TestClass);
                runTestMethod(obj, config.TestMethod);
                return;
            }
            // Run engine!
            var engine = new TWEngine();
            engine.Run();
        }

        private void runWinformsTestrunner()
        {
            var treeBuilder = new TestsTreeBuilder();
            var builder = new NunitTestsTreeBuilder(treeBuilder);
            builder.CreateTestsTree(Assembly.LoadFrom("Unit Tests.dll"));
            try
            {
                builder.CreateTestsTree(Assembly.LoadFrom("../../Gameplay/bin/x86/Debug/Gameplay.dll"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            TestRunnerGUI runnerGui = new TestRunnerGUI(treeBuilder.RootNode);
            //runner.RunTestNewProcessPath = "\"" + Assembly.GetExecutingAssembly().Location + "\"" + " -test {0}";

            runnerGui.Run();
        }

        private void runTestMethod(object testObject, string methodName)
        {
            try
            {
                testObject.GetType().GetMethod(methodName).Invoke(testObject, null);
            }
            catch (Exception)
            {
                Console.WriteLine("Can't find testmethod {0}", methodName);
                throw new BootStrapException();
            }
        }

        private object createTestClass(string className)
        {
            object testclass;
            try
            {
                var assembly = Assembly.LoadFrom("Unit Tests.dll");
                var type = assembly.GetType(className);
                return Activator.CreateInstance(type);


            }
            catch (Exception)
            {
                Console.WriteLine("Can't load testclass '{0}'", className);
                throw new BootStrapException();
            }
        }


        private class BootStrapException : Exception
        {

        }
    }
}
