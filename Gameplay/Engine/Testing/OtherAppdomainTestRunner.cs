using System;
using System.Collections.Generic;
using System.Linq;
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
            throw new NotImplementedException();
        }


        /// <summary>
        /// TODO: this is a generic testrunner with callbacks!!
        /// </summary>
        private class CallbackObject
        {
            private string typeName;
            private string methodName;
            private string testRunnerType;

        }

    }
}
