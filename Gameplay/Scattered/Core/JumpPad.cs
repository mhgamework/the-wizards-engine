using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
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

        private const float minTravelDuration = 3f;
        private const float preferredSpeed = 50f;
        private float travelDuration;
        private float extraHeight;

        public JumpPad(Level level, SceneGraphNode node)
        {
            this.level = level;
            Node = node;
            node.AssociatedObject = this;
            var mesh = TW.Assets.LoadMesh("Scattered\\Models\\JumpPad");

            var renderNode = node.CreateChild();
            level.CreateEntityNode(renderNode).Alter(c => c.Entity.Mesh = mesh)
                /*.Alter(c => c.CreateInteractable(onInteract))*/;
        }

        private void launchPlayer()
        {
            if (targetJumpPad == null)
                return;

            var landingMat = Matrix.Translation(0, 0, 5f) * targetJumpPad.Node.Absolute;
            Quaternion r;
            Vector3 s;
            landingMat.Decompose(out s, out r, out targetPos);

            targetPos += new Vector3(0, 5f, 0);

            var xDist = Vector3.Distance(new Vector3(padPos.X, 0, padPos.Z), new Vector3(targetPos.X, 0, targetPos.Z));
            travelDuration = Math.Max(xDist / preferredSpeed, minTravelDuration);
            extraHeight = Math.Max(xDist / 50f * 20f, 20f);

            calcInitialSpeed(travelDuration);
            timeToTravel = travelDuration;
        }

        private bool playerClose()
        {
            var playerPos = level.LocalPlayer.Position;
            var dist = Vector3.Distance(new Vector3(playerPos.X, 0, playerPos.Z), new Vector3(padPos.X, 0, padPos.Z));
            return dist < 2f;
        }

        private float xSpeed;
        private float ySpeed;
        private float yScaling;
        private void calcInitialSpeed(float duration)
        {
            var xDist = Vector3.Distance(new Vector3(padPos.X, 0, padPos.Z), new Vector3(targetPos.X, 0, targetPos.Z));
            var yDist = targetPos.Y - padPos.Y;

            xSpeed = xDist / duration;
            ySpeed = yDist / duration;

            yScaling = Math.Max(targetPos.Y, padPos.Y) + extraHeight;
        }

        private Vector3 getPosAtTime(float t)
        {
            var percent = t / travelDuration;
            var sinYHeight = (float)Math.Sin(percent * Math.PI) * yScaling;

            var xPos = t * xSpeed;
            var yPos = t * ySpeed + sinYHeight;

            var unitDir = new Vector3(targetPos.X, 0, targetPos.Z) - new Vector3(padPos.X, 0, padPos.Z);
            unitDir.Normalize();

            return new Vector3(padPos.X + xPos * unitDir.X, padPos.Y + yPos, padPos.Z + unitDir.Z * xPos);
        }

        private void update()
        {
            Quaternion r;
            Vector3 s;
            Node.Absolute.Decompose(out s, out r, out padPos);

            if (playerClose())
            {
                launchPlayer();
            }

            if (timeToTravel > 0.001f)
            {
                timeToTravel -= TW.Graphics.Elapsed;
                timeTravelled += TW.Graphics.Elapsed;
                var newPos = getPosAtTime(timeTravelled);
                level.LocalPlayer.Position += (newPos - level.LocalPlayer.Position);

                var dir = new Vector3(targetPos.X, 0, targetPos.Z) - new Vector3(padPos.X, 0, padPos.Z);
                var playerDir = level.LocalPlayer.Direction;
                var xlerp = MathHelper.Lerp(playerDir.X, dir.X, 0.2f);
                var zlerp = MathHelper.Lerp(playerDir.Z, dir.Z, 0.2f);
                level.LocalPlayer.Direction = new Vector3(xlerp, playerDir.Y, zlerp);
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
