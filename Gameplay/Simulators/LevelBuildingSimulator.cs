using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.LevelBuilding;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.WorldRendering;
using Microsoft.Xna.Framework.Input;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Simulators
{
    public class LevelBuildingSimulator : ISimulator
    {
        private LevelBuildingController controller;

        public LevelBuildingSimulator(CameraInfo camera)
        {
            controller = new LevelBuildingController(camera);
        }

        public void Simulate()
        {
            controller.Update();
        }
    }
}
