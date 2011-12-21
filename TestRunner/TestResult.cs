using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.TestRunner
{
    [Serializable]
    public class TestResult
    {
        public static TestResult FromException(Exception result)
        {
            var testResult = new TestResult();
            testResult.Success = true;
            if (result != null)
            {
                testResult.Success = false;
                testResult.ErrorType = result.GetType().FullName;
                testResult.ErrorMessage = result.Message;
                testResult.StackTrace = result.StackTrace;
            }
            return testResult;
        }

        public bool Success;
        public string ErrorType;
        public string ErrorMessage;
        public string StackTrace;
    }
}
