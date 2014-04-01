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
        public SceneGraphNode Node { get; private set; }
        public float MaxJumpDistance = 250f;
        public bool IsLaunchingPlayer;

        private readonly Level level;

        private Vector3 padPos;
        private JumpPad targetJumpPad;
        public JumpPad TargetJumpPad { get { return targetJumpPad; } set { targetJumpPad = value; updateTargetPos(); } }

        private Vector3 targetPos;
        private readonly SceneGraphNode landingNode;

        private float timeTravelled;
        private float timeToTravel;
        private const float minTravelDuration = 3f;
        private const float preferredSpeed = 50f;
        private float travelDuration;
        private float extraHeight;

        public bool IsPerformingJump { get { return timeToTravel > 0; } }


        public JumpPad(Level level, SceneGraphNode node)
        {
            this.level = level;
            Node = node;
            node.AssociatedObject = this;
            var mesh = TW.Assets.LoadMesh("Scattered\\Models\\JumpPad");

            var renderNode = node.CreateChild();
            level.CreateEntityNode(renderNode).Alter(c => c.Entity.Mesh = mesh)
                .Alter(c => c.CreateInteractable(onInteract));

            landingNode = node.CreateChild();
            landingNode.Relative = Matrix.Translation(new Vector3(0, 2f, 5f));

        }

        public Vector3 GetLandingCoordinates()
        {
            Vector3 s;
            Vector3 ret;
            Quaternion r;
            landingNode.Absolute.Decompose(out s, out r, out ret);
            return ret;
        }

        public static BoundingBox GetLocalBoundingBox()
        {
            return new BoundingBox(new Vector3(-3f, 0, -3f), new Vector3(3f, 1, 6));
        }

        public void CalculateTrajectorySettings()
        {
            if (targetJumpPad == null)
                return;

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
            if (Math.Abs(playerPos.Y - padPos.Y) > 10f)
                return false;

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

        public Vector3 GetPosAtTime(float t)
        {
            var time = Math.Min(t, travelDuration);
            var percent = time / travelDuration;

            var sinYHeight = (float)Math.Sin(percent * Math.PI) * yScaling;

            var xPos = time * xSpeed;
            var yPos = time * ySpeed + sinYHeight;

            var unitDir = new Vector3(targetPos.X, 0, targetPos.Z) - new Vector3(padPos.X, 0, padPos.Z);
            unitDir.Normalize();

            return new Vector3(padPos.X + xPos * unitDir.X, padPos.Y + yPos, padPos.Z + unitDir.Z * xPos);
        }

        private void updateTargetPos()
        {
            targetPos = targetJumpPad.GetLandingCoordinates();
        }

        private void update()
        {
            Quaternion r;
            Vector3 s;
            Node.Absolute.Decompose(out s, out r, out padPos);

            if (playerCurrentlyLaunchedByOtherPad())
                return;

            if (playerClose())
            {
                CalculateTrajectorySettings();
            }

            if (timeToTravel > 0.001f)
            {
                IsLaunchingPlayer = true;
                if (Vector3.Distance(targetPos, level.LocalPlayer.Position) < 1f)
                {
                    level.LocalPlayer.Position = targetPos;
                    timeToTravel = 0f;
                }
                else
                {
                    timeToTravel -= TW.Graphics.Elapsed;
                    timeTravelled += TW.Graphics.Elapsed;
                    var newPos = GetPosAtTime(timeTravelled);
                    level.LocalPlayer.Position = newPos;
                    //level.LocalPlayer.Position += (newPos - level.LocalPlayer.Position);
                }

                /*var dir = new Vector3(targetPos.X, 0, targetPos.Z) - new Vector3(padPos.X, 0, padPos.Z);
                dir.Normalize();
                var playerDir = level.LocalPlayer.Direction;
                var xlerp = MathHelper.Lerp(playerDir.X, dir.X, 0.2f);
                var zlerp = MathHelper.Lerp(playerDir.Z, dir.Z, 0.2f);
                var newDir = new Vector3(xlerp, playerDir.Y, zlerp);
                newDir.Normalize();
                level.LocalPlayer.Direction = newDir;*/
            }
            else
            {
                IsLaunchingPlayer = false;
                timeTravelled = 0f;
                timeToTravel = 0f;
            }

        }

        private bool playerCurrentlyLaunchedByOtherPad()
        {
            var allPads = level.Islands.SelectMany(i => i.Addons.OfType<JumpPad>());
            foreach (var pad in allPads)
            {
                if (pad == this)
                    continue;

                if (pad.IsLaunchingPlayer)
                    return true;
            }
            return false;
        }

        private bool selectingTarget;
        private List<JumpPadLocator> locators = new List<JumpPadLocator>();
        private void onInteract()
        {
            if (selectingTarget)
            {
                removeLocators();
                selectingTarget = false;
                return;
            }

            selectingTarget = true;
            var allpads = level.Islands.SelectMany(i => i.Addons.OfType<JumpPad>());
            foreach (var pad in allpads)
            {
                if (pad == this)
                    continue;

                Vector3 s;
                Quaternion r;
                Vector3 pos;
                pad.Node.Absolute.Decompose(out s, out r, out pos);

                var distance = Vector3.Distance(pos, padPos);
                if (distance > MaxJumpDistance)
                    continue;

                var dir = new Vector3(pos.X, 0, pos.Z) - new Vector3(padPos.X, 0, padPos.Z);
                dir.Normalize();
                dir *= 2f + (distance / MaxJumpDistance) * 2f;

                var inverse = Node.Absolute;
                inverse.Invert();
                var extraTransform = Matrix.Scaling(0.25f, 0.25f, 0.25f) *
                                     Matrix.Translation(dir + padPos + new Vector3(0, 1, 0)) * inverse;

                var locator = new JumpPadLocator(level, Node, extraTransform, this, pad, pad == targetJumpPad);
                locators.Add(locator);
            }
        }

        public void TargetPadPicked(JumpPad targetPad)
        {
            TargetJumpPad = targetPad;
            removeLocators();
            selectingTarget = false;
        }

        private void removeLocators()
        {
            foreach (var locator in locators)
            {
                locator.Dispose();
            }
            locators = new List<JumpPadLocator>();
        }

        public void PrepareForRendering()
        {
            update();
        }



        public class JumpPadLocator
        {
            private readonly Level level;
            private readonly JumpPad parentPad;
            private readonly JumpPad targetPad;
            private SceneGraphNode renderNode;

            public JumpPadLocator(Level level, SceneGraphNode node, Matrix extratransform, JumpPad parentPad, JumpPad targetPad, bool isCurrentlySelected)
            {
                this.level = level;
                this.parentPad = parentPad;
                this.targetPad = targetPad;

                var mesh = isCurrentlySelected ? TW.Assets.LoadMesh("Scattered\\Models\\JumpPadLocatorSelected") : TW.Assets.LoadMesh("Scattered\\Models\\JumpPadLocator");

                renderNode = node.CreateChild();
                renderNode.Relative = extratransform;

                level.CreateEntityNode(renderNode).Alter(c => c.Entity.Mesh = mesh)
                    .Alter(c => c.CreateInteractable(onInteract));
            }

            private void onInteract()
            {
                parentPad.TargetPadPicked(targetPad);
            }

            public void Dispose()
            {
                level.DestroyNode(renderNode);
                renderNode = null;
            }
        }
    }
}
