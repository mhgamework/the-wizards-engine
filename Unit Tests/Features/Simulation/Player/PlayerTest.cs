using System;
using MHGameWork.TheWizards.Tests.Features.Various.Client;
using MHGameWork.TheWizards._XNA.Gameplay;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Player.Client;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Simulation.Player
{
    [TestFixture]
    public class PlayerTest
    {


        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestPlayerClientPhysics()
        {
            throw new Exception();
            XNAGame game = new XNAGame();

            PhysicsEngine engine = new PhysicsEngine();

            PlayerData player = new PlayerData();
            player.Name = "MHGameWork";

            PlayerController controller = null;
            //PlayerInput input = null;
            PlayerClientPhysics playerClientPhysics = null;
            PlayerThirdPersonCamera playerCamera = new PlayerThirdPersonCamera(game, player);
            game.AddXNAObject(playerCamera);
            game.SetCamera(playerCamera);

            TheWizards.Client.ClientPhysicsQuadTreeNode tree = ClientTest.CreateTestClientPhysicsQuadtree();

            QuadTreeVisualizerXNA visualizer = new QuadTreeVisualizerXNA();

            game.InitializeEvent += delegate
             {

                 engine.Initialize(game);
                 PhysicsDebugRendererXNA debugRenderer = new PhysicsDebugRendererXNA(game, engine.Scene);

                 game.AddXNAObject(debugRenderer);

                 //controller = new PlayerController(game, player, engine.Scene);
                 //input = new PlayerInput(controller);
                 playerClientPhysics = new MHGameWork.TheWizards.Player.Client.PlayerClientPhysics(controller);



             };

            game.UpdateEvent += delegate
                {
                    engine.Update(game);
                    playerClientPhysics.Update(tree);

                    //input.Update(game);

                };


            game.DrawEvent +=
                delegate
                {

                    visualizer.RenderNodeGroundBoundig(game, tree,
                        delegate(ClientPhysicsQuadTreeNode node, out Color col)
                        {
                            col = Color.Green;

                            return node.PhysicsObjects.Count == 0;
                        });

                    visualizer.RenderNodeGroundBoundig(game, tree,
                       delegate(ClientPhysicsQuadTreeNode node, out Color col)
                       {
                           col = Color.Orange;

                           return node.PhysicsObjects.Count > 0;
                       });

                    visualizer.RenderNodeGroundBoundig(game, tree,
                       delegate(ClientPhysicsQuadTreeNode node, out Color col)
                       {
                           col = Color.Red;

                           return node.DynamicObjectsCount > 0;
                       });
                    //game.LineManager3D.AddCenteredBox(player.Position, 1, Color.Red);
                    //game.LineManager3D.AddLine(player.Position, player.Position + controller.GetForwardVector(), Color.Green);
                };

            game.Run();


        }

    }
}
