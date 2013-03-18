using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MHGameWork.TheWizards.TestRunner
{
    public class TestRunnerGUI
    {

        // These tests are accessible in the Tests themselves (while running them)

        private static bool isRunning;
        public static bool IsRunning
        {
            get { lock (scopeLock) return isRunning; }
            private set { lock (scopeLock) isRunning = value; }
        }

        private static bool isRunningAutomated;
        public static bool IsRunningAutomated
        {
            get { lock (scopeLock)return isRunningAutomated; }
            private set { lock (scopeLock) isRunningAutomated = value; }
        }

        private static object scopeLock = new object();
        //TODO
        //public bool AutoStartMode { get; set; }



        private TestNode rootNode;
        SaveData data;


        public TestRunnerGUI(Assembly TestsAssembly)
        {
            var builder = new NunitTestsTreeBuilder();

            rootNode = builder.CreateTestsTree(TestsAssembly);

            TestRunner = createTestRunner();

        }
        public TestRunnerGUI(TestNode rootNode)
        {
            this.rootNode = rootNode;
            TestRunner = createTestRunner();
        }

        private static ConsoleOutputTestRunner createTestRunner()
        {
            return new ConsoleOutputTestRunner(new NUnitTestRunner());
        }


        public void Run()
        {


            rootNode.SortRecursive();

            var sel = new TestSelectionInterface(rootNode);
            sel.ResumeTest = delegate
                             {
                                 sel.Hide();
                                 resumeTestRun();
                                 sel.Show();
                             };
            sel.RunTestGroup = delegate(TestNode obj)
                               {
                                   data.SelectedPath = sel.SelectedNodePath;
                                   sel.Hide();
                                   doTestRun(obj);
                                   sel.Show();
                               };
            sel.RunTestMethod = delegate(TestNode obj)
                                {
                                    data.SelectedPath = sel.SelectedNodePath;
                                    sel.Hide();
                                    doTestRun(obj);
                                    sel.Show();
                                };



            // The constructor of TestSelectionInterface needs to be called first because 
            //   it links the treeview to the testnodes :s
            loadState(getSaveFilePath());
            if (data == null)
                data = new SaveData();

            if (data.RunningTestsNodePath != null && data.RunAutomated)
                resumeTestRun();



            sel.Data = data;
            sel.SelectNodeByPath(data.SelectedPath);

            sel.ShowSelectTestDialog();

            data.SelectedPath = sel.SelectedNodePath;

            //data = new SaveData();
            data.Rootnode = rootNode;


            data.Save(getSaveFilePath());
        }

        public void RunTestByName(string name)
        {
            throw new NotImplementedException();
            var builder = new NunitTestsTreeBuilder();
            //rootNode = builder.CreateTestsTree(TestsAssembly);
            var node = rootNode.FindByPath(name);
            if (node == null)
                throw new Exception("Test with name: (" + name + ") was not found!");

            TestRunner.Run(node.Test);
            //runTestMethod(node.TestMethod); TODO: fix automated testing
        }


        private void updateParents(TestNode node)
        {
            if (node == null) return;
            var parent = node.Parent;
            while (parent != null)
            {
                //if (parent.Parent == null) return; // Ignore root node
                updateNodeState(parent);
                updateNodeInTreeView(parent);
                parent = parent.Parent;
            }
        }

        private void doTestRun(TestNode node)
        {
            if (!data.DontRerun)
                clearNodeStatusRecursive(node);

            data.RunningTestsNodePath = node.GetPath();
            data.LastTestRunPath = null;
            data.Rootnode = rootNode;
            data.Save(getSaveFilePath());

            resumeTestRun();
        }
        private void resumeTestRun()
        {
            //if (!data.RunAutomated)
            //    mainForm.Hide();

            setStatus("Running");

            var node = rootNode.FindByPath(data.RunningTestsNodePath);

            bool resuming = true;

            runTestsInNode(node, ref resuming);

            updateNodeRecursive(node);

            updateParents(node);

            data.RunningTestsNodePath = null;
            data.LastTestRunPath = null;

            //if (!data.RunAutomated)
            //    mainForm.Show();

            setStatus("");
        }
        private void runTestsInNode(TestNode node, ref bool resuming)
        {
            if (data.DontRerun) resuming = false;
            if (data.LastTestRunPath == null)
                resuming = false;
            if (node == null) return;
            if (node.IsTestMethod)
            {
                if (data.DontRerun && node.State != TestState.None)
                    return;
                if (resuming)
                {
                    if (node.GetPath() == data.LastTestRunPath)
                        resuming = false;

                    return;
                }

                //TODO: mainForm.treeView.SelectedNode = node.TreeNode;
                SetScopedVariablesRunning();

                data.LastTestRunPath = node.GetPath();

                //Console.WriteLine("Running Test: " + node.TestMethod.DeclaringType.FullName + "." + node.TestMethod.Name);

                node.State = TestState.Failed;
                TestRunner.Run(node.Test);
                //var testResult = runTestMethod(node.TestMethod);


                ClearScopedVariables();



                node.State = TestState.Success;



                //if (!testResult.Success)
                //if (testResult.ErrorType == typeof(NotImplementedException).FullName)
                //node.State = TestState.NotImplemented;
                //else
                //node.State = TestState.Failed;

                updateNodeInTreeView(node);


            }
            for (int i = 0; i < node.Children.Count; i++)
            {
                var c = node.Children[i];

                runTestsInNode(c, ref resuming);
            }


        }


        ///
        /// TODO:fix automated testing
        /// 

        /*private TestResult runTestMethod(MethodInfo method)
        {
            if (data.RunAutomated)
                return RunTestInOtherProcess(method.DeclaringType.Assembly, method.DeclaringType.FullName, method.Name);

            var obj = new CallbackObject(NUnitTestRunner);
            obj.TypeFullQualifiedName = method.DeclaringType.AssemblyQualifiedName;
            obj.MethodName = method.Name;
            




            if (data != null) // This is in case of running with RunTestByName (without userinterface)
            {
                obj.DebugMode = !data.RunAutomated; // setting
                //data = new SaveData();
                data.Rootnode = rootNode;


                data.Save(getSaveFilePath());
            }


            Exception ex;

            //ex = runSeperateAppdomain(obj);
            ex = runCurrentAppdomain(obj);


            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();

            return TestResult.FromException(ex);
            //obj.RunAutomated();

        }*/

        protected ITestRunner TestRunner { get; set; }

        private Exception runCurrentAppdomain(CallbackObject callbackObject)
        {
            callbackObject.RunTest();
            return callbackObject.ThrowedException;
        }

        private Exception runSeperateAppdomain(CallbackObject obj)
        {
            var appDomain = AppDomain.CreateDomain("TestRunnerNew");
            //appDomain.Load(new AssemblyName(TestsAssembly.FullName));

            appDomain.DoCallBack(obj.RunTest);

            var ex = (Exception)appDomain.GetData("TestException");

            try
            {
                AppDomain.Unload(appDomain);// this causes crashes in character controllers?

            }
            catch (Exception unloadEx)
            {
                Console.WriteLine(unloadEx);
            }
            return ex;
        }


        private void SetScopedVariablesRunning()
        {
            lock (scopeLock)
            {
                IsRunning = true;
                IsRunningAutomated = true;
            }

        }
        private void ClearScopedVariables()
        {
            lock (scopeLock)
            {
                IsRunning = false;
                IsRunningAutomated = false;
            }
        }

        private void updateNodeRecursive(TestNode node)
        {
            if (node == null) return;
            // Update children
            for (int i = 0; i < node.Children.Count; i++)
            {
                var c = node.Children[i];
                updateNodeRecursive(c);
            }

            updateNodeState(node);



            updateNodeInTreeView(node);

        }
        private void updateNodeState(TestNode node)
        {
            if (node.IsTestMethod) return;
            bool allSuccess = true;

            bool fail = false;
            bool notImplemented = false;
            bool none = false;

            for (int i = 0; i < node.Children.Count; i++)
            {
                var c = node.Children[i];
                if (c.State == TestState.Failed)
                    fail = true;

                if (c.State == TestState.NotImplemented)
                    notImplemented = true;

                if (c.State != TestState.Success)
                    allSuccess = false;
                if (c.State == TestState.None)
                    none = true;

            }

            if (fail) node.State = TestState.Failed;
            else if (none) node.State = TestState.None;
            else if (notImplemented) node.State = TestState.NotImplemented;
            else if (allSuccess) node.State = TestState.Success;

        }
        private void updateNodeInTreeView(TestNode node)
        {
            switch (node.State)
            {
                case TestState.None:
                    node.TreeNode.BackColor = Color.White;
                    break;
                case TestState.Failed:
                    node.TreeNode.BackColor = Color.LightSalmon;
                    break;
                case TestState.Success:
                    node.TreeNode.BackColor = Color.LightGreen;
                    break;
                case TestState.NotImplemented:
                    node.TreeNode.BackColor = Color.Yellow;
                    break;

            }
        }
        private void clearNodeStatusRecursive(TestNode node)
        {
            // Update children
            for (int i = 0; i < node.Children.Count; i++)
            {
                var c = node.Children[i];
                updateNodeRecursive(c);
            }

            node.State = TestState.None;
        }

        private void setStatus(string status)
        {
            //TODO:
            if (status != "")
                status = " - " + status;
            //mainForm.Text = "NUnitTestRunner" + status;
        }

        private void loadState(string path)
        {
            if (!File.Exists(path)) return;
            //SaveData data = null;
            data = SaveData.Load(path);

            loadSavedNodesRecursive(data.Rootnode, rootNode);

            updateNodeRecursive(rootNode);

        }

        private void loadSavedNodesRecursive(TestNode savedNode, TestNode node)
        {
            node.State = savedNode.State;


            for (int i = 0; i < savedNode.Children.Count; i++)
            {
                var c = savedNode.Children[i];

                var foundC = node.FindChild(c.Text);
                if (foundC != null)
                    loadSavedNodesRecursive(c, foundC);
            }

        }

        private string getSaveFilePath()
        {
            return Application.StartupPath + "\\TestRunnerConfig.xml";
        }


        public enum TestState
        {
            None,
            Success,
            Failed,
            NotImplemented
        }

        [Serializable]
        public class CallbackObject
        {
            public string TypeFullQualifiedName;
            public string MethodName;
            public bool DebugMode;


            private ITestRunner runner;

            public CallbackObject(ITestRunner runner)
            {
                this.runner = runner;
                ThrowedException = null;
            }

            public Exception ThrowedException { get; private set; }


            public void RunTest()
            {
                Type type;

                type = Type.GetType(TypeFullQualifiedName);

                if (type == null) throw new Exception("Test type not found in the new appdomain!");
                var test = Activator.CreateInstance(type);
                var method = type.GetMethod(MethodName);
                if (DebugMode)
                    runTestDebug(test, method);
                else
                    runTestAutomated(test, method);
            }

            private void runTestDebug(object test, MethodInfo method)
            {
                var thread = new Thread(delegate()
                {
                    //runner.RunNormal(test, method);

                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Name = "TestThread";
                thread.Start();

                thread.Join();
            }

            private void runTestAutomated(object test, MethodInfo method)
            {
                var thread = new Thread(delegate()
                {
                    //ThrowedException = runner.RunAutomated(test, method);

                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Name = "TestThread";
                thread.Start();

                thread.Join();

                AppDomain.CurrentDomain.SetData("TestException", ThrowedException);
            }


        }

    }
}
