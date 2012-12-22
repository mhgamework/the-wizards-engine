using System;
using System.Reflection;

namespace MHGameWork.TheWizards.TestRunner
{
    public interface ITestRunner
    {
        /// <summary>
        /// This simply runs the correct pieces of code for given test method.
        /// </summary>
        /// <param name="test"></param>
        /// <param name="method"></param>
        void RunNormal(object test, MethodInfo method);

        /// <summary>
        /// This method runs given test method and stores returns the result of the test
        /// </summary>
        /// <param name="test"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        Exception RunAutomated(object test, MethodInfo method);
    }
}