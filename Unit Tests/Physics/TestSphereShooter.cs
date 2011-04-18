using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Tests.Physics
{
    public class TestSphereShooter : IXNAObject
    {
        private readonly PhysicsEngine engine;
        private readonly ClientPhysicsQuadTreeNode root;
        List<ClientPhysicsTestSphere> spheres = new List<ClientPhysicsTestSphere>();
        private SphereMesh sphereMesh;

        private XNAGame game;

        public TestSphereShooter(XNAGame game, PhysicsEngine engine, ClientPhysicsQuadTreeNode root)
        {
            this.game = game;
            this.engine = engine;
            this.root = root;
            sphereMesh = new SphereMesh(0.3f, 20, Color.Green);

        }

        public void Initialize(IXNAGame _game)
        {
            sphereMesh.Initialize(game);

        }

        public void Render(IXNAGame _game)
        {

            for (int i = 0; i < spheres.Count; i++)
            {
                sphereMesh.WorldMatrix = Matrix.CreateTranslation(spheres[i].Center);
                sphereMesh.Render(game);
            }
        }

        public void Update(IXNAGame _game)
        {
            sphereMesh.Update(game);
            if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
            {
                var iSphere = new ClientPhysicsTestSphere(engine.Scene,
                    game.SpectaterCamera.CameraPosition + game.SpectaterCamera.CameraDirection
                    , 0.3f);

                iSphere.InitDynamic();
                iSphere.Actor.LinearVelocity = game.SpectaterCamera.CameraDirection * 10;

                spheres.Add(iSphere);
            }



            for (int i = 0; i < spheres.Count; i++)
            {
                spheres[i].Update(root, game);
            }
        }
    }
}
