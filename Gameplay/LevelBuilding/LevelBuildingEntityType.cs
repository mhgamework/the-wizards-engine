using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.LevelBuilding
{
    /// <summary>
    /// Responsible for placing and manipulating (basic) Entities in the world.
    /// </summary>
    public class LevelBuildingEntityType : ILevelBuildingObjectType
    {
        private IMesh mesh;
 
        public LevelBuildingEntityType(IMesh mesh)
        {
            this.mesh = mesh;
        }

        public void ProcessInput(LevelBuildingObjectFactory factory, LevelBuildingInfo info)
        {
            updateText(info.Textarea);

            var selected = getSelectedObject(factory, info);
            Vector3 scale;
            Quaternion rotation;
            Vector3 translation;
            selected.WorldMatrix.Decompose(out scale, out rotation, out translation);

            translation = getPlacePos(info);

            if (TW.Graphics.Mouse.RightMouseJustPressed)
            {
                rotation = rotation * Quaternion.RotationAxis(Vector3.UnitY, (float)Math.PI * 0.5f);
            }

            selected.WorldMatrix = Matrix.Scaling(scale)*Matrix.RotationQuaternion(rotation)*
                                   Matrix.Translation(translation);

            if (TW.Graphics.Mouse.LeftMouseJustPressed)
            {
                info.SelectedObject = null;
            }

            if(TW.Graphics.Keyboard.IsKeyPressed(Key.X))
            {
                factory.DeleteObject(selected);
                info.SelectedObject = null;
            }
        }

        private void updateText(Textarea t)
        {
            t.Text = "ENTITY-TYPE \nLeftClick: place \nRightClick: rotate \nX: delete";
        }

        public object GetNewObject()
        {
            var ent = new WorldRendering.Entity();
            ent.Mesh = mesh;
            ent.Solid = true;
            return ent;
        }

        public void Delete(object o)
        {
            if (!(o is WorldRendering.Entity))
                throw new Exception("Invalid argument!");

            var m = (WorldRendering.Entity) o;
            m.Mesh = null;
        }

        private WorldRendering.Entity getSelectedObject(LevelBuildingObjectFactory factory, LevelBuildingInfo info)
        {
            var s = info.SelectedObject;
            if (s == null || (factory.GetLevelBuildingTypeFromObject(s) != this))
            {
                s = factory.CreateFromType(this);
                info.SelectedObject = s;
            }

            return (WorldRendering.Entity) s;
        }

        private Vector3 getPlacePos(LevelBuildingInfo info)
        {
            Ray ray = info.Camera.GetCenterScreenRay();
            Vector3 cPos = info.Grid.GetSelectedPosition(ray);

            return cPos;
        }

    }
}
