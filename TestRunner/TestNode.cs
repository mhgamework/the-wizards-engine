using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MHGameWork.TheWizards.TestRunner
{
    public class TestNode : IComparable<TestNode>
    {
        public string Text;
        [XmlIgnore]
        public TestNode Parent;
        [XmlElement()]
        public List<TestNode> Children = new List<TestNode>();

        [XmlIgnore]
        public MethodInfo TestMethod;
        [XmlIgnore]
        public Type TestClass;

        public TestRunnerGUI.TestState State;

        [XmlIgnore]
        public TreeNode TreeNode;

        public int CompareTo(TestNode other)
        {
            return Text.CompareTo(other.Text);
        }

        public override string ToString()
        {
            return Text;
        }

        public TestNode FindOrCreateChild(string text)
        {
            var c = FindChild(text);
            if (c != null) return c;

            return CreateChild(text);

        }

        public TestNode FindChild(string text)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var c = Children[i];
                if (c.Text == text) return c;

            }
            return null;
        }

        public TestNode FindByPath(string textPath)
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

        public TestNode CreateChild(string text)
        {
            var c = new TestNode { Text = text, Parent = this };
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

        public TestNode AddTestMethod(MethodInfo method)
        {
            var c = CreateChild(method.Name);
            c.TestClass = this.TestClass;
            c.TestMethod = method;

            return c;
        }
        public TestNode AddTestClass(Type type)
        {
            var c = CreateChild(type.Name);
            c.TestClass = type;

            return c;
        }

        public bool IsTestClass { get { return TestClass != null && TestMethod == null; } }
        public bool IsTestMethod { get { return TestMethod != null; } }
        public bool IsNamespace { get { return TestClass == null; } }
    }
}