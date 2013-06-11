using System;
using System.Linq;
using MHGameWork.TheWizards.Audio;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Building;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Cannons
{
    [ModelObjectChanged]
    public class Cannon : EngineModelObject,IPhysical,IBuildable
    {
        public Vector3 Position { get; set; }
        public float Angle { get; set; }
        public float NextShot { get; set; }
        public float ShootInterval { get; set; }

        public SoundEmitter Sound { get; set; }

        public Cannon()
        {
            ShootInterval = 4;
            Sound = new SoundEmitter();
            Buildable = new BuildablePart();
            Buildable.Parent = this;
            Physical = new Physical();

            Buildable.RequiredResources.Add(ResourceFactory.Get.Cannonball);
            Buildable.RequiredResources.Add(ResourceFactory.Get.Stone);
            Buildable.RequiredResources.Add(ResourceFactory.Get.Stone);
            Buildable.RequiredResources.Add(ResourceFactory.Get.Stone);
            Buildable.RequiredResources.Add(ResourceFactory.Get.Stone);
            Buildable.RequiredResources.Add(ResourceFactory.Get.Stone);
            Buildable.RequiredResources.Add(ResourceFactory.Get.Stone);

        }

        public void Update()
        {
            
            if (Sound.Sound == null)
            {
                Sound.Sound = SoundFactory.Load("RTS\\CannonShot.wav");
                Sound.Position = Position;
                Sound.Loop = false;
                Sound.Ambient = false;
            }

            var targetAngle = calculateTargetAngle();
            if (!reachedAngle(targetAngle))
                turnToTarget(targetAngle);
            else
                attemptFire();
        }

        private void attemptFire()
        {
            if (NextShot > TW.Graphics.TotalRunTime) return;
            resetShotInterval();
            Sound.Start();
            var g = findClosestGoblin();
            if (g != null) TW.Data.Objects.Remove(g);

        }

        private void resetShotInterval()
        {
            NextShot = TW.Graphics.TotalRunTime + ShootInterval;
        }

        private void turnToTarget(float targetAngle)
        {
            var speed = 2f;
            var diff = targetAngle - Angle;
            var add = Math.Sign(diff) * TW.Graphics.Elapsed * speed;
            if (Math.Abs(add) > Math.Abs(diff)) add = diff;
            Angle += add;
            resetShotInterval();
        }


        private bool reachedAngle(float targetAngle)
        {
            return Math.Abs(Angle - targetAngle) < 0.01;
        }

        private float calculateTargetAngle()
        {
            Goblin closest;
            float angle;
            closest = findClosestGoblin();
            if (closest == null)
                return Angle;

            var toTarget = closest.Position - Position;

            //toTarget =
            //TW.Data.GetSingleton<CameraInfo>().ActiveCamera.ViewInverse.xna().Translation.dx() -
            //Position;

            toTarget.Y = 0;
            toTarget.Normalize();

            var dot = Vector3.Dot(-Vector3.UnitZ, toTarget);
            var ret = (float)Math.Acos(dot);
            if (toTarget.X > 0) ret = -ret;

            return ret;

        }

        private Goblin findClosestGoblin()
        {
            return TW.Data.Objects.Where(g => g is Goblin)
                        .Cast<Goblin>()
                        .OrderBy(g => Vector3.Distance(g.Position, Position))
                        .FirstOrDefault();

        }

        public BuildablePart Buildable { get; set; }
        public Physical Physical { get; set; }
        public void UpdatePhysical()
        {
            var scale = Buildable.BuildProgress*0.9f + 0.1f;
            Physical.WorldMatrix = Matrix.Scaling(scale,scale,scale)* Matrix.RotationY(Angle) * Matrix.Translation(Position);
            Physical.Mesh = TW.Assets.LoadMesh("RTS\\Cannon");
        }
    }
}
