using DirectX11.Graphics;
using MHGameWork.TheWizards._XNA.Gameplay;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Player
{
    /// <summary>
    /// This class is responsible for simulating the local player input
    /// </summary>
    public class LocalPlayerSimulator : ISimulator
    {
        private readonly PlayerData player;
        private PlayerController controller;
        private CameraInfo camInfo;

        public LocalPlayerSimulator(PlayerData player)
        {
            this.player = player;
            controller = new PlayerController(player);
            controller.Initialize(TW.Scene);

            camInfo = TW.Model.GetSingleton<CameraInfo>();

        }

        public void Simulate()
        {
            var game = TW.Game;


            if (camInfo.ActiveCamera is ThirdPersonCamera)
            {
                var cam = ((ThirdPersonCamera)camInfo.ActiveCamera);
                controller.HorizontalAngle = cam.LookAngleHorizontal;
              
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
