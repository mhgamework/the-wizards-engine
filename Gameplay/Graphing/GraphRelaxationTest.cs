using System.Collections.Generic;
using System.Drawing;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
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

        [Test]
        public void TestBasicForce()
        {
            //IMutableVertexAndEdgeListGraph;
            //IMutableBidirectionalGraph 

            //IVertexAndEdgeListGraph;

            var g = new AdjacencyGraph<MyVertex, MyEdge>();

            var colorsDrawing = new[] { Color.Red, Color.Green, Color.Blue, Color.Orange, Color.White, Color.Yellow, Color.Purple, Color.Pink, Color.Brown, Color.Black, Color.Gray };
            var colors = colorsDrawing.Select(c => new Color4(c)).ToArray();

            var vertices = new List<MyVertex>();
            var edges = new List<MyEdge>();

            foreach (var c in colors)
            {
                vertices.Add(new MyVertex { Color = c });
            }

            for (int i = 1; i < vertices.Count; i++)
            {
                edges.Add(new MyEdge { Source = vertices[0], Target = vertices[1] });
            }

            var relaxer = new BasicForceRelaxer();

            engine.AddSimulator(new BasicSimulator(delegate
                {
                    relaxer.Relax();

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

        private class MyVertex : IVertex3D
        {
            public Color4 Color { get; set; }
            public Vector3 Position { get; set; }
        }
        private class MyEdge : IEdge<MyVertex>
        {
            public MyVertex Source { get; set; }
            public MyVertex Target { get; set; }
        }
    }
}