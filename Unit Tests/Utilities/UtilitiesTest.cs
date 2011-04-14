using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using MHGameWork.TheWizards.Utilities;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Utilities
{
    [TestFixture]
    public class UtilitiesTest
    {
        [Test]
        public void TestTestRunner()
        {
            TestRunner runner = new TestRunner();
            runner.TestsAssembly = Assembly.GetExecutingAssembly();
            runner.Run();
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
            TestRunner runner = new TestRunner();
            runner.TestsAssembly = Assembly.GetExecutingAssembly();
            runner.RunTestByName("MHGameWork.TheWizards.Tests.Utilities.UtilitiesTest.TestSimpleTest");

        }

        /// <summary>
        /// Note: this test only works when the current executing process has set the RunTestNewProcessPath correctly
        /// Example: runner.RunTestNewProcessPath = Assembly.GetExecutingAssembly().Location + " \"-test {0}\"";
        /// </summary>
        [Test]
        public void TestRunNestedTestNewProcess()
        {
            var p = TestRunner.RunTestInOtherProcess("MHGameWork.TheWizards.Tests.Utilities.UtilitiesTest.TestSimpleTest");

        }
    }
}
