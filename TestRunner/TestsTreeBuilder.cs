using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MHGameWork.TheWizards.TestRunner
{
    public class TestsTreeBuilder
    {
        private TestNode rootNode;
        public TestNode CreateTestsTree(Assembly assembly)
        {

            rootNode = new TestNode();
            rootNode.Text = "_ROOT_";

            var types = assembly.GetExportedTypes()
                .Where(t => t.GetCustomAttributes(typeof(NUnit.Framework.TestFixtureAttribute), false).Length > 0).ToArray();


            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];


                var parentNamespace = getOrCreateParentNode(type);
                var testClass = parentNamespace.AddTestClass(type);

                var methods =
                    type.GetMethods().Where(
                        m => m.GetCustomAttributes(typeof(NUnit.Framework.TestAttribute), false).Length > 0).ToArray();

                for (int j = 0; j < methods.Length; j++)
                {
                    var method = methods[j];
                    if (method.IsStatic) continue;
                    testClass.AddTestMethod(method);
                }

            }
            return rootNode;
        }



        private TestNode getOrCreateParentNode(Type type)
        {
            var parts = type.FullName.Split('.');

            var parent = rootNode;

            for (int i = 0; i < parts.Length - 1; i++) // Note the -1 here
            {
                parent = parent.FindOrCreateChild(parts[i]);
            }

            return parent;
        }
    }
}
