using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Model;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.World.Rendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Model
{
    [TestFixture]
    public class ModelTest
    {
        [Test]
        public void TestChangeEntity()
        {
            var container = new ModelContainer();

            var ent = new TheWizards.Model.Entity();
            container.AddObject(ent);

            ent.Mesh = new RAMMesh();

            int length;
            ModelContainer.ObjectChange[] array;
            container.GetEntityChanges(out array, out length);




            Assert.AreEqual(1, length);
            Assert.AreEqual(ent, array[0].ModelObject);
        }

        [Test]
        public void TestWorldRenderer()
        {
            var game = new DX11Game();
            game.InitDirectX();
            
            
            var container = new ModelContainer();

            var ent = new TheWizards.Model.Entity();
            container.AddObject(ent);

            var mesh = OBJParser.OBJParserTest.GetBarrelMesh(new TheWizards.OBJParser.OBJToRAMMeshConverter(new RAMTextureFactory()));

            var deferred = new DeferredRenderer(game);
            var renderer = new WorldRenderer(container, deferred);

            var light = deferred.CreateDirectionalLight();
            light.LightDirection = Vector3.Normalize(new Vector3(1, -1, 1));
            light.ShadowsEnabled = true;

            var time = 0f;

            game.GameLoopEvent += delegate
                                  {
                                      time += game.Elapsed;
                                      ent.Mesh = mesh;
                                      ent.WorldMatrix = Matrix.Translation(Vector3.UnitX*time);

                                      renderer.ProcessWorldChanges();
                                      container.ClearDirty();

                                      deferred.Draw();
                                  };

            game.Run();
        }

        [Test]
        public void TestPlayerMovement()
        {
            
        }
    }
}
