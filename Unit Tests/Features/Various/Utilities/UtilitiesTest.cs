using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using MHGameWork.TheWizards.TestRunner;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Various.Utilities
{
    [TestFixture]
    public class UtilitiesTest
    {
        /// <summary>
        /// TODO: move test
        /// </summary>
        [Test]
        public void TestTestRunner()
        {
            throw new NotImplementedException(); // This causes problems with automated testing
            TestRunnerGUI runnerGui = new TestRunnerGUI();
            runnerGui.TestsAssembly = Assembly.GetExecutingAssembly();
            runnerGui.Run();
        }

        [Test]
        public void TestNonImplementedException()
        {
            throw new NotImplementedException();
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "This test has succeeded!!!")]
        public void TestException()
        {

            throw new Exception("This test has succeeded!!!");
        }

        [Test]
        public void TestTestRunnerExeNewProcess()
        {
            Process p = new Process();
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.ErrorDialog = false;

            /* StringBuilder sb = new StringBuilder("/COVERAGE ");
             sb.Append("TestRunner.exe");*/
            //p.StartInfo.FileName = "vsinstr.exe";
            p.StartInfo.FileName = "TestRunner.exe";
            p.StartInfo.Arguments = "-test Exception";
            p.Start();

            string stdoutx = p.StandardOutput.ReadToEnd();
            string stderrx = p.StandardError.ReadToEnd();
            p.WaitForExit();

            Console.WriteLine("Exit code : {0}", p.ExitCode);
            Console.WriteLine("Stdout : {0}", stdoutx);
            Console.WriteLine("Stderr : {0}", stderrx);

        }

        [Test]
        public void TestSimpleTest()
        {
            MessageBox.Show("Test succeeded!");
        }

        [Test]
        public void TestRunTestByName()
        {
            TestRunnerGUI runnerGui = new TestRunnerGUI();
            runnerGui.TestsAssembly = Assembly.GetExecutingAssembly();
            runnerGui.RunTestByName("MHGameWork.TheWizards.Tests.Utilities.UtilitiesTest.TestSimpleTest");

        }

        /// <summary>
        /// Note: this test only works when the current executing process has set the RunTestNewProcessPath correctly
        /// Example: runner.RunTestNewProcessPath = Assembly.GetExecutingAssembly().Location + " \"-test {0}\"";
        /// </summary>
        [Test]
        public void TestRunNestedTestNewProcess()
        {
            TestRunnerGUI.RunTestInOtherProcess(typeof(UtilitiesTest).Assembly, "MHGameWork.TheWizards.Tests.Utilities.UtilitiesTest", "TestSimpleTest");

        }
    }
}
