using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Trigger;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.LevelBuilding
{
    public class LevelBuildingTriggerObjectType : ILevelBuildingObjectType
    {

        public void ProcessInput(LevelBuildingObjectFactory factory, LevelBuildingInfo info)
        {
            var temp = getSelectedObject(factory, info);
            if (!CanHandleObject(temp))
                return;

            var selected = (TriggerObject)temp;
            
            var translation = getPlacePos(info);
            selected.SetPosition(Matrix.Translation(translation));

            if (TW.Graphics.Mouse.LeftMouseJustPressed)
            {
                info.SelectedObject = null;
            }

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.X))
            {
                factory.DeleteObject(selected);
                info.SelectedObject = null;
            }

            //updateTextArea(selected, info.Textarea);
        }

        private void updateTextArea(TriggerObject selected, Textarea textarea)
        {
            var t = selected.Trigger;

            textarea.Text = "TRIGGER";

            Vector3 scale;
            Quaternion rotation;
            Vector3 translation;
            selected.GetPosition().Decompose(out scale, out rotation, out translation);
            textarea.Text += "\nPosition: (" + translation.X + ", " + translation.Y + ", " + translation.Z + ")";

            textarea.Text += "\nType: ";
            if (t.IsOr())
                textarea.Text += "OR";
            else
                textarea.Text += "AND";

            textarea.Text += "\n Inverted: ";
            if (t.IsInverted())
                textarea.Text += "true";
            else
                textarea.Text += "false";

            textarea.Text += "\nConditions:";
            foreach(ICondition c in t.Conditions)
            {
                textarea.Text += "\n- " + c.ToString();
            }

            textarea.Text += "\nActions:";
            foreach (IAction a in t.Actions)
            {
                textarea.Text += "\n- " + a.ToString();
            }
        }

        public object GetNewObject()
        {
            return new TriggerObject();
        }

        public void Delete(object o)
        {
            if (!CanHandleObject(o))
                throw new Exception("Invalid Argument");

            ((TriggerObject)o).Entity.Mesh = null;
        }

        public bool CanHandleObject(object o)
        {
            return o is TriggerObject;
        }

        private Vector3 getPlacePos(LevelBuildingInfo info)
        {
            Ray ray = info.Camera.GetCenterScreenRay();
            Vector3 cPos = info.Grid.GetSelectedPosition(ray);

            return cPos;
        }

        private object getSelectedObject(LevelBuildingObjectFactory factory, LevelBuildingInfo info)
        {
            var s = info.SelectedObject;
            if (s == null || !CanHandleObject(s))
            {
                s = factory.CreateFromType(this);
                info.SelectedObject = s;
            }

            return s;
        }
    }
}
