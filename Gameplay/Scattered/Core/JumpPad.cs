using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using MHGameWork.TheWizards.Scattered._Engine;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class JumpPad : IIslandAddon
    {
        private readonly Level level;

        private Vector3 padPos;

        private JumpPad targetJumpPad;
        public JumpPad TargetJumpPad { get { return targetJumpPad; } set { targetJumpPad = value; } }

        private Vector3 targetPos = new Vector3(50, 2, 0);

        private float timeTravelled;
        private float timeToTravel;

        private const float travelDuration = 3f;
        private const float g = 9.81f;


        public JumpPad(Level level, SceneGraphNode node)
        {
            this.level = level;
            Node = node;
            node.AssociatedObject = this;
            var mesh = TW.Assets.LoadMesh("Scattered\\Models\\JumpPad");

            var renderNode = node.CreateChild();
            level.CreateEntityNode(renderNode).Alter(c => c.Entity.Mesh = mesh)
                        .Alter(c => c.CreateInteractable(onInteract));
        }

        private void onInteract()
        {
            if (targetJumpPad == null)
                return;

            Quaternion r;
            Vector3 s;
            Node.Absolute.Decompose(out s, out r, out padPos);
            targetJumpPad.Node.Absolute.Decompose(out s, out r, out targetPos);
            targetPos += new Vector3(0, 2, 0);

            calcInitialSpeed(travelDuration);
            timeToTravel = travelDuration;
        }

        /*
        Fx = Vox*t + Ox;
        Fy = -0.5 * g * t * t + Voy*t + Oy;

        * Known values:
        P is the target point.
        O is the origin point.
         g is gravity.

        * Unknown values:
         Vo is Initial Velocity
         t is time needed to impact.
         we can set 't' with a fixed flight time -> 'duration'
         so:
               (Px-Ox)
        Vox = --------
               duration

                 Py + 0.5* g * duration * duration - Oy 
        Voy = ---------------------------------------
                          duration
         */

        private float v0x;
        private float v0y;

        private void calcInitialSpeed(float duration)
        {
            var dir = targetPos - padPos;
            dir.Y = 0;
            var xDist = dir.Length();

            v0x = xDist / duration;
            v0y = (targetPos.Y + 0.5f * g * duration * duration - padPos.Y) / duration;
        }

        private Vector3 getPosAtTime(float t)
        {
            var dir = targetPos - padPos;
            dir.Normalize();
            var fx = v0x * t;
            var fy = -0.5f * g * t * t + v0y * t;
            return new Vector3(dir.X * fx + padPos.X, fy + padPos.Y, dir.Z * fx + padPos.Z);
        }

        private void update()
        {
            if (timeToTravel > 0.001f)
            {
                timeToTravel -= TW.Graphics.Elapsed;
                timeTravelled += TW.Graphics.Elapsed;
                var newPos = getPosAtTime(timeTravelled);
                level.LocalPlayer.Position += (newPos - level.LocalPlayer.Position);

            }
            else
            {
                timeTravelled = 0f;
                timeToTravel = 0f;
            }

        }

        public SceneGraphNode Node { get; private set; }
        public void PrepareForRendering()
        {
            update();
        }
    }
}
