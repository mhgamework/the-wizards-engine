using System.Collections.Generic;
using System.Threading;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.DirectX11.Rendering.Deferred;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Culling
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
            FrustumCuller culler = new FrustumCuller(new BoundingBox(-radius, radius), 5);


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
            cullCam.EnableUserInput = false;

            bool rotate = true;

            int selectedNode = -1;

            var view = culler.CreateView();


            game.GameLoopEvent +=
                delegate
                {

                    view.UpdateVisibility(cullCam.ViewProjection);
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

                            return !view.IsNodeVisible(node);
                        });

                    visualizer.RenderNodeGroundBoundig(game, culler.RootNode,
                       delegate(FrustumCuller.CullNode node, out Color4 col)
                       {
                           col = Color.Orange.dx();

                           return view.IsNodeVisible(node);
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

        /// <summary>
        /// This test shows a problem with the PointLight's view frustums. The up and down frustum contains function gives intersects as result for each node,
        /// being all false positives due to an optimization in the culling algorithm. This problem is somewhat resolved by doing fast boundingbox checks for the entire frustum,
        /// and by reducing the height of the cullnodes.
        /// </summary>
        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestPointLightDownViewFrustumVisibility()
        {
            var game = new DX11Game();
            game.InitDirectX();
            Vector3 radius = new Vector3(100, 1000, 100);
            FrustumCuller culler = new FrustumCuller(new BoundingBox(-radius, radius), 6);



            QuadTreeVisualizer visualizer = new QuadTreeVisualizer();

            List<TestCullObject> cullObjects = new List<TestCullObject>();


            TestCullObject obj;


            for (int i = 0; i < cullObjects.Count; i++)
            {
                culler.AddCullable(cullObjects[i]);
            }




            Matrix viewProjection = Matrix.Identity;

            var pos = game.SpectaterCamera.CameraPosition;

            bool rotate = true;

            int selectedNode = -1;

            var view = culler.CreateView();


            game.GameLoopEvent +=
                delegate
                {
                    if (game.Keyboard.IsKeyDown(Key.C))
                        pos = game.SpectaterCamera.CameraPosition;

                    viewProjection = PointLightRenderer.CreateShadowMapView(pos, 3) *
                                     PointLightRenderer.CreateShadowMapProjection(10);

                    view.UpdateVisibility(viewProjection);


                    if (game.Keyboard.IsKeyPressed(Key.NumberPadPlus))
                        selectedNode++;
                    if (game.Keyboard.IsKeyPressed(Key.NumberPadMinus))
                        selectedNode--;










                    game.LineManager3D.AddViewFrustum(new BoundingFrustum(viewProjection), Color.Black.dx());
                    for (int i = 0; i < cullObjects.Count; i++)
                    {
                        game.LineManager3D.AddBox(cullObjects[i].BoundingBox.dx(), Color.Red.dx());
                    }


                    if (game.Keyboard.IsKeyDown(Key.Return))
                    {
                        int count = -1;
                        visualizer.RenderNodeGroundBoundig(game, culler.RootNode,
                        delegate(FrustumCuller.CullNode node, out Color4 col)
                        {
                            col = Color.Green.dx();

                            count++;
                            if (count == selectedNode)
                            {

                                col = Color.Red.dx();

                                node.Tag = "SELECTED!";
                            }
                            return count == selectedNode;
                        });
                    }
                    else
                        visualizer.RenderNodeGroundBoundig(game, culler.RootNode,
                           delegate(FrustumCuller.CullNode node, out Color4 col)
                           {
                               if (view.IsNodeVisible(node))
                               {
                                   col = Color.Orange.dx();
                                   return true;
                               }
                               col = Color.Green.dx();
                               return false;
                           });

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

        /// <summary>
        /// There is still a problem with this test. The second level nodes get put on visible on what seems to be an incorrect moment.
        /// </summary>
        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void TestCullerObjects()
        {
            var game = new DX11Game();
            game.InitDirectX();
            Vector3 radius = new Vector3(100, 1000, 100);
            FrustumCuller culler = new FrustumCuller(new BoundingBox(-radius, radius), 6);

            var mesh = RenderingTestsHelper.CreateSimpleTestMesh();

            var texturePool = new TheWizards.Rendering.Deferred.TexturePool(game);

            var gBuffer = new GBuffer(game.Device, 800, 600);

            var renderer = new DeferredMeshesRenderer(game, gBuffer, texturePool);
            var final = new CombineFinalRenderer(game, gBuffer);



            DeferredMeshRenderElement middle = null;

            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {

                    var el = renderer.AddMesh(mesh);
                    el.WorldMatrix = Matrix.Translation(MathHelper.Right * i * 2 + Vector3.UnitZ * j * 2);

                    if (i > 20 && i < 30 && j > 20 && j < 30)
                        el.Delete();
                    else
                        culler.AddCullable(el);
                }

            }

            QuadTreeVisualizer visualizer = new QuadTreeVisualizer();

            List<TestCullObject> cullObjects = new List<TestCullObject>();




            SpectaterCamera cullCam = new SpectaterCamera(game.Keyboard, game.Mouse, 10f, 80);
            cullCam.Positie = new Vector3(8, 10, 8);
            cullCam.EnableUserInput = false;

            bool rotate = true;

            int selectedNode = -1;

            var view = culler.CreateView();


            game.GameLoopEvent +=
                delegate
                {

                    view.UpdateVisibility(cullCam.ViewProjection);
                    var visibleCullables = view.GetVisibleCullables();

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





                    gBuffer.Clear();
                    gBuffer.SetTargetsToOutputMerger();
                    renderer.Draw();

                    game.Device.ImmediateContext.ClearState();
                    game.SetBackbuffer();

                    final.DrawCombined();


                    game.LineManager3D.AddViewFrustum(new BoundingFrustum(cullCam.ViewProjection), Color.White.dx());
                    for (int i = 0; i < visibleCullables.Count; i++)
                    {
                        game.LineManager3D.AddBox(visibleCullables[i].BoundingBox.dx(), Color.Red.dx());
                    }
                    visualizer.RenderNodeGroundBoundig(game, culler.RootNode,
                        delegate(FrustumCuller.CullNode node, out Color4 col)
                        {
                            col = Color.Green.dx();

                            return !view.IsNodeVisible(node);
                        });

                    visualizer.RenderNodeGroundBoundig(game, culler.RootNode,
                       delegate(FrustumCuller.CullNode node, out Color4 col)
                       {
                           col = Color.Orange.dx();

                           return view.IsNodeVisible(node);
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
