using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.WorldRendering;

namespace MHGameWork.TheWizards.LevelBuilding
{
    public class LevelBuildingInfo
    {
        public readonly CameraInfo Camera;
        public readonly ScalableGrid Grid;
        public Object SelectedObject;
        public Textarea Textarea;

        public LevelBuildingInfo(CameraInfo camera, ScalableGrid grid)
        {
            this.Camera = camera;
            this.Grid = grid;
            Textarea = new Textarea();
            Textarea.Color = new SlimDX.Color4(1, 1, 1);
            Textarea.Position = new SlimDX.Vector2(10, 500);
            Textarea.Size = new SlimDX.Vector2(500, 500);
        }
    }
}
