using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Player;
using PlayerController = MHGameWork.TheWizards.GamePlay.PlayerController;

namespace MHGameWork.TheWizards.Gameplay
{
    public class PlayerSceneComponent
    {
        private readonly Scene.Scene scene;

        public PlayerSceneComponent (Scene.Scene scene)
        {
            this.scene = scene;
            scene.AddSceneComponent(this);
        }

        public PlayerController CreateController(PlayerData playerData)
        {
            var c = new PlayerController(playerData);
            scene.Game.AddXNAObject(c);
            return c;
        }
    }
}
