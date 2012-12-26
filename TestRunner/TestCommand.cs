using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MHGameWork.TheWizards.TestRunner
{
    /// <summary>
    /// Responsible for representing a test run in a single object
    /// </summary>
    [Serializable]
    public class TestCommand
    {
        private byte[] runnerData;
        private byte[] testData;

        private TestCommand(byte[] runnerData, byte[] testData)
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

        public static TestCommand Create(ITestRunner runner, ITest test)
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


            return new TestCommand(runnerData, testData);
        }
        public static String ToString(TestCommand cmd)
        {
            var mem = new MemoryStream();
            var f = new BinaryFormatter();
            f.Serialize(mem,cmd);
            return Convert.ToBase64String(mem.ToArray());
        }
        public static TestCommand FromString(string str)
        {
            var mem = new MemoryStream(Convert.FromBase64String(str));
            var f = new BinaryFormatter();
            return (TestCommand)f.Deserialize(mem);
        }
    }
}