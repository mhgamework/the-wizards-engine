using System;

namespace MHGameWork.TheWizards.TestRunner
{
    public class TestsTreeBuilder
    {
        public TestsTreeBuilder()
        {
            RootNode = new TestNode();
            RootNode.Text = "_ROOT_";

        }

        public TestNode RootNode { get; set; }

        public TestNode GetOrCreateParentNode(Type type)
        {
            var parts = type.FullName.Split('.');

            var parent = RootNode;

            for (int i = 0; i < parts.Length - 1; i++) // Note the -1 here
            {
                parent = parent.FindOrCreateChild(parts[i]);
            }

            return parent;
        }
    }
}