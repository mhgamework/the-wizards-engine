using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using MHGameWork.TheWizards._XNA.Gameplay;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Simulators
{
    /// <summary>
    /// This class is responsible for simulating the local player input
    /// TODO: split
    /// Uses Physx!!
    /// </summary>
    public class LocalPlayerSimulator : ISimulator
    {
        private readonly PlayerData player;
        private PlayerController controller;
        private CameraInfo camInfo;
        private Engine.WorldRendering.World world;

        private PointLight light = new PointLight();

        private PlayerClientPhysics clientPhysics;


        public LocalPlayerSimulator(PlayerData player)
        {
            this.player = player;
            controller = new PlayerController(player);
            controller.Initialize(TW.Physics.Scene);

            camInfo = TW.Data.GetSingleton<CameraInfo>();

            world = TW.Data.GetSingleton<Engine.WorldRendering.World>();

            clientPhysics = new PlayerClientPhysics(controller);



        }

        public void Simulate()
        {
            var game = TW.Graphics;


            processUserInput(game);

            controller.Update(game);

            var root = TW.Data.GetSingleton<PhysXData>().RootNode;
            if (root != null)
                clientPhysics.Update(TW.Data.GetSingleton<PhysXData>().RootNode);

            light.Position = player.Position + MathHelper.Up * 2;


            updateThirdPersonCamera(game);

        }

        private void processUserInput(GraphicsWrapper game)
        {
            if (game.Keyboard.IsKeyDown(Key.W))
            {
                controller.DoMoveForward(game.Elapsed);
            }
            if (game.Keyboard.IsKeyDown(Key.S))
            {
                controller.DoMoveBackwards(game.Elapsed);
            }
            if (game.Keyboard.IsKeyDown(Key.A))
            {
                controller.DoStrafeLeft(game.Elapsed);
            }
            if (game.Keyboard.IsKeyDown(Key.D))
            {
                controller.DoStrafeRight(game.Elapsed);
            }
            if (game.Keyboard.IsKeyPressed(Key.Space))
            {
                controller.DoJump();
            }
        }

        private void updateThirdPersonCamera(GraphicsWrapper game)
        {
            if (!(camInfo.ActiveCamera is ThirdPersonCamera)) return;
            var cam = ((ThirdPersonCamera)camInfo.ActiveCamera);
            controller.HorizontalAngle = cam.LookAngleHorizontal;


            var center = new Vector4(0f, 0f, 0, 1);
            var centerScreenWorld = Vector4.Transform(center, Matrix.Invert(cam.ViewProjection));
            centerScreenWorld *= (1 / centerScreenWorld.W);



            //var ray = Functions.CreateRayFromViewInverse(camInfo.ActiveCamera.ViewInverse.xna());
            var ray = new Ray();
            ray.Position = cam.CalculatedLookTarget;
            var worldfd = new Vector3(centerScreenWorld.X, centerScreenWorld.Y, centerScreenWorld.Z);
            ray.Direction = Vector3.Normalize(worldfd - ray.Position);

            //game.LineManager3D.AddCenteredBox(worldfd, 0.01f, new Color4(Color.Red));
            //game.LineManager3D.AddCenteredBox(cam.CalculatedLookTarget, 0.01f, new Color4(Color.Green));

            var result = world.Raycast(ray, o => o != player.Entity);

            cam.MaxDistance = 500;
            if (result.IsHit)
                cam.MaxDistance = result.Distance - 0.2f;
        }
    }
}
