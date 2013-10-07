using System;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using NUnit.Framework;
using SlimDX;
using System.Linq;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Unused
{
    [TestFixture]
    [EngineTest]
    public class FlyingPrototypeTest
    {
        [ModelObjectChanged]
        private class Data : EngineModelObject
        {
            public Textarea Textarea;
            public void AddText(string text)
            {
                Textarea.Text += text + "\n";
            }
        }

        private TWEngine engine = EngineFactory.CreateEngine();

        private Island drivingIsland;
        private Island bridgeStart;

        private Data data;
        private float islandCollisionRadius = 20;

        [Test]
        public void TestFlying()
        {
            data = TW.Data.Get<Data>();
            TW.Graphics.SpectaterCamera.FarClip = 2000;
            DI.Get<TestSceneBuilder>().Setup = delegate
                {
                    setupData();

                    var s = new Seeder(123);

                    var scale = 5;

                    var range = new Vector3(100, 10, 100) * (float)Math.Sqrt(scale);

                    var velRange = new Vector3(1, 0, 1);

                    for (int i = 0; i < 20 * scale; i++)
                    {
                        var island = new Island();
                        island.Seed = s.NextInt(0, 10000);
                        island.Physical.SetPosition(s.NextVector3(-range.xna(), range.xna()).dx());
                        island.Velocity = s.NextVector3(-velRange.xna(), velRange.xna()).dx();
                        island.TargetHeight = island.Physical.GetPosition().Y;
                    }

                };

            engine.AddSimulator(new BasicSimulator(delegate
            {
                data.Textarea.Text = "";
                if (drivingIsland != null)
                {
                    data.AddText(drivingIsland.Velocity.ToString());
                }

                applyIslandWrap();
                applyIslandMovement();


                if (TW.Graphics.Keyboard.IsKeyReleased(Key.F))
                    onDriveButton();

                if (TW.Graphics.Keyboard.IsKeyReleased(Key.T))
                    onBridgeButton();

                driveIsland();
                applyIslandFloatationForce();
                applyIslandAirDrag();
                applyIslandShieldCollision();
                applyBridgePull();
                drawBridges();
            }));


            setupSims();
        }

      

        [Test]
        public void TestCollision()
        {
            data = TW.Data.Get<Data>();
            TW.Graphics.SpectaterCamera.FarClip = 2000;
            DI.Get<TestSceneBuilder>().Setup = setupData;
            var islands = TW.Data.Objects.OfType<Island>().ToArray();
            foreach (var i in islands) TW.Data.RemoveObject(i);
            var island = new Island();

            island.Seed = 1;
            island.Physical.SetPosition(new Vector3(20, 0, 0));
            island.Velocity = new Vector3(-2, 0, 0);

            island = new Island();
            island.Seed = 2;
            island.Physical.SetPosition(new Vector3(-20, 0, 0));
            island.Velocity = new Vector3(2, 0, 0);

            engine.AddSimulator(new BasicSimulator(delegate
            {
                applyIslandMovement();

                applyIslandShieldCollision();
            }));



            setupSims();
        }

        private void setupData()
        {
            data.Textarea = new Textarea()
                {
                    Position = new Vector2(10, 10),
                    Size = new Vector2(300, 200)
                };
        }


        private void setupSims()
        {


            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        private void applyIslandFloatationForce()
        {
            foreach (var i in TW.Data.Objects.OfType<Island>())
            {
                var diff = i.Physical.GetPosition().Y - i.TargetHeight;
                i.ApplyForce(-diff * Vector3.UnitY);
                i.ApplyForce(-0.1f * Vector3.UnitY * Vector3.Dot(Vector3.UnitY, i.Velocity));
            }

        }

        private static void applyIslandMovement()
        {
            foreach (var i in TW.Data.Objects.OfType<Island>())
            {
                // Move and wrap
                var oldPos = i.Physical.GetPosition();
                var newPos = i.Physical.GetPosition() + i.Velocity * TW.Graphics.Elapsed;
                i.Physical.SetPosition(newPos);

                //TW.Graphics.LineManager3D.AddCenteredBox(newPos, islandCollisionRadius, new Color4(0, 0, 0));
            }
        }

        private static void applyIslandWrap()
        {
            foreach (var i in TW.Data.Objects.OfType<Island>())
            {
                // Move and wrap
                var newPos = i.Physical.GetPosition();
                for (int j = 0; j < 3; j++)
                {
                    var worldRange = 2000;
                    if (newPos[j] > worldRange) newPos[j] = -worldRange;
                    if (newPos[j] < -worldRange) newPos[j] = worldRange;
                }


                i.Physical.SetPosition(newPos);
            }
        }

        private void applyIslandShieldCollision()
        {
            var coeff = 1f;
            foreach (var i in TW.Data.Objects.OfType<Island>())

                foreach (var j in TW.Data.Objects.OfType<Island>())
                {
                    if (i == j) continue;
                    var dir = j.Physical.GetPosition() - i.Physical.GetPosition();
                    var dist = dir.Length();

                    dir.Normalize();

                    if (dist > islandCollisionRadius) continue;
                    if (Vector3.Dot(i.Velocity, dir) < 0
                        && Vector3.Dot(j.Velocity, -dir) < 0) continue;

                    var speedAlongDir = Vector3.Dot(i.Velocity, dir) * dir - Vector3.Dot(j.Velocity, dir) * dir;
                    i.Velocity -= speedAlongDir;
                    j.Velocity += speedAlongDir;
                    //i.ApplyForce(-dir * coeff);
                    //j.ApplyForce(dir * coeff);

                }
        }

        /// <summary>
        /// F_D\, =\, \tfrac12\, \rho\, v^2\, C_D\, A
        /// </summary>
        private void applyIslandAirDrag()
        {
            var dragCoeff = 0.01f;
            foreach (var i in TW.Data.Objects.OfType<Island>())
            {
                // Short for -norm(vel) * len(vel) * len(vel) * coeff;
                i.ApplyForce(-i.Velocity * i.Velocity.Length() * dragCoeff);
            }
            // simplified drag equation (using velocity ^2)
        }

        private void applyBridgePull()
        {
            foreach (var b in TW.Data.Objects.OfType<Bridge>())
            {
                // assume islands equal weight
                var deltaX = b.CalculateCurrentLength() - b.InitialLength;

                //if (deltaX > 10) continue;
                if (deltaX < 0) continue;

                //deltaX = 10;
                var force = b.GetBridgeDirection();
                //force.Y = 0;
                var maxForce = 0.1f;
                if (force.Length() > maxForce)
                    force = force / force.Length() * maxForce;
                b.Start.ApplyForce(force * deltaX);
                b.End.ApplyForce(-force * deltaX);
            }
        }

        private void drawBridges()
        {
            var offset = new Vector3(0, 5, 0);

            foreach (var b in TW.Data.Objects.OfType<Bridge>())
            {
                var col = new Color4(0, 1, 0);
                if (b.CalculateCurrentLength() > b.InitialLength)
                    col = new Color4(1, 0, 0);

                TW.Graphics.LineManager3D.AddLine(b.Start.Physical.GetPosition() + offset, b.End.Physical.GetPosition() + offset, col);
            }

            var target = getTargetedIsland();
            if (bridgeStart != null && target != null)
            {
                TW.Graphics.LineManager3D.AddLine(bridgeStart.Physical.GetPosition() + offset, target.Physical.GetPosition() + offset, new Color4(1, 0, 0));
            }
        }

        private void driveIsland()
        {
            if (drivingIsland == null) return;

            var dir = new Vector3(0, 0, 0);
            if (TW.Graphics.Keyboard.IsKeyDown(Key.W)) dir += new Vector3(0, 0, -1);
            if (TW.Graphics.Keyboard.IsKeyDown(Key.A)) dir += new Vector3(-1, 0, 0);
            if (TW.Graphics.Keyboard.IsKeyDown(Key.S)) dir += new Vector3(0, 0, 1);
            if (TW.Graphics.Keyboard.IsKeyDown(Key.D)) dir += new Vector3(1, 0, 0);

            if (TW.Graphics.Keyboard.IsKeyDown(Key.C)) drivingIsland.Velocity = new Vector3();

            dir = -Vector3.TransformNormal(dir, Matrix.RotationY(-TW.Graphics.SpectaterCamera.AngleHorizontal));

            dir *= 5;

            drivingIsland.Velocity += dir * TW.Graphics.Elapsed;

            TW.Graphics.SpectaterCamera.CameraPosition = drivingIsland.Physical.GetPosition() +
                                                         new Vector3(0, 10, 0);
        }

        private void onBridgeButton()
        {

            // Try select
            var selectedIsland = getTargetedIsland();
            if (selectedIsland == null)
            {
                bridgeStart = null;
                return;
            }

            if (bridgeStart == null)
            {
                bridgeStart = selectedIsland;
                return;
            }

            var b = new Bridge()
                {
                    Start = bridgeStart,
                    End = selectedIsland
                };

            b.InitialLength = 30;// b.CalculateCurrentLength();

            bridgeStart = null;


        }

        private static Island getTargetedIsland()
        {
            var result = TW.Data.Get<Engine.WorldRendering.World>().Raycast(TW.Data.Get<CameraInfo>().GetCenterScreenRay());
            var selectedIsland = result.IsHit ? result.Object as Island : null;
            return selectedIsland;
        }

        private void onDriveButton()
        {
            // Try select
            drivingIsland = getTargetedIsland();
        }

        [ModelObjectChanged]
        public class Bridge : EngineModelObject
        {
            public Bridge()
            {

            }

            public Island Start { get; set; }
            public Island End { get; set; }
            public float InitialLength { get; set; }

            public float CalculateCurrentLength()
            {
                return Vector3.Distance(Start.Physical.GetPosition(), End.Physical.GetPosition());
            }
            public Vector3 GetBridgeDirection()
            {
                return Vector3.Normalize(End.Physical.GetPosition() - Start.Physical.GetPosition());
            }
        }
    }
}