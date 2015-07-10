using System.Collections.Generic;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Tests.Features.Simulation.Physics
{
    public class TestSphereShooter : IXNAObject
    {
        private readonly PhysicsEngine engine;
        private readonly ClientPhysicsQuadTreeNode root;
        private readonly ICamera shooterCamera;
        List<ClientPhysicsTestSphere> spheres = new List<ClientPhysicsTestSphere>();
        private SphereMesh sphereMesh;

        private IXNAGame game;

        public TestSphereShooter(IXNAGame game, PhysicsEngine engine, ClientPhysicsQuadTreeNode root, ICamera shooterCamera)
        {
            this.game = game;
            this.engine = engine;
            this.root = root;
            this.shooterCamera = shooterCamera;
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
                var pos = shooterCamera.ViewInverse.Translation;
                var dir = Vector3.Transform(Vector3.Forward, shooterCamera.ViewInverse) - pos;

                var iSphere = new ClientPhysicsTestSphere(engine.Scene,
                    pos + dir
                    , 0.3f);

                iSphere.InitDynamic();
                iSphere.Actor.LinearVelocity = dir * 10;

                spheres.Add(iSphere);
            }



            for (int i = 0; i < spheres.Count; i++)
            {
                spheres[i].Update(root, game);
            }
        }
    }
}
