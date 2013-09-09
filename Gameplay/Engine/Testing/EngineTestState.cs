using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.Engine.Files;

namespace MHGameWork.TheWizards.Engine.Testing
{
    /// <summary>
    /// Responsible for storing the current engine test
    /// Responsible for persisting it on disk
    /// </summary>
    public class EngineTestState
    {
        private IEngineFilesystem filesystem;

        private MethodInfo activeTest;

        public EngineTestState(IEngineFilesystem filesystem)
        {
            this.filesystem = filesystem;
            load();
        }

        public void SetActiveTest(MethodInfo testMethod)
        {
            activeTest = testMethod;
            store();
        }
        public MethodInfo GetActiveTest()
        {
            return activeTest;
        }

        /// <summary>
        /// Returns a unique name for the test
        /// </summary>
        /// <returns></returns>
        public string GetTestName()
        {
            return activeTest.ReflectedType.FullName + "." + activeTest.Name;
        }

        private void store()
        {
            var data = new Data();
            if (activeTest != null)
            {
                data.ActiveTestClass = TW.Data.TypeSerializer.Serialize(activeTest.ReflectedType);
                data.ActiveTestMethod = activeTest.Name;
            }
            filesystem.StoreXml(data, getFileName());
        }
        private void load()
        {
            Data data = null;
            activeTest = null;

            try
            {
                data = filesystem.LoadXml<Data>(getFileName());
                if (data == null) return;
                if (data.ActiveTestMethod == null) return;
                var type = TW.Data.TypeSerializer.Deserialize(data.ActiveTestClass);
                activeTest = type.GetMethod(data.ActiveTestMethod);
            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex,"Load test state");
            }
           
        }

        private static string getFileName()
        {
            return "Engine\\TestState";
        }

        public class Data
        {
            public String ActiveTestClass { get; set; }
            public string ActiveTestMethod { get; set; }
            //public byte[] SerializedTest { get; set; }

        }
    }
}
