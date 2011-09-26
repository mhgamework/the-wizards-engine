using System;
using MHGameWork.TheWizards.Scripting;

namespace MHGameWork.TheWizards.Tests.Scripting
{
    /// <summary>
    /// MHGameWork.TheWizards.Tests.Scripting.TestScriptWrapper
    /// </summary>
    public class TestScriptWrapper : ScriptWrapper, ScriptingTest.ITestScriptInterface
    {
        private ScriptingTest.ITestScriptInterface fieldName;


        public void Execute()
        {
            fieldName.Execute();
        }

        protected override void setScript(object value)
        {
            fieldName = (ScriptingTest.ITestScriptInterface)value;
        }

        protected override object getScript()
        {
            return fieldName;
        }
    }
}