﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using MHGameWork.TheWizards.Graphics;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Utilities
{
    public class TestRunner
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

        private static string runTestOtherProcessCommand;
        public static Process RunTestInOtherProcess(string testPath)
        {
            // Cheat for the moment
            var complete = string.Format(runTestOtherProcessCommand, testPath);

            lock (scopeLock)
            {
                //var pi = new ProcessStartInfo(complete);
                // Cheating the crap out of me
                var pi = new ProcessStartInfo(complete.Substring(0, complete.IndexOf("\"", 3) + 1), complete.Substring(complete.IndexOf("\"", 3) + 2));
                pi.UseShellExecute = false;

                var process = new Process();
                process.StartInfo = pi;
                process.Start();
                return process;
            }
        }



        private static object scopeLock = new object();
        //TODO
        //public bool AutoStartMode { get; set; }

        /// <summary>
        /// the command line to execute. {0} will be replace by the test name
        /// </summary>
        public string RunTestNewProcessPath { get; set; }

        private TestRunnerForm mainForm;
        private Dictionary<string, TreeNode> nodesDict = new Dictionary<string, TreeNode>();
        private TreeDataNode RootNode;
        private float autoShutDownValue = 2;
        SaveData data;

        public Assembly TestsAssembly
        { get; set; }


        public void Run()
        {
            var assembly = TestsAssembly;



            mainForm = new TestRunnerForm();

            buildTestsTree();
            RootNode.SortRecursive();

            var node = new TreeNode();
            mainForm.treeView.Nodes.Add(node);

            RootNode.TreeNode = node;
            buildTreeView(RootNode, node);

            mainForm.treeView.KeyUp += new KeyEventHandler(treeView_KeyUp);
            mainForm.FormClosing += new FormClosingEventHandler(mainForm_FormClosing);
            mainForm.Load += new EventHandler(mainForm_Load);

            mainForm.chkAutomated.CheckedChanged += new EventHandler(chkAutomated_CheckedChanged);
            mainForm.chkDontRerun.CheckedChanged += new EventHandler(chkDontRerun_CheckedChanged);


            hideSettingsPanel();

            loadState();

            mainForm.chkAutomated.Checked = data.RunAutomated;
            mainForm.chkDontRerun.Checked = data.DontRerun;

            mainForm.Show();
            if (data.RunningTestsNodePath != null && data.RunAutomated)
                resumeTestRun();

            Application.Run(mainForm);

        }

        public void RunTestByName(string name)
        {
            buildTestsTree();
            var node = RootNode.FindByPath(name);
            if (node == null)
                throw new Exception("Test with name: (" + name + ") was not found!");
            runTestMethod(node.TestMethod);
        }


        private void buildTreeView(TreeDataNode dataNode, TreeNode node)
        {
            node.Text = dataNode.Text;
            node.Tag = dataNode;

            for (int i = 0; i < dataNode.Children.Count; i++)
            {
                var c = dataNode.Children[i];

                var childNode = new TreeNode();
                node.Nodes.Add(childNode);
                c.TreeNode = childNode;

                buildTreeView(c, childNode);
            }
        }
        private void buildTestsTree()
        {
            var assembly = TestsAssembly;


            RootNode = new TreeDataNode();
            RootNode.Text = "_ROOT_";

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


        }


        private TreeDataNode getOrCreateParentNode(Type type)
        {
            var parts = type.FullName.Split('.');

            var parent = RootNode;

            for (int i = 0; i < parts.Length - 1; i++) // Note the -1 here
            {
                parent = parent.FindOrCreateChild(parts[i]);
            }

            return parent;
        }
        private void updateParents(TreeDataNode node)
        {
            var parent = node.Parent;
            while (parent != null)
            {
                //if (parent.Parent == null) return; // Ignore root node
                updateNodeState(parent);
                updateNodeInTreeView(parent);
                parent = parent.Parent;
            }
        }

        private void doTestRun(TreeDataNode node)
        {
            if (!data.DontRerun)
                clearNodeStatusRecursive(node);
            data.RunningTestsNodePath = node.GetPath();
            data.LastTestRunPath = null;
            resumeTestRun();
        }
        private void resumeTestRun()
        {
            if (!data.RunAutomated)
                mainForm.Hide();

            setStatus("Running");

            var node = RootNode.FindByPath(data.RunningTestsNodePath);

            bool resuming = true;

            runTestsInNode(node, ref resuming);

            updateNodeRecursive(node);

            updateParents(node);

            data.RunningTestsNodePath = null;
            data.LastTestRunPath = null;

            if (!data.RunAutomated)
                mainForm.Show();

            setStatus("");
        }
        private void runTestsInNode(TreeDataNode node, ref bool resuming)
        {
            if (data.DontRerun) resuming = false;
            if (data.LastTestRunPath == null)
                resuming = false;

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

                mainForm.treeView.SelectedNode = node.TreeNode;
                SetScopedVariablesRunning();

                data.LastTestRunPath = node.GetPath();

                Console.WriteLine("Running Test: " + node.TestMethod.DeclaringType.FullName + "." + node.TestMethod.Name);

                node.State = TestState.Failed;
                var ex = runTestMethod(node.TestMethod);


                ClearScopedVariables();



                node.State = TestState.Success;



                if (ex != null)
                    if (ex is NotImplementedException)
                        node.State = TestState.NotImplemented;
                    else
                        node.State = TestState.Failed;

                updateNodeInTreeView(node);


            }
            for (int i = 0; i < node.Children.Count; i++)
            {
                var c = node.Children[i];

                runTestsInNode(c, ref resuming);
            }


        }

        private Exception runTestMethod(MethodInfo method)
        {
            var appDomain = AppDomain.CreateDomain("TestRunnerNew");
            var assembly = appDomain.Load(new AssemblyName(TestsAssembly.FullName));


            var type = method.DeclaringType;

            var obj = new CallbackObject();
            obj.AssemblyName = TestsAssembly.FullName;
            obj.TypeFullQualifiedName = method.DeclaringType.AssemblyQualifiedName;
            obj.MethodName = method.Name;
            obj.AutoShutDown = autoShutDownValue;
            obj.RunTestOtherProcessCommand = RunTestNewProcessPath;

            if (data != null) // This is in case of running with RunTestByName (without userinterface)
            {
                obj.RunAutomated = data.RunAutomated;
                obj.DebugMode = !data.RunAutomated; // setting
                saveState();//save state incase of crash
            }


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

            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();

            return ex;
            //obj.RunTest();

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

        private void updateNodeRecursive(TreeDataNode node)
        {
            // Update children
            for (int i = 0; i < node.Children.Count; i++)
            {
                var c = node.Children[i];
                updateNodeRecursive(c);
            }

            updateNodeState(node);



            updateNodeInTreeView(node);

        }
        private void updateNodeState(TreeDataNode node)
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
        private void updateNodeInTreeView(TreeDataNode node)
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
        private void clearNodeStatusRecursive(TreeDataNode node)
        {
            // Update children
            for (int i = 0; i < node.Children.Count; i++)
            {
                var c = node.Children[i];
                updateNodeRecursive(c);
            }

            node.State = TestState.None;
        }

        void mainForm_Load(object sender, EventArgs e)
        {

        }
        void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveState();
        }
        void treeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                if (mainForm.treeView.SelectedNode == null) return;
                var data = mainForm.treeView.SelectedNode.Tag as TreeDataNode;

                if (data == RootNode) return;

                doTestRun(data);
            }
            if (e.KeyCode == Keys.F6)
            {
                resumeTestRun();
            }
            if (e.KeyCode == Keys.F2)
            {
                toggleSettingsPanel();
            }

            if (e.KeyCode == Keys.Enter)
            {
                if (mainForm.treeView.SelectedNode == null) return;
                var data = mainForm.treeView.SelectedNode.Tag as TreeDataNode;

                if (data.IsTestMethod)
                    doTestRun(data);
            }
            if (e.KeyCode == Keys.Escape)
            {
                mainForm.Close();
            }
        }
        void chkAutomated_CheckedChanged(object sender, EventArgs e)
        {
            data.RunAutomated = mainForm.chkAutomated.Checked;
        }
        void chkDontRerun_CheckedChanged(object sender, EventArgs e)
        {
            data.DontRerun = mainForm.chkDontRerun.Checked;
        }

        private bool settingsPanelVisible = true;
        private void showSettingsPanel()
        {
            mainForm.Width = mainForm.settingsPanel.Right;
            settingsPanelVisible = true;
        }
        private void hideSettingsPanel()
        {
            mainForm.Width = mainForm.treeView.Width;
            settingsPanelVisible = false;
        }
        private void toggleSettingsPanel()
        {
            if (settingsPanelVisible)
                hideSettingsPanel();
            else
                showSettingsPanel();
        }
        private void setStatus(string status)
        {
            if (status != "")
                status = " - " + status;
            mainForm.Text = "TestRunner" + status;
        }

        private void loadState()
        {
            data = new SaveData();
            if (!File.Exists(getSaveFilePath())) return;
            //SaveData data = null;
            var s = new XmlSerializer(typeof(SaveData));
            using (var fs = new FileStream(getSaveFilePath(), FileMode.OpenOrCreate))
            {
                data = (SaveData)s.Deserialize(fs);
            }


            var tree = mainForm.treeView;


            if (data.SelectedPath == "")
                return;

            var dataNode = RootNode.FindByPath(data.SelectedPath);
            if (dataNode == null)
                return;
            tree.SelectedNode = dataNode.TreeNode;

            loadSavedNodesRecursive(data.Rootnode, RootNode);

            updateNodeRecursive(RootNode);
        }
        private void loadSavedNodesRecursive(TreeDataNode savedNode, TreeDataNode node)
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

        private void saveState()
        {
            //data = new SaveData();
            data.Rootnode = RootNode;
            var node = mainForm.treeView.SelectedNode;
            if (node != null)
                data.SelectedPath = (node.Tag as TreeDataNode).GetPath();

            var s = new XmlSerializer(typeof(SaveData));

            using (var fs = new FileStream(getSaveFilePath(), FileMode.Create))
            {
                s.Serialize(fs, data);
            }
        }

        private string getSaveFilePath()
        {
            return Application.StartupPath + "\\TestRunnerConfig.xml";
        }


        public class SaveData
        {


            public string SelectedPath;
            public bool RunAutomated;
            /// <summary>
            /// Only run tests with State none
            /// </summary>
            public bool DontRerun;

            //public bool RunInSeperateAppdomain;


            public string RunningTestsNodePath;
            public string LastTestRunPath;




            public TreeDataNode Rootnode;


        }

        public enum TestState
        {
            None,
            Success,
            Failed,
            NotImplemented
        }

        public class TreeDataNode : IComparable<TreeDataNode>
        {
            public string Text;
            [XmlIgnore]
            public TreeDataNode Parent;
            [XmlElement()]
            public List<TreeDataNode> Children = new List<TreeDataNode>();

            [XmlIgnore]
            public MethodInfo TestMethod;
            [XmlIgnore]
            public Type TestClass;

            public TestState State;

            [XmlIgnore]
            public TreeNode TreeNode;

            public int CompareTo(TreeDataNode other)
            {
                return Text.CompareTo(other.Text);
            }

            public override string ToString()
            {
                return Text;
            }

            public TreeDataNode FindOrCreateChild(string text)
            {
                var c = FindChild(text);
                if (c != null) return c;

                return CreateChild(text);

            }

            public TreeDataNode FindChild(string text)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    var c = Children[i];
                    if (c.Text == text) return c;

                }
                return null;
            }

            public TreeDataNode FindByPath(string textPath)
            {
                if (textPath == null) return null;

                if (textPath.Contains('.'))
                {
                    var name = textPath.Substring(0, textPath.IndexOf('.'));
                    textPath = textPath.Substring(textPath.IndexOf('.') + 1);
                    var c = FindChild(name);
                    if (c == null) return null;
                    return c.FindByPath(textPath);
                }

                return FindChild(textPath);
            }

            public string GetPath()
            {
                if (Parent == null) return "";
                if (Parent.Parent == null) return Text;
                return Parent.GetPath() + "." + Text;
            }

            public TreeDataNode CreateChild(string text)
            {
                var c = new TreeDataNode { Text = text, Parent = this };
                Children.Add(c);
                return c;
            }

            public void SortRecursive()
            {
                Children.Sort();
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].SortRecursive();
                }
            }

            public TreeDataNode AddTestMethod(MethodInfo method)
            {
                var c = CreateChild(method.Name);
                c.TestClass = this.TestClass;
                c.TestMethod = method;

                return c;
            }
            public TreeDataNode AddTestClass(Type type)
            {
                var c = CreateChild(type.Name);
                c.TestClass = type;

                return c;
            }

            public bool IsTestClass { get { return TestClass != null && TestMethod == null; } }
            public bool IsTestMethod { get { return TestMethod != null; } }
            public bool IsNamespace { get { return TestClass == null; } }
        }

        [Serializable]
        public class CallbackObject
        {
            public string TypeFullQualifiedName;
            public string MethodName;
            public string AssemblyName;

            public bool RunAutomated;
            public float AutoShutDown;
            public bool DebugMode;

            public string RunTestOtherProcessCommand;



            public void RunTest()
            {
                TestRunner.runTestOtherProcessCommand = this.RunTestOtherProcessCommand;
                XNAGame.AutoShutdown = AutoShutDown;
                XNAGame.DefaultInputDisabled = true;

                if (!RunAutomated)
                {
                    XNAGame.AutoShutdown = -1;
                    XNAGame.DefaultInputDisabled = false;
                }

                var type = Type.GetType(TypeFullQualifiedName);
                if (type == null) throw new Exception("Test type not found in the new appdomain!");
                var test = Activator.CreateInstance(type);

                var method = type.GetMethod(MethodName);

                if (DebugMode)
                    runTestDebug(test, method);
                else
                    runTestAutomated(test, method);

                //method.Invoke(test, null);

            }

            private void runTestDebug(object test, MethodInfo method)
            {
                var thread = new Thread(delegate()
                {
                    runTestJob(test, method);

                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Name = "TestThread";
                thread.Start();

                thread.Join();
            }

            private void runTestAutomated(object test, MethodInfo method)
            {
                Exception ThrowedException = null;
                var thread = new Thread(delegate()
                {
                    try
                    {
                        runTestJob(test, method);
                    }
                    catch (Exception ex)
                    {
                        ThrowedException = ex;
                        Console.WriteLine(ex);

                        var attrs = method.GetCustomAttributes(typeof(ExpectedExceptionAttribute), false);
                        for (int i = 0; i < attrs.Length; i++)
                        {
                            var attr = (ExpectedExceptionAttribute)attrs[i];
                            if (ex.GetType() == attr.ExpectedException)
                            {
                                if (attr.ExpectedMessage == ex.Message || attr.ExpectedMessage == null)
                                {
                                    ThrowedException = null;
                                    Console.WriteLine("This Exception is a valid result for this test");
                                    break;
                                }

                            }
                        }

                    }
                    catch
                    {
                        Console.WriteLine("An Unmanaged exception has been thrown!!");
                        ThrowedException = new Exception("An Unmanaged exception has been thrown!!");
                    }

                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Name = "TestThread";
                thread.Start();

                thread.Join();

                AppDomain.CurrentDomain.SetData("TestException", ThrowedException);
            }

            private void runTestJob(object test, MethodInfo method)
            {
                var setupMethod = FindSingleMethodWithAttribute(test.GetType(),
                                                                typeof(NUnit.Framework.SetUpAttribute));
                if (setupMethod != null)
                {
                    var setupDeleg = (TestDelegate)Delegate.CreateDelegate(typeof(TestDelegate), test, setupMethod);
                    setupDeleg();
                }



                var testDeleg = (TestDelegate)Delegate.CreateDelegate(typeof(TestDelegate), test, method);
                testDeleg();



                var tearDownMethod = FindSingleMethodWithAttribute(test.GetType(),
                                                typeof(NUnit.Framework.TearDownAttribute));
                if (tearDownMethod != null)
                {
                    var tearDownDeleg = (TestDelegate)Delegate.CreateDelegate(typeof(TestDelegate), test, tearDownMethod);
                    tearDownDeleg();
                }


            }
        }

        private delegate void TestDelegate();

        public static MethodInfo FindSingleMethodWithAttribute(Type type, Type attributeType)
        {
            var methods =
                    type.GetMethods().Where(
                        m => m.GetCustomAttributes(attributeType, false).Length > 0).ToArray();

            if (methods.Length == 0) return null;
            if (methods.Length > 1) throw new Exception("More than one method with given attributeType found!");

            return methods[0];
        }

    }

}
