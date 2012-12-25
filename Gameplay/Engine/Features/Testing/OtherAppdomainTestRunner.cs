using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using MHGameWork.TheWizards.TestRunner;

namespace MHGameWork.TheWizards.Engine.Testing
{
    public class OtherAppdomainTestRunner :ITestRunner
    {
        private ITestRunner runner;

        public OtherAppdomainTestRunner(ITestRunner runner)
        {
            this.runner = runner;
            
        }


        public void Run(ITest test)
        {
            createCallback(test);


            var appDomain = AppDomain.CreateDomain("TestRunnerNew");
            appDomain.Load(new AssemblyName(runner.GetType().Assembly.FullName));
            appDomain.Load(test.TestAssembly.FullName);

            var callback = createCallback(test);

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

        private CallbackObject createCallback(ITest test)
        {
            var memStream = new MemoryStream();
            var f = new BinaryFormatter();
            f.Serialize(memStream, runner);
            memStream.Flush();
            var runnerData = memStream.ToArray();


            memStream = new MemoryStream();
            f = new BinaryFormatter();
            f.Serialize(memStream, test);
            memStream.Flush();
            var testData = memStream.ToArray();


            return new CallbackObject(runnerData, testData);
        }


        /// <summary>
        /// TODO: this is a generic testrunner with callbacks!!
        /// </summary>
        [Serializable]
        private class CallbackObject
        {
            private byte[] runnerData;
            private byte[] testData;

            public CallbackObject(byte[] runnerData, byte[] testData)
            {
                this.runnerData = runnerData;
                this.testData = testData;
            }

            public void Run()
            {
                MemoryStream mem;
                var f = new BinaryFormatter();
                mem = new MemoryStream(runnerData);
                var runner =(ITestRunner) f.Deserialize(mem);
                mem = new MemoryStream(testData);
                var test = (ITest) f.Deserialize(mem);

                runner.Run(test);

            }
        }

    }
}
