using System;
using System.IO;
using System.Reflection;
using System.Threading;
using MHGameWork.TheWizards.Scripting;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Core.Scripting
{
    [TestFixture]
    public class ScriptingTest
    {

        /// <summary>
        /// This test contains some parts of the code that is generated for the ScriptLoaderWrappers. With dottrace the IL can be read.
        /// The TestScriptWrapper is an example of manually generated code for a wrapper
        /// Use: ildasm /item:MHGameWork.TheWizards.Tests.Scripting.TestScriptWrapper $(TargetPath) /text
        /// </summary>
        [Test]
        public void TestScriptLoaderIL()
        {
            var f = new TestScriptWrapper();
            f.Script = new object();
        }

        [Test]
        public void TestScriptLoaderCreateWrapper()
        {
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("ScriptLoaderWrappers"), System.Reflection.Emit.AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule("Wrappers");
            var type = ScriptLoader.DefineWrapperType(module, typeof(ITestScriptInterface));
            var inst = (ScriptWrapper)Activator.CreateInstance(type);
            inst.Script = new TestScriptImplementation();
            var myScript = (ITestScriptInterface)inst;
            myScript.Execute();
        }

        [Test]
        public void TestScriptLoaderLoad()
        {
            var loader =new ScriptLoader();
            var script = loader.Load<ITestScriptInterface>(new FileInfo("../../Unit Tests/Scripting/TestScriptImplementation.cs"));
            script.Execute();

        }

        [Test]
        public void TestScriptLoaderReLoad()
        {
            var loader = new ScriptLoader();
            var script = loader.Load<ITestScriptInterface>(new FileInfo("../../Unit Tests/Scripting/TestScriptImplementation.cs"));
            script.Execute();

            while (true)
            {
                Thread.Sleep(1000);
                loader.AttempReload();
                script.Execute();
            }



        }

        public interface ITestScriptInterface
        {
            void Execute();

        }
    }
}
