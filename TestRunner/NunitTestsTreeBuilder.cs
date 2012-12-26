using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MHGameWork.TheWizards.TestRunner
{
    public class NunitTestsTreeBuilder
    {
        
        private readonly TestsTreeBuilder testsTreeBuilder;

        public NunitTestsTreeBuilder()
        {
            testsTreeBuilder = new TestsTreeBuilder();
        }

        public NunitTestsTreeBuilder(TestsTreeBuilder testsTreeBuilder)
        {
            this.testsTreeBuilder = testsTreeBuilder;
        }

        public TestNode CreateTestsTree(Assembly assembly)
        {
            List<Type> list = new List<Type>();
            foreach (Type t in assembly.GetExportedTypes())
            {
                if (t.GetCustomAttributes(typeof (NUnit.Framework.TestFixtureAttribute), false).Length > 0) list.Add(t);
            }
            var types = list.ToArray();


            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];


                var parentNamespace = testsTreeBuilder.GetOrCreateParentNode(type);
                var testClass = AddTestClass(parentNamespace, type);

                var methods =
                    type.GetMethods().Where(
                        m => m.GetCustomAttributes(typeof(NUnit.Framework.TestAttribute), false).Length > 0).ToArray();

                for (int j = 0; j < methods.Length; j++)
                {
                    var method = methods[j];
                    if (method.IsStatic) continue;
                    AddTestMethod(testClass,type, method);
                }

            }
            return testsTreeBuilder.RootNode;
        }


        public TestNode AddTestMethod(TestNode testNode,Type testClass, MethodInfo method)
        {

            var c = testNode.CreateChild(method.Name);
            c.Test = new NUnitTest( method,testClass);

            return c;
        }
        public TestNode AddTestClass(TestNode testNode, Type type)
        {
            var c = testNode.CreateChild(type.Name);
            //c.Test = new NUnitTest(null,type);

            return c;
        }
    }
}
