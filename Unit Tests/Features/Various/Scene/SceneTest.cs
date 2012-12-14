﻿using System;
using MHGameWork.TheWizards.Client;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Various.Scene
{
    [TestFixture]
    public class SceneTest
    {
        [Test]
        public void TestRaycastScene()
        {
            var game = new TestTWGame();

            var scene = new _XNA.Scene.Scene(game.Renderer, game.PhysicsFactory);
            game.Game.AddXNAObject(scene);
            var ent = scene.CreateEntity();
            ent.Mesh = game.BarrelMesh;
            ent.Static = true;
            ent.Solid = true;

            game.PhysicsTreeRoot.AddDynamicObjectToIntersectingNodes(new ClientPhysicsTestSphere(Vector3.Zero, 100));

            game.Game.UpdateEvent += delegate
                                         {
                                             var result = scene.RaycastEntityPhysX(new Ray(Vector3.Backward * 5, Vector3.Forward), o => true);


                                             game.Game.LineManager3D.AddBox(result.ScriptingEntity.BoundingBox, Color.Red);


                                         };

            game.Game.Run();
        }

        [Test]
        public void TestContactHandler()
        {
            //TODO:

            throw new NotImplementedException();
        }
    }
}
