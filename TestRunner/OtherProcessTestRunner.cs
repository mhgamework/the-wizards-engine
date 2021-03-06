﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace MHGameWork.TheWizards.TestRunner
{
    public class OtherProcessTestRunner : ITestRunner
    {
        private readonly ITestRunner otherRunner;

        public OtherProcessTestRunner(ITestRunner otherRunner)
        {
            this.otherRunner = otherRunner;
        }

        public TestResult RunTestInOtherProcess(TestCommand command)
        {

            // Run in new process

            var info = new ProcessStartInfo( Assembly.GetExecutingAssembly().Location, "-s " + TestCommand.ToString(command));
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;

            var p = Process.Start(info);
            p.WaitForExit(); // 7000
            if (!p.HasExited)
            {
                p.Kill();
                return new TestResult { Success = false, ErrorMessage = "Test was aborted for taking to long!" };
            }
            var s = new XmlSerializer(typeof(TestResult));
            string xml = null;

            while (!p.StandardOutput.EndOfStream)
            {
                var line = p.StandardOutput.ReadLine();
                if (line == ">>>Start Result")
                    break;

            }
            while (!p.StandardOutput.EndOfStream)
            {
                var line = p.StandardOutput.ReadLine();

                if (line == "<<<End Result")
                    break;

                xml += line;
            }

            //if (xml == null) throw new InvalidOperationException();
            return null;
            //return (TestResult)s.Deserialize(new StringReader(xml));
        }

        public void Run(ITest test)
        {
            var cmd = TestCommand.Create(otherRunner, test);

            RunTestInOtherProcess(cmd);
        }
    }
}
