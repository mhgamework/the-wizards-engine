using System;
using System.Collections.Generic;
using System.Threading;
using DirectX11;
using DirectX11.Graphics;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Tests.Rendering
{
    [TestFixture]
    public class CullingTest
    {
        /// <summary>
        /// TODO: test remove cullable
        /// </summary>
        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestCullerPlaceInTree()
        {
            var game = new DX11Game();
            Vector3 radius = new Vector3(100, 1000, 100);
            FrustumCuller culler = new FrustumCuller(new BoundingBox(-radius, radius), 4);


            var visualizer = new QuadTreeVisualizer();

            List<TestCullObject> cullObjects = new List<TestCullObject>();


            TestCullObject obj;

            obj = new TestCullObject(new Vector3(5, 2, 5), 2);
            cullObjects.Add(obj);

            obj = new TestCullObject(new Vector3(-20, 2, -20), 15);
            cullObjects.Add(obj);

            obj = new TestCullObject(new Vector3(100, 2, -20), 4);
            cullObjects.Add(obj);

            obj = new TestCullObject(new Vector3(-50, 9, 24), 20);
            cullObjects.Add(obj);

            for (int i = 0; i < cullObjects.Count; i++)
            {
                culler.AddCullable(cullObjects[i]);
            }

            game.GameLoopEvent +=
                delegate
                {
                    for (int i = 0; i < cullObjects.Count; i++)
                    {
                        game.LineManager3D.AddBox(cullObjects[i].BoundingBox.dx(), Color.Red.dx());
                    }
                    visualizer.RenderNodeGroundBoundig(game, culler.RootNode,
                        delegate(FrustumCuller.CullNode node, out Color4 col)
                        {
                            col = Color.Green.dx();

                            return node.Cullables.Count == 0;
                        });

                    visualizer.RenderNodeGroundBoundig(game, culler.RootNode,
                       delegate(FrustumCuller.CullNode node, out Color4 col)
                       {
                           col = Color.Orange.dx();

                           return node.Cullables.Count > 0;
                       });
                };



            game.Run();
        }

        /// <summary>
        /// There is still a problem with this test. The second level nodes get put on visible on what seems to be an incorrect moment.
        /// </summary>
        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestCullerVisibility()
        {
            var game = new DX11Game();
            Vector3 radius = new Vector3(100, 1000, 100);
            FrustumCuller culler = new FrustumCuller(new BoundingBox(-radius, radius), 6);



            QuadTreeVisualizer visualizer = new QuadTreeVisualizer();

            List<TestCullObject> cullObjects = new List<TestCullObject>();


            TestCullObject obj;


            for (int i = 0; i < cullObjects.Count; i++)
            {
                culler.AddCullable(cullObjects[i]);
            }



            SpectaterCamera cullCam = new SpectaterCamera(game.Keyboard, game.Mouse, 10f, 80);
            cullCam.Positie = new Vector3(8, 10, 8);
            culler.CullCamera = cullCam;
            cullCam.EnableUserInput = false;

            bool rotate = true;

            int selectedNode = -1;


            game.GameLoopEvent +=
                delegate
                {


                    culler.UpdateVisibility();
                    if (rotate)
                        cullCam.AngleHorizontal += game.Elapsed * MathHelper.Pi * (1 / 8f);

                    if (game.Keyboard.IsKeyPressed(Key.NumberPadPlus))
                        selectedNode++;
                    if (game.Keyboard.IsKeyPressed(Key.NumberPadMinus))
                        selectedNode--;

                    if (game.Keyboard.IsKeyPressed(Key.Return))
                    {
                        int count = -1;
                        visualizer.RenderNodeGroundBoundig(game, culler.RootNode,
                        delegate(FrustumCuller.CullNode node, out Color4 col)
                        {
                            col = Color.Red.dx();
                            count++;
                            if (count == selectedNode)
                            {
                                node.Tag = "SELECTED!";
                            }
                            return count == selectedNode;
                        });
                    }

                    if (game.Keyboard.IsKeyPressed(Key.NumberPad0))
                        rotate = !rotate;








                    game.LineManager3D.AddViewFrustum(new global::DirectX11.BoundingFrustum(cullCam.ViewProjection), Color.Black.dx());
                    for (int i = 0; i < cullObjects.Count; i++)
                    {
                        game.LineManager3D.AddBox(cullObjects[i].BoundingBox.dx(), Color.Red.dx());
                    }
                    visualizer.RenderNodeGroundBoundig(game, culler.RootNode,
                        delegate(FrustumCuller.CullNode node, out Color4 col)
                        {
                            col = Color.Green.dx();

                            return !node.Visible;
                        });

                    visualizer.RenderNodeGroundBoundig(game, culler.RootNode,
                       delegate(FrustumCuller.CullNode node, out Color4 col)
                       {
                           col = Color.Orange.dx();

                           return node.Visible;
                       });

                    cullCam.Update(game.Elapsed);
                    /*int count = -1;
                    visualizer.RenderNodeGroundBoundig(game, culler.RootNode,
                    delegate(Culler.CullNode node, out Color col)
                    {
                        col = Color.Red;
                        count++;
                        return count == selectedNode;
                    });*/
                };



            game.Run();
        }


        public class TestCullObject : ICullable
        {
            private Microsoft.Xna.Framework.BoundingBox boundingBox;
            private int visibleReferenceCount;
            public int VisibleReferenceCount
            {
                get { return visibleReferenceCount; }
                set { visibleReferenceCount = value; }
            }

            public TestCullObject(Vector3 pos, float radius)
            {
                boundingBox = new BoundingBox(pos - MathHelper.One * radius, pos + MathHelper.One * radius).xna();
            }

            public TestCullObject(BoundingBox bb)
            {
                boundingBox = bb.xna();
            }

            #region ICullable Members

            public Microsoft.Xna.Framework.BoundingBox BoundingBox
            {
                get { return boundingBox; }
            }

            #endregion
        }

    }
}
