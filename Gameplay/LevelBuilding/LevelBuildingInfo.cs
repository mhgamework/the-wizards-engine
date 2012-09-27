using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.WorldRendering;

namespace MHGameWork.TheWizards.LevelBuilding
{
    public class LevelBuildingInfo
    {
        public readonly CameraInfo Camera;
        public readonly ScalableGrid Grid;
        public LevelBuildingData LevelBuildingData {get; private set;}

        public LevelBuildingInfo(CameraInfo camera, ScalableGrid grid)
        {
            this.Camera = camera;
            this.Grid = grid;
            LevelBuildingData = new LevelBuildingData();
        }
    }
}
