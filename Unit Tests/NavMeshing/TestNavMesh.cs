using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.NavMeshing;
using MHGameWork.TheWizards.Testing;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.NavMeshing
{
    [TestFixture]
    public class TestNavMesh
    {
        private TestDataValidator validator = new TestDataValidator();
        private NavMesh mesh;
        private DX11Game game;
        private NavMeshNode rootNode;
        private NavMeshNode left;
        private NavMeshNode right;

        private NavMesh createTestMesh()
        {
            //
            //    a-e
            //   /|\|
            //  b-c-d
            //

            var a = new Vector3(0, 0, -1);
            var b = new Vector3(-1, 0, 0);
            var c = new Vector3(0, 0, 0);
            var d = new Vector3(1, 0, 0);
            var e = new Vector3(1, 0, -1);

            rootNode = new NavMeshNode(new[] {a, c, d});
            left = new NavMeshNode(new[] {a, b, c});
            right = new NavMeshNode(new[] {a, d, e});

            //rootNode.SetConnected(0) = left;
            //left.Connected(2) = rootNode;

            //rootNode.Connected[1] = right;
            //right.Connected[0] = rootNode;

            return new NavMesh(rootNode);
        }

        private void init()
        {
            game = new DX11Game();
            game.InitDirectX();
            mesh = createTestMesh();

            var visualizer = new NavMeshVisualizer(new LineManager3DLines(game.Device));
            game.GameLoopEvent += delegate
                                      {
                                          
                                          visualizer.UpdateLines(mesh);
                                          game.LineManager3D.WorldMatrix = Matrix.Translation(0, 0, -1);
                                          game.LineManager3D.Render(visualizer.Lines, game.Camera);
                                      };
        }



        [Test]
        public void TestSplitPolygon()
        {
            var poly1 = new Polygon();
            Polygon poly2;
            poly1.Points = new[] {new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1)};
            poly1.Split(1, new Vector3(0, 0, 0), out poly2);

            validator.ValidateFields(poly1);
            validator.ValidateFields(poly2);
        }

        [Test]
        public void TestVisualizer()
        {
            init();

            game.Run();
        }


        [Test]
        public void TestSplitNode()
        {
            init();
            var builder = new NavMeshBuilder();
            NavMeshNode newNode;
            //builder.SplitNode(rootNode, 0, new Vector3(0, 0, -0.4f), out newNode);

            validator.ValidateFields(rootNode);

            game.Run();
        }
    }
}
