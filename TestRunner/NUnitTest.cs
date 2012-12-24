using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace MHGameWork.TheWizards.TestRunner
{
    public class NUnitTest : ITest
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
    }
}
