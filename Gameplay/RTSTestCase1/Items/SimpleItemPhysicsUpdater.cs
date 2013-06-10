using System;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.RTSTestCase1.Players;
using MHGameWork.TheWizards.RTSTestCase1._Common;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Items
{
    public class SimpleItemPhysicsUpdater
    {
        private readonly IWorldLocator worldLocator;

        private Random r = new Random(324);

        public SimpleItemPhysicsUpdater(IWorldLocator worldLocator)
        {
            this.worldLocator = worldLocator;
        }

        public void Simulate(float elapsed)
        {
            foreach (var o in TW.Data.Objects.OfType<IItem>())
            {
                var p = o as IPhysical;
                if (p == null) continue;

                var force = new Vector3();
                var radius = 0.7f;
                foreach (var close in worldLocator.AtPosition(p.Physical.GetPosition(), radius).Cast<IPhysical>())
                {
                    if (close == o) continue;

                    var dir = close.Physical.GetPosition() - p.Physical.GetPosition();

                    var length = dir.Length();

                    if (length < 0.000001)
                    {
                        dir = new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble()) * 2f - MathHelper.One;
                        length = 0.001f;
                    }

                    var maxForce = 20;
                    var a = -maxForce/radius;

                    var factor = 1;
                    if (length < 0.05)
                        factor = 20;

                    force += -Vector3.Normalize(dir) * factor; // * (-1*length + maxForce);
                }


                p.Physical.WorldMatrix *= Matrix.Translation(force * elapsed);
                
                
                simulateGravity(elapsed,o);
            }
        }

        private void simulateGravity(float elapsed,IItem t)
        {
            if (!t.Item.Free) return;
            var p = t as IPhysical;

            if (p == null) return;

            var pos = p.Physical.GetBoundingBox().Minimum;
            if (Math.Abs(pos.Y) > 0.001)
                p.Physical.WorldMatrix *= Matrix.Translation(0, -Math.Sign(pos.Y) * elapsed, 0);

        }
    }
}