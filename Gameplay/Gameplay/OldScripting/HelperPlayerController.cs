using System;
using MHGameWork.TheWizards._XNA.Gameplay;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Player;

namespace MHGameWork.TheWizards.Tests.Gameplay.OldScripting
{
    /// <summary>
    /// This is a TEST object. Used to easy testing code
    /// </summary>
    public class HelperPlayerController : IXNAObject
    {
        public PlayerController Controller { get; private set; }
        public PlayerThirdPersonCamera ThirdPersonCamera { get; private set; }

        public HelperPlayerController(IXNAGame game, PlayerData playerData)
        {
            Controller = new PlayerController(playerData);
            ThirdPersonCamera = new PlayerThirdPersonCamera(game, playerData);
            game.SetCamera(ThirdPersonCamera);
            ThirdPersonCamera.Enabled = true;
            game.AddXNAObject(ThirdPersonCamera);
            throw new NotImplementedException();
            //game.AddXNAObject(Controller);
        }

        public void Initialize(IXNAGame _game)
        {
        }

        public void Render(IXNAGame _game)
        {
        }

        public void Update(IXNAGame _game)
        {
            Controller.HorizontalAngle = ThirdPersonCamera.LookAngleHorizontal;

        }
    }
}
