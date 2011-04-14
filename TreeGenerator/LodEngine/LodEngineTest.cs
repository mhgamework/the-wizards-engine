using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.ServerClient.CascadedShadowMaps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SampleCommon;
using TreeGenerator.help;
using TreeGenerator.TreeEngine;
using NUnit.Framework;

namespace TreeGenerator.LodEngine
{
    [TestFixture]
    public class LodEngineTest
    {
        [Test]
        public void TestTWRendererTreeRender()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;

            TWRenderer renderer = new TWRenderer(game);
            EngineTreeRenderData renderData;
            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(5);
            TreeStructure structure = TreeStructure.GetTestTreeStructure(game);
            
            TreeTypeData treeTypeData = TreeTypeData.GetTestTreeType(game);
            TreeStructureGenerater structGen = new TreeStructureGenerater();
            structure = structGen.GenerateTree(treeTypeData, 984);
            renderData = gen.GetRenderData(structure, game, 0);

            Seeder seeder = new Seeder(98756);



            TWRenderElement renderElement;
            game.InitializeEvent +=
                delegate
                {
                    renderData.Initialize();
                    renderElement = renderer.CreateElement(renderData);
                    renderElement.WorldMatrix = Matrix.CreateTranslation(new Vector3(0, 0,0));

                    //renderElement = renderer.CreateElement(renderData);
                    //renderElement.WorldMatrix = Matrix.CreateTranslation(new Vector3(1, 0, 1));
                };
            game.UpdateEvent +=
                delegate
                {
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.B))
                    {
                        structure = structGen.GenerateTree(treeTypeData, seeder.NextInt(0,10000));
                        renderData = gen.GetRenderData(structure, game, 0);
                        renderData.Initialize();
                        renderElement = renderer.CreateElement(renderData);
                        renderElement.WorldMatrix = Matrix.CreateTranslation(game.SpectaterCamera.CameraPosition- game.SpectaterCamera.CameraPosition.Y * Vector3.UnitY);
                    }
                };
            game.DrawEvent +=
                delegate
                    {
                        game.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
                    renderer.Render();
                };
            game.Run();

        }
        [Test]
        public void TestSingleModelLodLayer()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;

            TWRenderer renderer = new TWRenderer(game);
            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(10);
            TreeLodEngine lodEngine = new TreeLodEngine();
            TreeLodEntity treeLodEntity;

            ModelLodLayer layer = new ModelLodLayer(0, renderer, gen,50);
            game.InitializeEvent +=
                delegate
                {
                    lodEngine.AddITreeLodLayer(layer, 10);
                    treeLodEntity = lodEngine.CreateTreeLodEntity(TreeStructure.GetTestTreeStructure(game));
                    treeLodEntity.WorldMatrix = Matrix.CreateTranslation(new Vector3(10, 0, 10));

                    treeLodEntity = lodEngine.CreateTreeLodEntity(TreeStructure.GetTestTreeStructure(game));
                    treeLodEntity.WorldMatrix = Matrix.CreateTranslation(new Vector3(2, 0, 2));

                    treeLodEntity = lodEngine.CreateTreeLodEntity(TreeStructure.GetTestTreeStructure(game));
                    treeLodEntity.WorldMatrix = Matrix.CreateTranslation(new Vector3(2, 0, 10));
                };
            game.UpdateEvent +=
                delegate
                {
                    lodEngine.Update(game);
                };
            game.DrawEvent +=
                delegate
                {
                    renderer.Render();
                };
            game.Run();
        }

        [Test]
        public void TestModelLodLayerForest()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;

            TWRenderer renderer = new TWRenderer(game);
            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(10);
            TreeLodEngine lodEngine = new TreeLodEngine();
            TreeLodEntity treeLodEntity;

            ModelLodLayer layer = new ModelLodLayer(0, renderer, gen, 50);
            TreeStructure treeStruct = TreeStructure.GetTestTreeStructure(game);
            game.InitializeEvent +=
                delegate
                {
                    lodEngine.AddITreeLodLayer(layer, 0);
                    lodEngine.AddITreeLodLayer(new ModelLodLayer(1, renderer, gen, 50), 20);
                    lodEngine.AddITreeLodLayer(new ModelLodLayer(2, renderer, gen, 50), 100);
                    Seeder seeder = new Seeder(123);
                    for (int i = 0; i < 500; i++)
                    {
                        treeLodEntity = lodEngine.CreateTreeLodEntity(treeStruct);

                        treeLodEntity.WorldMatrix = Matrix.CreateTranslation(seeder.NextVector3(new Vector3(0, 0, 0), new Vector3(300, 0, 300)));
                    }



                };
            game.UpdateEvent +=
                delegate
                {
                    lodEngine.Update(game);
                };
            game.DrawEvent +=
                delegate
                    {
                      
                    renderer.Render();
                };
            game.Run();
        }
        [Test]
        public void TestMultipleModelLodLayer()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;

            TWRenderer renderer = new TWRenderer(game);
            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(10);
            TreeLodEngine lodEngine = new TreeLodEngine();
            TreeLodEntity treeLodEntity;

            ModelLodLayer layer = new ModelLodLayer(0, renderer, gen, 50);
            game.InitializeEvent +=
                delegate
                {
                    lodEngine.AddITreeLodLayer(layer, 0);
                    lodEngine.AddITreeLodLayer(new ModelLodLayer(1, renderer, gen, 50), 10);
                    lodEngine.AddITreeLodLayer(new ModelLodLayer(2, renderer, gen, 50), 20);

                    treeLodEntity = lodEngine.CreateTreeLodEntity(TreeStructure.GetTestTreeStructure(game));
                    treeLodEntity.WorldMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 0));

                    treeLodEntity = lodEngine.CreateTreeLodEntity(TreeStructure.GetTestTreeStructure(game));
                    treeLodEntity.WorldMatrix = Matrix.CreateTranslation(new Vector3(4, 0, 3));

                    treeLodEntity = lodEngine.CreateTreeLodEntity(TreeStructure.GetTestTreeStructure(game));
                    treeLodEntity.WorldMatrix = Matrix.CreateTranslation(new Vector3(8, 0, 8));

                    treeLodEntity = lodEngine.CreateTreeLodEntity(TreeStructure.GetTestTreeStructure(game));
                    treeLodEntity.WorldMatrix = Matrix.CreateTranslation(new Vector3(12, 0, 20));
                };
            game.UpdateEvent +=
                delegate
                {
                    lodEngine.Update(game);
                };
            game.DrawEvent +=
                delegate
                {
                    renderer.Render();
                };
            game.Run();
        }

        [Test]
        public void TestImposterLodLayer()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;

            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(10);

            TreeLodEntity treeLodEntity;
            ImposterLodLayer layer = null;
            game.InitializeEvent +=
                delegate
                {
                    layer = new ImposterLodLayer(gen, game, 128, 2, 0.5f, 4f, 4);

                    var structure = TreeStructure.GetTestTreeStructure(game);
                    var ent = new TreeLodEntity(structure);
                    ent.WorldMatrix = Matrix.CreateTranslation(new Vector3(10, 0, 10));

                    layer.AddEntity(ent);


                };
            game.UpdateEvent += () => layer.Update();
            game.DrawEvent +=
                delegate
                    {
                        layer.RenderImpostersLod(); layer.Render(); };
            game.Run();
        }
        [Test]
        public void TestImposterLodLayerMultipleTrees()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;

            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(10);

            TreeLodEntity treeLodEntity;
            ImposterLodLayer layer = null;
            var structure = TreeStructure.GetTestTreeStructure(game);
            Seeder seeder = new Seeder(654);
            List<TreeLodEntity> entities = new List<TreeLodEntity>();
            game.InitializeEvent +=
                delegate
                {
                    layer = new ImposterLodLayer(gen, game, 64, 10, 0.5f, 4f, 4);


                    var ent = new TreeLodEntity(structure);
                    ent.WorldMatrix = Matrix.CreateTranslation(new Vector3(10, 0, 10));
                    layer.AddEntity(ent);
                    entities.Add(ent);

                };
            game.UpdateEvent +=
                delegate
                {

                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.A))
                    {
                        var ent = new TreeLodEntity(structure);
                        ent.WorldMatrix = Matrix.CreateTranslation(seeder.NextVector3(Vector3.Zero, new Vector3(20, 0, 20)));
                        layer.AddEntity(ent);
                        entities.Add(ent);
                    }
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.R))
                    {
                        int k = seeder.NextInt(0, entities.Count - 1);
                        layer.RemoveEntity(entities[k]);
                    }
                    layer.Update();
                };
            game.DrawEvent += delegate
            {
                layer.RenderImpostersLod(); layer.Render();
            };
            game.Run();
        }
        [Test]
        public void TestImposterAndModelLodLayerTest()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;
            game.SpectaterCamera.NearClip = 1f;
            game.SpectaterCamera.FarClip = 20000;

            TWRenderer renderer = new TWRenderer(game);
            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(10);
            TreeLodEngine lodEngine = new TreeLodEngine();
            TreeLodEntity treeLodEntity;

            ImposterLodLayer layer = null;
            ImposterLodLayer layer2 = null;
            TreeStructure treeStruct = TreeStructure.GetTestTreeStructure(game);
            SimplePlaneMesh plane = null;
            game.InitializeEvent +=
                delegate
                {
                    plane = new SimplePlaneMesh();
                    plane.WorldMatrix = Matrix.Identity;
                    plane.Width = 20000;
                    plane.Height = 20000;
                    plane.Color = Color.Beige;
                    plane.Initialize(game);


                    layer = new ImposterLodLayer(gen, game, 256, 8, 0.5f, 4f, 5);
                    layer2 = new ImposterLodLayer(gen, game, 128, 16, 0.8f, 4.5f, 5);
                    lodEngine.AddITreeLodLayer(new ModelLodLayer(0, renderer, gen, 100), 0);
                    lodEngine.AddITreeLodLayer(new ModelLodLayer(1, renderer, gen, 200), 20);
                    lodEngine.AddITreeLodLayer(new ModelLodLayer(2, renderer, gen, 300), 60);
                    lodEngine.AddITreeLodLayer(layer, 100);
                    lodEngine.AddITreeLodLayer(layer2, 250);
                    Seeder seeder = new Seeder(123);
                    for (int i = 0; i < 500; i++)
                    {
                        treeLodEntity = lodEngine.CreateTreeLodEntity(treeStruct);
                        treeLodEntity.WorldMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, seeder.NextFloat(0, MathHelper.TwoPi)) * Matrix.CreateTranslation(seeder.NextVector3(new Vector3(-350, 0, 0 - 350), new Vector3(350, 0, 350)));
                    }



                };
            game.UpdateEvent +=
                delegate
                {
                    plane.Update();
                    lodEngine.Update(game);
                    layer.Update();
                    layer2.Update();
                };
            game.DrawEvent +=
                delegate
                {
                    layer.RenderImpostersLod();
                    layer2.RenderImpostersLod();
                    GraphicsDevice device = game.GraphicsDevice;
                    RenderState renderState = device.RenderState;

                    device.RenderState.PointSpriteEnable = false;


                    device.RenderState.CullMode = CullMode.None;
                    device.RenderState.AlphaTestEnable = false;
                    device.RenderState.AlphaBlendEnable = false;
                    game.SetCamera(game.SpectaterCamera);
                    plane.Render();
                    layer2.Render();
                    layer.Render();
                    renderer.Render();

                };
            game.Run();
        }
        [Test]
        public void TestImposterRingLayer()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;

            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(10);

            TreeLodEntity treeLodEntity;
            ImposterRingLayer layer = null;
            game.InitializeEvent +=
                delegate
                {
                    layer = new ImposterRingLayer(100, gen);

                    var structure = TreeStructure.GetTestTreeStructure(game);
                    var ent = new TreeLodEntity(structure);
                    ent.WorldMatrix = Matrix.CreateTranslation(new Vector3(100, 0, 100));
                    Vector3 radius = new Vector3(2048 * 5, 4000, 2048 * 5);
                    layer.initialize(game, new BoundingBox(-radius, radius), 4);
                    layer.AddEntity(ent);


                };
            game.UpdateEvent += () => layer.Update();
            game.DrawEvent += () => layer.Render();
            game.Run();
        }
        [Test]
        public void TestImposterAndModelLodLayerAndImposterRingLayerTest()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;
            game.SpectaterCamera.NearClip = 1f;
            game.SpectaterCamera.FarClip = 20000;

            TWRenderer renderer = new TWRenderer(game);
            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(10);
            TreeLodEngine lodEngine = new TreeLodEngine();
            TreeLodEntity treeLodEntity;

            ImposterLodLayer layer = null;
            ImposterLodLayer layer2 = null;
            ImposterRingLayer layer3 = null;
            TreeStructure treeStruct = TreeStructure.GetTestTreeStructure(game);
            SimplePlaneMesh plane = null;
            game.InitializeEvent +=
                delegate
                {
                    plane = new SimplePlaneMesh();
                    plane.WorldMatrix = Matrix.Identity;
                    plane.Width = 20000;
                    plane.Height = 20000;
                    plane.Color = Color.Beige;
                    plane.Initialize(game);


                    layer = new ImposterLodLayer(gen, game, 256, 10, 0.5f, 4f, 5);
                    layer2 = new ImposterLodLayer(gen, game, 128, 25, 0.8f, 4.5f, 5);
                    layer3 = new ImposterRingLayer(150, gen);
                    Vector3 radius = new Vector3(2048 * 5, 4000, 2048 * 5);
                    layer3.initialize(game, new BoundingBox(-radius, radius), 4);
                    lodEngine.AddITreeLodLayer(new ModelLodLayer(0, renderer, gen, 100), 0);
                    lodEngine.AddITreeLodLayer(new ModelLodLayer(1, renderer, gen, 200), 20);
                    lodEngine.AddITreeLodLayer(new ModelLodLayer(2, renderer, gen, 300), 60);
                    lodEngine.AddITreeLodLayer(layer, 100);
                    lodEngine.AddITreeLodLayer(layer2, 150);
                    lodEngine.AddITreeLodLayer(layer3, 170);
                    Seeder seeder = new Seeder(123);
                    for (int i = 0; i < 5000; i++)
                    {
                        treeLodEntity = lodEngine.CreateTreeLodEntity(treeStruct);
                        treeLodEntity.WorldMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, seeder.NextFloat(0, MathHelper.TwoPi)) * Matrix.CreateTranslation(seeder.NextVector3(new Vector3(0, 0, 0), new Vector3(1000, 0,1000)));
                    }



                };
            game.UpdateEvent +=
                delegate
                {
                    plane.Update();
                    lodEngine.Update(game);
                    layer.Update();
                    layer2.Update();
                    layer3.Update();
                };
            game.DrawEvent +=
                delegate
                {
                    layer.RenderImpostersLod();
                    layer2.RenderImpostersLod();
                    
                    GraphicsDevice device = game.GraphicsDevice;
                    RenderState renderState = device.RenderState;

                    device.RenderState.PointSpriteEnable = false;


                    device.RenderState.CullMode = CullMode.None;
                    device.RenderState.AlphaTestEnable = false;
                    device.RenderState.AlphaBlendEnable = false;
                    game.SetCamera(game.SpectaterCamera);
                    plane.Render();
                    layer3.Render();
                    layer2.Render();
                    layer.Render();
                    renderer.Render();

                };
            game.Run();
        }
       [Test]
        public void TestTempLeafSize()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;
            game.SpectaterCamera.NearClip = 1f;
            game.SpectaterCamera.FarClip = 20000;

            TWRenderer renderer = new TWRenderer(game);
            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(10);
            TreeLodEngine lodEngine = new TreeLodEngine();
            TreeLodEntity treeLodEntity;

            TreeStructure treeStruct = TreeStructure.GetTestTreeStructure(game);
            game.InitializeEvent +=
                delegate
                {


                    lodEngine.AddITreeLodLayer(new ModelLodLayer(0, renderer, gen, 50), 0);
                    Seeder seeder = new Seeder(123);
                    for (int i = 0; i < 20; i++)
                    {
                        treeLodEntity = lodEngine.CreateTreeLodEntity(treeStruct);
                        treeLodEntity.WorldMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, seeder.NextFloat(0, MathHelper.TwoPi)) * Matrix.CreateTranslation(seeder.NextVector3(new Vector3(-350, 0, 0 - 350), new Vector3(350, 0, 350)));
                    }



                };
            game.UpdateEvent +=
                delegate
                {
                    lodEngine.Update(game);
                };
            game.DrawEvent +=
                delegate
                {
                    GraphicsDevice device = game.GraphicsDevice;
                    RenderState renderState = device.RenderState;

                    device.RenderState.PointSpriteEnable = false;


                    device.RenderState.CullMode = CullMode.None;
                    device.RenderState.AlphaTestEnable = false;
                    device.RenderState.AlphaBlendEnable = false;
                    game.SetCamera(game.SpectaterCamera);
                    renderer.Render();

                };
            game.Run();
        }
        [Test]
        public void TestRenderDepthTree()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;


            EngineTreeRenderData renderData;
            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(10);
            renderData = gen.GetRenderData(TreeStructure.GetTestTreeStructure(game), game, 0);

            TWRenderElement renderElement;
            DirectionalLight light = null;
            BasicShader depthShader, modelShader;
            game.InitializeEvent +=
                delegate
                {

                    light = new DirectionalLight();
                    light.Direction = new Vector3(0.3f, -0.8f, 0.3f);
                    light.Direction.Normalize();
                    light.Color = Color.Red.ToVector3();
                    int width = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
                    int height = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
                    renderData.Initialize();

                };
            game.UpdateEvent +=
                delegate
                {

                };
            game.DrawEvent +=
                delegate
                {
                    renderData.RenderLinearDepth();
                    //game.LineManager3D.AddBox(BoundingBox.CreateFromPoints(renderData.BoundingBoxData), Color.Red);
                };
            game.Run();
        }
       [Test]
        public void TestShadowedTree()
        {
            XNAGame game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;


            EngineTreeRenderData renderData;
            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(10);
            renderData = gen.GetRenderData(TreeStructure.GetTestTreeStructure(game), game, 0);


            CSMRenderer CsmRenderer = null;
            DirectionalLight light = null;
            SpectaterCamera cam = null;
            RenderTarget2D renderTarget = null;
            bool goSpectator = false;
            game.InitializeEvent +=
                delegate
                {
                    CsmRenderer = new CSMRenderer(game);
                    
                    cam = new SpectaterCamera(game);
                    game.SpectaterCamera.FarClip = 1000;
                    cam.Enabled = false;
                    light = new DirectionalLight();
                    light.Direction = new Vector3(0.3f, -0.8f, 0.3f);
                    light.Direction.Normalize();
                    light.Color = Color.Red.ToVector3();
                    int width = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
                    int height = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
                    renderTarget = new RenderTarget2D(game.GraphicsDevice, width, height, 1, SurfaceFormat.Single, RenderTargetUsage.DiscardContents);
                    renderData.Initialize();

                };
            game.UpdateEvent +=
                delegate
                {
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.C))
                    {
                        goSpectator = !goSpectator;

                        cam.Enabled = goSpectator;
                        game.SpectaterCamera.Enabled = !goSpectator;

                        if (goSpectator)
                            game.SetCamera(cam);
                        else
                            game.SetCamera(game.SpectaterCamera);

                    }

                };
            game.DrawEvent +=
                delegate
                {

                    game.GraphicsDevice.SetRenderTarget(0, renderTarget);
                    renderData.RenderLinearDepth();
                    game.GraphicsDevice.SetRenderTarget(0, null);
                    CsmRenderer.ShowCascadeSplits = true;
                    CsmRenderer.RenderDebug = true;
                    CsmRenderer.Enabled = true;
                    CsmRenderer.Render(
                    delegate(IXNAGame game1, BasicShader effect)
                    {
                        
                        effect.SetParameter("g_matWorld", Matrix.Identity);
                        //effect.SetParameter("g_matViewProj", cam.ViewProjection);
                        effect.effect.CommitChanges();
                        renderData.RenderPrimitives();
                    }, renderTarget, light, cam);

                    renderData.Render(game);
                    game.SpriteBatch.Begin();
                    game.SpriteBatch.Draw(CsmRenderer.shadowMap.GetTexture(), new Rectangle(0, 0, 128 * 4, 128), Color.White);
                    game.SpriteBatch.End();

                };
            game.Run();
        }
    }
}
