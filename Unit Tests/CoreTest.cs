using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests
{
    [TestFixture]
    public class CoreTest
    {
        /// <summary>
        /// Tests the QuadtreeVisualizer by creating a quadtree from scratch.
        /// </summary>
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestQuadtreeVisualizer()
        {
            TestQuadTreeNode root = createSimpleQuadtree();
            QuadTreeVisualizer visualizer = new QuadTreeVisualizer();

            XNAGame game = new XNAGame();

            game.DrawEvent += delegate
                                  {
                                      visualizer.RenderNodeGroundBoundig(game, root);
                                  };

            game.Run();


        }

        /// <summary>
        /// Creates a simple quadtree from scratch, without using any other functions (split/merge etc)
        /// </summary>
        private TestQuadTreeNode createSimpleQuadtree()
        {

            TestQuadTreeNode root = new TestQuadTreeNode();
            QuadTreeNodeData<TestQuadTreeNode> rootNodeData = new QuadTreeNodeData<TestQuadTreeNode>();
            rootNodeData.BoundingBox = new BoundingBox(new Vector3(-20, -20, -20), new Vector3(20, 20, 20)).dx();


            TestQuadTreeNode child = new TestQuadTreeNode();
            QuadTreeNodeData<TestQuadTreeNode> childNodeData = new QuadTreeNodeData<TestQuadTreeNode>();
            child = new TestQuadTreeNode();
            rootNodeData.UpperLeft = child;
            childNodeData.Parent = root;
            childNodeData.BoundingBox = new BoundingBox(new Vector3(-20, -20, -20), new Vector3(0, 20, 0)).dx();

            child.NodeData = childNodeData;

            root.NodeData = rootNodeData;

            return root;

        }

        /// <summary>
        /// Tests the split and merge functions by first splitting and then merging parts of a quadtree
        /// </summary>
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestQuadtreeSplitMerge()
        {
            TestQuadTreeNode root = new TestQuadTreeNode();

            root.NodeData = new QuadTreeNodeData<TestQuadTreeNode>(
                new BoundingBox(new Vector3(-100, -5, -100), new Vector3(100, 5, 100)).dx()
                );

            QuadTree.Split(root, 4);

            QuadTree.Merge(root.NodeData.LowerRight.NodeData.LowerRight.NodeData.UpperLeft);

            QuadTree.MergeRecursive(root.NodeData.LowerLeft.NodeData.UpperRight);

            QuadTree.MergeRecursive(root.NodeData.UpperRight);

            QuadTreeVisualizer visualizer = new QuadTreeVisualizer();

            XNAGame game = new XNAGame();

            game.DrawEvent +=
                delegate
                {
                    visualizer.RenderNodeGroundBoundig(game, root);
                    visualizer.RenderNodeBoundingBox(game, root);
                };

            game.Run();
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestXnaMathExtensionMergeBoundingBox()
        {
            var b0 = new BoundingBox();
            var b1 = new BoundingBox(Vector3.Zero, Vector3.One * 2);
            var b2 = new BoundingBox(new Vector3(2, -1, -3), new Vector3(4, 3, 1));
            var b3 = b0.MergeWith(b1).MergeWith(b2);

            XNAGame game = new XNAGame();

            game.DrawEvent +=
                delegate
                {
                    game.LineManager3D.AddBox(b0, Color.Black);
                    game.LineManager3D.AddBox(b1, Color.Red);
                    game.LineManager3D.AddBox(b2, Color.Green);
                    game.LineManager3D.AddBox(b3, Color.Brown);
                };

            game.Run();
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestXnaMathExtensionCreateRotationMatrixFromDirection()
        {

            XNAGame game = new XNAGame();

            game.DrawEvent +=
                delegate
                {
                    var v1 = Vector3.Normalize(new Vector3(1, 4, 2));
                    var v2 = Vector3.Normalize(new Vector3(-21, -4, 2));

                    var m1 = Vector3.Transform(Vector3.Forward,
                                               XnaMathExtensions.CreateRotationMatrixFromDirectionVector(v1));
                    var m2 = Vector3.Transform(Vector3.Forward,
                                               XnaMathExtensions.CreateRotationMatrixFromDirectionVector(v2));
                    game.LineManager3D.AddLine(Vector3.Zero, v1 * 2, Color.Red);
                    game.LineManager3D.AddLine(Vector3.Zero, v2 * 2, Color.Red);

                    game.LineManager3D.AddLine(m1 * 2, m1 * 4, Color.Yellow);
                    game.LineManager3D.AddLine(m2 * 2, m2 * 4, Color.Yellow);
                };

            game.Run();
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestIntervalCaller()
        {

            XNAGame game = new XNAGame();

            IntervalCaller caller1 = null, caller2 = null;
            BoxMesh box1 = null;
            BoxMesh box2 = null;
            box1 = new BoxMesh();
            box2 = new BoxMesh();
            game.AddXNAObject(box1);
            game.AddXNAObject(box2);

            var pos = new Vector3(2, 0, 0);

            caller1 = new IntervalCaller(delegate
            {
                if (box1.Color.R == 255)
                    box1.Color = Color.Green;
                else
                    box1.Color = Color.Red;

            }, 1 / 5f);

            caller2 = new IntervalCaller(delegate
                                             {
                                                 if (pos.Z == 0)
                                                     pos.Z = 2;
                                                 else
                                                     pos.Z = 0;
                                                 box2.WorldMatrix = Matrix.CreateTranslation(pos);
                                             }, 1/3f);



            game.InitializeEvent += delegate
                                        {

                                        };

            game.DrawEvent +=
                delegate
                    {
                        caller1.Update(game.Elapsed);
                        caller2.Update(game.Elapsed);
                    };

            game.Run();
        }

        class TestQuadTreeNode : IQuadTreeNode<TestQuadTreeNode>
        {

            #region IQuadTreeNode<TestQuadTreeNode> Members

            private QuadTreeNodeData<TestQuadTreeNode> nodeData;

            public QuadTreeNodeData<TestQuadTreeNode> NodeData
            {
                get
                {
                    return nodeData;
                }
                set
                {
                    nodeData = value;
                }
            }

            public TestQuadTreeNode CreateChild(QuadTreeNodeData<TestQuadTreeNode> nodeData)
            {
                return new TestQuadTreeNode();
            }

            #endregion
        }
    }
}
