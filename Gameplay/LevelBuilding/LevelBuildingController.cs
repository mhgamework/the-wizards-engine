using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.LevelBuilding
{


    public class LevelBuildingController
    {
        private readonly CameraInfo camera;
        private ILevelBuildingObject activeObject;
        private ScalableGrid grid;
        private LevelData data;

        public LevelBuildingController(CameraInfo camera)
        {
            this.camera = camera;
            grid = new ScalableGrid();
        }

        public void Update()
        {
            Ray ray = camera.GetCenterScreenRay();
            Vector3 cPos = grid.GetSelectedPosition(ray);

            if (activeObject != null)
            {
                activeObject.Position = cPos;
            }

            if (TW.Game.Mouse.LeftMouseJustPressed && activeObject != null)
            {
                data.AddLevelBuildingObject(activeObject);
                activeObject = null;
            }

            if (TW.Game.Mouse.RightMouseJustPressed && activeObject != null)
            {
                activeObject.Rotation = activeObject.Rotation *
                                        Quaternion.RotationAxis(Vector3.UnitY, (float)Math.PI * 0.5f);
            }

            if (TW.Game.Mouse.LeftMouseJustPressed && activeObject == null)
            {
                ILevelBuildingObject selected; //TODO: change to get raycasted object
                if (selected == null)
                    return;

                if (TW.Game.Keyboard.IsKeyDown(Key.C))
                {
                    activeObject = selected.Clone();
                }
                else
                {
                    //activeobject = object at cPos, remove this object from data
                }
                //if numkey pressed, set this object to quickslot
            }

            //numkeys => quickslots
            //numkey + '+' or '-' => scroll through available tiles for quickslot

        }
    }
}
