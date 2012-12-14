using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.LevelBuilding;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.WorldRendering;
using Microsoft.Xna.Framework.Input;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Simulators
{
    public class LevelBuildingSimulator : ISimulator
    {
        private LevelBuildingController controller;
        private bool firstFrame;

        public LevelBuildingSimulator(PlayerData playerData, CameraInfo camera, LevelBuildingObjectFactory factory)
        {
            controller = new LevelBuildingController(playerData, camera, factory);
            firstFrame = true;
        }

        public void Simulate()
        {
            if (firstFrame)
            {
                firstFrame = false;
                return;
            }

            controller.Update();
        }
    }
}
