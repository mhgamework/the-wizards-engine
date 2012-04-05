using System;
using MHGameWork.TheWizards._XNA.Scene;
using MHGameWork.TheWizards._XNA.Scripting;
using MHGameWork.TheWizards._XNA.Scripting.API;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Player;

namespace MHGameWork.TheWizards._XNA.Gameplay
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
            throw new NotImplementedException(); // Old XNA code to be ported
            //scene.Game.AddXNAObject(c);
            return c;
        }

        public PlayerThirdPersonCamera EnablePlayerCamera(IPlayer player, PlayerController controller)
        {
            this.controller = controller;
            thirdPersonCamera = new PlayerThirdPersonCamera(scene.Game, player.GetData());
            scene.Game.SetCamera(thirdPersonCamera);
            thirdPersonCamera.Enabled = true;
            scene.Game.AddXNAObject(thirdPersonCamera);

            return thirdPersonCamera;
        }

        public void RaiseUseEvent(IEntity entity, IPlayer player)
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
