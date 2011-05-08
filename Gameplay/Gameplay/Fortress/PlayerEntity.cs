using System;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Scripting.API;
using PlayerController = MHGameWork.TheWizards.GamePlay.PlayerController;

namespace MHGameWork.TheWizards.Gameplay.Fortress
{
    public class PlayerEntity : IScript, IUpdateHandler
    {
        private PlayerSceneComponent psc;
        private PlayerController controller;


        public void Init(IEntityHandle handle)
        {
            handle.RegisterUpdateHandler();

            psc = handle.GetSceneComponent<PlayerSceneComponent>();

            controller = psc.CreateController(new PlayerData());
        }

        public void Destroy()
        {
        }

        public void Update()
        {
            if (game.Keyboard.IsKeyDown(Keys.Z))
            {
                controller.DoMoveForward(game.Elapsed);
            }
            if (game.Keyboard.IsKeyDown(Keys.S))
            {
                controller.DoMoveBackwards(game.Elapsed);
            }
            if (game.Keyboard.IsKeyDown(Keys.Q))
            {
                controller.DoStrafeLeft(game.Elapsed);
            }
            if (game.Keyboard.IsKeyDown(Keys.D))
            {
                controller.DoStrafeRight(game.Elapsed);
            }
            if (game.Keyboard.IsKeyPressed(Keys.Space))
            {
                controller.DoJump();
            }
        }
    }
}
