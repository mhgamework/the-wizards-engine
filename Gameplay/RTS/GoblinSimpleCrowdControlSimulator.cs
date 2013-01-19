using System;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.RTS
{
    public class GoblinSimpleCrowdControlSimulator : ISimulator
    {
        public void Simulate()
        {
            foreach (Goblin goblin in TW.Data.Objects.Where(o => o is Goblin))
            {
                var force = Vector3.Zero;
                foreach (Goblin other in TW.Data.Objects.Where(o => o is Goblin))
                {
                    if (goblin == other) continue;
                    var toOther = (other.Position - goblin.Position);

                    float minDist = 0.5f;
                    float maxDist = 1;

                    force += createRepellForce(maxDist, minDist, toOther);
                }

                var playerPos = TW.Data.GetSingleton<CameraInfo>().ActiveCamera.ViewInverse.xna().Translation.dx();
                var toPlayer = goblin.Position - playerPos;
                force += createRepellForce(3, 2, -toPlayer)*5;
                force *= 3;
                if (force.Length() > 5) force = Vector3.Normalize(force) * 5;
                goblin.Position += force * TW.Graphics.Elapsed;
            }
        }

        private Vector3 createRepellForce(float maxDist, float minDist, Vector3 toOther)
        {
            var dist = toOther.Length();
            dist -= minDist;
            dist = Math.Max(dist, 0);

            var factor = -dist / (maxDist - minDist) + 1;
            factor *= (1 / dist);
            //if (factor < 0.0001) return true;

            factor = MathHelper.Clamp(factor, 0, 100);
            var displace = factor * -toOther;
            displace.Y = 0;
            return displace;
        }
    }
}