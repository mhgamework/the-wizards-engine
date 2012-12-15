using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.Trigger
{
    [ModelObjectChanged]
    public class PlayerPositionCondition : EngineModelObject, ICondition
    {
        private int type;
        private bool inverted;

        private readonly PlayerData player;
        public BoundingBox BoundingBox { get; set; }
        private WireframeBox box;

        private bool isSatisfied;
        private bool hasBeenSatisfied;

        public PlayerPositionCondition()
        {
            
        }
        public PlayerPositionCondition(PlayerData player, BoundingBox bb)
        {
            this.player = player;
            this.BoundingBox = bb;

            box = new WireframeBox();
            box.Color = new Color4(1, 1, 1);
            box.FromBoundingBox(bb);
            box.Visible = false;
        }

        public bool IsSatisfied()
        {
            if (box.Visible == false)
                box.Visible = true;

            reCheck();

            if (type == ConditionType.ONCE && hasBeenSatisfied)
                return !inverted; //true in not-inverted case
            if (type == ConditionType.SWITCH && isSatisfied)
                return !inverted;

            return inverted; //false in not-inverted case
        }

        public void SetType(int conditionType)
        {
            type = conditionType;

            if (type == ConditionType.SWITCH)
                box.Color = new Color4(0, 0, 1);
            if (type == ConditionType.ONCE)
                box.Color = new Color4(1, 0, 0);
        }

        public void Invert()
        {
            inverted = true;
        }

        private void reCheck()
        {
            var pPos = player.Position;

            if (isInBetween(pPos.X, BoundingBox.Minimum.X, BoundingBox.Maximum.X) && isInBetween(pPos.Y, BoundingBox.Minimum.Y, BoundingBox.Maximum.Y) && isInBetween(pPos.Z, BoundingBox.Minimum.Z, BoundingBox.Maximum.Z))
            {
                isSatisfied = true;
                hasBeenSatisfied = true;
            }
            else
                isSatisfied = false;
        }

        private bool isInBetween(float val, float a, float b)
        {
            if (a < b)
            {
                return val >= a && val <= b;
            }
            if (a > b)
            {
                return val >= b && val <= a;
            }

            return val == a; //a == b in this case
        }
    }
}
