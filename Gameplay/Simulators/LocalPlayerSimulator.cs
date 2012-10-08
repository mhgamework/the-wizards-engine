using MHGameWork.TheWizards._XNA.Gameplay;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Simulators
{
    /// <summary>
    /// This class is responsible for simulating the local player input
    /// </summary>
    public class LocalPlayerSimulator : ISimulator
    {
        private readonly PlayerData player;
        private PlayerController controller;
        private CameraInfo camInfo;
        private WorldRendering.World world;

        public LocalPlayerSimulator(PlayerData player)
        {
            this.player = player;
            controller = new PlayerController(player);
            controller.Initialize(TW.Physics.Scene);

            camInfo = TW.Data.GetSingleton<CameraInfo>();

            world = TW.Data.GetSingleton<WorldRendering.World>();

        }

        public void Simulate()
        {
            var game = TW.Graphics;


            if (camInfo.ActiveCamera is ThirdPersonCamera)
            {
                var cam = ((ThirdPersonCamera)camInfo.ActiveCamera);
                controller.HorizontalAngle = cam.LookAngleHorizontal;



                var ray = Functions.CreateRayFromViewInverse(camInfo.ActiveCamera.ViewInverse.xna());

                ray.Direction = -ray.Direction;
                ray.Position = player.Position.xna();

                var result = world.Raycast(ray.dx(), o => o != player.Entity);

                cam.MaxDistance = 500;
                if (result.IsHit)
                    cam.MaxDistance = result.Distance - 0.001f;



            }


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

            controller.Update(game);


        }
    }
}
