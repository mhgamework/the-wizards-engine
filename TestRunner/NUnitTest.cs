using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace MHGameWork.TheWizards.TestRunner
{
    [Serializable]
    public class NUnitTest : ITest,ISerializable
    {
        [XmlIgnore]
        public MethodInfo TestMethod;
        [XmlIgnore]
        public Type TestClass;

        public NUnitTest(MethodInfo testMethod, Type testClass)
        {
            TestMethod = testMethod;
            TestClass = testClass;
        }

        protected NUnitTest(SerializationInfo info, StreamingContext context)
        {
            var assemblyName = info.GetString("assembly");
            var className = info.GetString("class");
            var methodName = info.GetString("method");

            var assembly = Assembly.LoadFile(assemblyName);

            TestClass = assembly.GetTypes().First(t => t.FullName == className);
            TestMethod = TestClass.GetMethod(methodName);

        }

        public Assembly TestAssembly
        {
            get { return TestClass.Assembly; }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("assembly", TestClass.Assembly.Location);
            info.AddValue("class", TestClass.FullName);
            info.AddValue("method", TestMethod.Name);
        }
    }
}
