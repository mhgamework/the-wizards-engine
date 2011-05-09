using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Scene;
using MHGameWork.TheWizards.Scripting;
using PlayerController = MHGameWork.TheWizards.GamePlay.PlayerController;

namespace MHGameWork.TheWizards.Gameplay
{
    public class PlayerSceneComponent : IXNAObject
    {
        private readonly Scene.Scene scene;
        private PlayerController controller;
        private PlayerThirdPersonCamera thirdPersonCamera;

        public PlayerSceneComponent(Scene.Scene scene)
        {
            this.scene = scene;
            scene.AddSceneComponent(this);
        }

        public PlayerController CreateController(PlayerData playerData)
        {
            // ScriptLayer to be removed
            ScriptLayer.Scene = scene.PhysicsElementFactory.Engine.Scene;

            var c = new PlayerController(playerData);
            scene.Game.AddXNAObject(c);
            return c;
        }

        public PlayerThirdPersonCamera  EnablePlayerCamera(IPlayer player, PlayerController controller)
        {
            this.controller = controller;
            thirdPersonCamera = new PlayerThirdPersonCamera(scene.Game, player.GetData());
            scene.Game.SetCamera(thirdPersonCamera);
            thirdPersonCamera.Enabled = true;
            scene.Game.AddXNAObject(thirdPersonCamera);

            return thirdPersonCamera;
        }

        public void RaiseUseEvent(Scripting.API.IEntity entity, IPlayer player)
        {
            var ent = (APIEntity)entity;
            ent.Entity.RaisePlayerUse(player);

        }

        public void Initialize(IXNAGame _game)
        {

        }
        public void Render(IXNAGame _game)
        {
        }
        public void Update(IXNAGame _game)
        {
            if (controller == null) return;
            controller.HorizontalAngle = thirdPersonCamera.LookAngleHorizontal;

        }
    }
}
