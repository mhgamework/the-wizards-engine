﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MHGameWork.TheWizards.Engine.Tests.PhysX;
using MHGameWork.TheWizards.Reflection;
using MHGameWork.TheWizards.TestRunner;

namespace MHGameWork.TheWizards.Engine.Testing
{
    public class EngineTestPicker
    {
        private TestNode rootNode;

        private EngineTestState testState;

        public EngineTestPicker(EngineTestState testState)
        {
            this.testState = testState;
        }

        public bool PickCompleted
        {
            get { return pickCompleted; }
            private set { pickCompleted = value; }
        }


        private TestNode createTree()
        {
            var builder = new TestsTreeBuilder();

            //createSimulatorsTree(builder);

            var nunit = new NunitTestsTreeBuilder(builder);
            nunit.CreateTestsTree(TW.Data.GameplayAssembly);

            var unittests = Assembly.LoadFrom("Unit Tests.dll");
            nunit.CreateTestsTree(unittests);

            return builder.RootNode;

        }


        public void ShowTestPicker()
        {
            var ui = new TestSelectionInterface(createTree());
            ui.Data = new SaveData();
            if (testState.GetActiveTest() != null)
                ui.SelectNodeByPath(testState.GetActiveTest().ReflectedType.FullName + "." + testState.GetActiveTest().Name);
            NUnitTest ret = null;
            ui.RunTestMethod += delegate(TestNode node)
            {

                ret = (NUnitTest)node.Test;
                if (ret.TestMethod == null)
                {
                    ret = null;
                    return;
                }
                pickedTest = ret;
                pickCompleted = true;

                ui.Hide();
            };

            ui.Show();

        }

        private volatile NUnitTest pickedTest;
        private volatile bool pickCompleted;



        public NUnitTest GetPickedTest()
        {
            return pickedTest;
        }
    }
}
