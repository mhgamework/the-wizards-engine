using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Engine.Tests.PhysX;
using MHGameWork.TheWizards.Reflection;
using MHGameWork.TheWizards.TestRunner;

namespace MHGameWork.TheWizards.Engine.Testing
{
    public class EngineTestPicker
    {
        private TestNode rootNode;


        public NUnitTest SelectTest()
        {
            var testData = TW.Data.GetSingleton<TestingData>();
            var ui = new TestSelectionInterface(createTree());
            ui.Data = new SaveData();
            ui.SelectNodeByPath(testData.ActiveTestClass + "." + testData.ActiveTestMethod);
            NUnitTest ret = null;
            ui.RunTestMethod +=   delegate(TestNode node)
                {
                    
                    ret = (NUnitTest)node.Test;
                    if (ret.TestMethod == null)
                    {
                        ret = null;
                        return;
                    }
                    ui.Hide();
                };

            

            ui.ShowDialog();

            return ret;
        }

        private TestNode createTree()
        {
            var builder = new TestsTreeBuilder();

            //createSimulatorsTree(builder);

            var nunit = new NunitTestsTreeBuilder(builder);
            nunit.CreateTestsTree(TW.Data.GameplayAssembly);

            var unittests = Assembly.LoadFrom( "Unit Tests.dll");
            nunit.CreateTestsTree(unittests);

            return builder.RootNode;

        }
        private TestNode createSimulatorsTree(TestsTreeBuilder builder)
        {
            rootNode = new TestNode();
            rootNode.Text = "_ROOT_";

            var types = TW.Data.GameplayAssembly.GetExportedTypes()
                .Where(t => typeof(ITestSimulator).IsAssignableFrom(t)).ToArray();


            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];


                var parentNamespace = builder.GetOrCreateParentNode(type);
                var testClass = parentNamespace.CreateChild(type.Name);

                testClass.Test = new EngineTest{ SimulatorType =  type};

            }
            return rootNode;

        }

    }
}
