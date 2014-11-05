using System.Collections.Generic;

namespace ObjectVersioning.Tests
{
    /*[TestFixture]
    public class RecursionExtensionsTest
    {
        private Node r;
        private Node ra;
        private Node rb;
        private Node raa;
        private Node rba;
        private Node rbb;

        [SetUp]
        public void Setup()
        {
            r = new Node();
            ra = new Node();
            rb = new Node();
            raa = new Node();
            rba = new Node();
            rbb = new Node();


            r.AddChild(ra);
            r.AddChild(rb);

            ra.AddChild(raa);

            rb.AddChild(rba);
            rb.AddChild(rbb);
        }

        [Test]
        public void TestDepthFirst()
        {
            CollectionAssert.AreEqual(new[] { ra, raa, rb, rba, rbb }, r.DepthFirst(c => c.Children));
            CollectionAssert.AreEqual(new[] { raa }, ra.DepthFirst(c => c.Children));
            CollectionAssert.AreEqual(new[] { rba, rbb }, rb.DepthFirst(c => c.Children));

        }

        [Test]
        public void TestBreadthFirst()
        {
            CollectionAssert.AreEqual(new[] { ra, rb, raa, rba, rbb }, r.BreadthFirst(c => c.Children).PrintArray());
            CollectionAssert.AreEqual(new[] { raa }, ra.BreadthFirst(c => c.Children));
            CollectionAssert.AreEqual(new[] { rba, rbb }, rb.BreadthFirst(c => c.Children));
        }

        [Test]
        public void TestRoot()
        {
            Assert.AreEqual(r, r.Root(c => c.Parent));
            Assert.AreEqual(r, ra.Root(c => c.Parent));
            Assert.AreEqual(r, rb.Root(c => c.Parent));
            Assert.AreEqual(r, raa.Root(c => c.Parent));
            Assert.AreEqual(r, rba.Root(c => c.Parent));
            Assert.AreEqual(r, rbb.Root(c => c.Parent));
        }


        public class Node
        {
            public static int nextId = 0;
            private int id;
            public Node()
            {
                id = nextId++;
            }

            public List<Node> Children = new List<Node>();
            public Node Parent;

            public override string ToString()
            {
                return string.Format("Id: {0}", id);
            }

            public void AddChild(Node child)
            {
                Children.Add(child);
                child.Parent = this;
            }
        }
    }*/
}