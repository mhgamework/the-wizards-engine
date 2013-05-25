using System.Collections.Generic;
using System.Drawing;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using NUnit.Framework;
using QuickGraph;
using System.Linq;
using SlimDX;

namespace MHGameWork.TheWizards.Graphing
{
    [TestFixture]
    [EngineTest]
    public class GraphRelaxationTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();
        private Color[] colorsDrawing;
        private Color4[] colors;

        [SetUp]
        public void Setup()
        {
            colorsDrawing = new[]
                {
                    Color.Red, Color.Green, Color.Blue, Color.Orange, Color.White, Color.Yellow, Color.Purple,
                    Color.Pink, Color.Brown, Color.Black, Color.Gray
                };
            colors = colorsDrawing.Select(c => new Color4(c)).ToArray();
        }

        [Test]
        public void TestBasicForce()
        {
            DI.Get<TestSceneBuilder>().Setup = () => createVertices(2);

            var g = new BidirectionalGraph<MyVertex, MyEdge>();
            var edges = new List<MyEdge>();
            var verts = TW.Data.Get<Data>().Vertices;

            edges.Add(new MyEdge { Source = verts[0], Target = verts[1] });

            runRelaxTest(g, verts, edges);
        }

        private void createVertices(int count = -1)
        {
            if (count == -1) count = colors.Length;
            var vertices = new List<MyVertex>();
            for (int i = 0; i < count; i++)
            {
                var c = colors[i % colors.Length];
                vertices.Add(new MyVertex { Color = c });
            }
            TW.Data.Get<Data>().Vertices.Clear(); // TODO: shouldn't need a clear here!
            TW.Data.Get<Data>().Vertices.AddRange(vertices);
        }

        [Test]
        public void TestBasicForceAllToOne()
        {
            DI.Get<TestSceneBuilder>().Setup = () => createVertices();

            var g = new BidirectionalGraph<MyVertex, MyEdge>();

            var edges = new List<MyEdge>();
            var verts = TW.Data.Get<Data>().Vertices;

            for (int i = 1; i < verts.Count; i++)
                edges.Add(new MyEdge { Source = verts[0], Target = verts[i] });

            runRelaxTest(g, verts, edges);
        }

        [Test]
        public void TestBasicForceLine()
        {
            DI.Get<TestSceneBuilder>().Setup = () => createVertices(100);

            var g = new BidirectionalGraph<MyVertex, MyEdge>();

            var edges = new List<MyEdge>();
            var verts = TW.Data.Get<Data>().Vertices;

            var queue = new Queue<MyVertex>();
            foreach (var v in verts) queue.Enqueue(v);

            addEdgesRecursive(queue.Dequeue(), queue, edges, 100000);

            runRelaxTest(g, verts, edges);
        }

        [Test]
        public void TestBasicForceComplex()
        {
            DI.Get<TestSceneBuilder>().Setup = () => createVertices(100);

            var g = new BidirectionalGraph<MyVertex, MyEdge>();

            var edges = new List<MyEdge>();
            var verts = TW.Data.Get<Data>().Vertices;

            var queue = new Queue<MyVertex>();
            foreach (var v in verts) queue.Enqueue(v);

            addEdgesRecursive(queue.Dequeue(), queue, edges, 4);

            runRelaxTest(g, verts, edges);
        }

        [Test]
        public void TestBasicForceDecoupled()
        {
            DI.Get<TestSceneBuilder>().Setup = () => createVertices(10);

            var g = new BidirectionalGraph<MyVertex, MyEdge>();

            var edges = new List<MyEdge>();
            var verts = TW.Data.Get<Data>().Vertices;

            for (int i = 1; i < 4; i++)
                edges.Add(new MyEdge { Source = verts[0], Target = verts[i] });

            for (int i = 6; i < 10; i++)
                edges.Add(new MyEdge { Source = verts[5], Target = verts[i] });

            runRelaxTest(g, verts, edges);

        }

        private void addEdgesRecursive(MyVertex parent, Queue<MyVertex> verts, List<MyEdge> outEdges, int maxDepth)
        {
            if (maxDepth == 0) return;

            for (int i = 0; i < 4; i++)
            {
                if (verts.Count == 0) return;
                var c = verts.Dequeue();
                outEdges.Add(new MyEdge { Source = parent, Target = c });
                addEdgesRecursive(c, verts, outEdges, maxDepth - 1);
            }
        }

        private void runRelaxTest(BidirectionalGraph<MyVertex, MyEdge> g, List<MyVertex> verts, List<MyEdge> edges)
        {
            g.AddVertexRange(verts);
            g.AddEdgeRange(edges);

            var relaxer = new BasicForceRelaxer<MyVertex, MyEdge>(g);
            relaxer.Reset();
            engine.AddSimulator(new BasicSimulator(delegate
                {
                    for (int i = 0; i < 10; i++)
                        relaxer.Relax(TW.Graphics.Elapsed);

                    foreach (var n in g.Vertices)
                    {
                        TW.Graphics.LineManager3D.AddCenteredBox(n.Position, 1, n.Color);
                    }
                    foreach (var e in g.Edges)
                    {
                        TW.Graphics.LineManager3D.AddLine(e.Source.Position, e.Source.Color, e.Target.Position, e.Target.Color);
                    }
                }));
        }

        private class MyVertex : EngineModelObject, IVertex3D
        {
            public Color4 Color { get; set; }
            public Vector3 Position { get; set; }
        }
        private class MyEdge : IEdge<MyVertex>
        {
            public MyVertex Source { get; set; }
            public MyVertex Target { get; set; }
        }

        private class Data : EngineModelObject
        {
            public Data()
            {
                Vertices = new List<MyVertex>();
            }
            public List<MyVertex> Vertices { get; set; }
        }
    }
}