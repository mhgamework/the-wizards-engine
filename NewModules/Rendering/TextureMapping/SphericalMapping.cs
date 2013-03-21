using System;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering.TextureMapping
{
    public class SphericalMapping
    {

        public Vector2 Map(Vector3 localHitPoint)
        {
            // first, compute theta and phi

            var theta = (float)Math.Acos(localHitPoint.Y);
            var phi = (float)Math.Atan2(localHitPoint.X, localHitPoint.Z);
            if (phi < 0.0f)
            {
                phi += MathHelper.TwoPi;
            }

            // next, map theta and phi to (u, v) in [0, 1] X [0, 1]

            var u = (float)(phi * (1.0f / MathHelper.TwoPi));
            var v = (float)(1.0 - theta * (1 / MathHelper.Pi));

            return new Vector2(u, v);
        }
    }
}