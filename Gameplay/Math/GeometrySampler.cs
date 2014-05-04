using System;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// Allows uniform random sampling of geometric shapes and distributions.
    /// </summary>
    public class GeometrySampler
    {
        private readonly Seeder s;

        public GeometrySampler(Seeder s)
        {
            this.s = s;
        }

        public Vector2 RandomPointOnCircle()
        {
            var angle = s.NextFloat(0, MathHelper.TwoPi);
            return new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
        }

        /// <summary>
        /// From http://mathoverflow.net/questions/24688/efficiently-sampling-points-uniformly-from-the-surface-of-an-n-sphere
        /// </summary>
        /// <returns></returns>
        public Vector3 RandomPointOnSphere()
        {
            var rand = new Vector3(RandomNormalDistribution(), RandomNormalDistribution(), RandomNormalDistribution());
            rand.Normalize();
            return rand;
        }

        /// <summary>
        /// Approximation of the normal distribution
        /// </summary>
        /// <returns></returns>
        public float RandomNormalDistribution()
        {
            var rand = 0f;
            for (int i = 0; i < 12; i++)
                rand += s.NextFloat(0, 1);
            return rand - 6;
        }
    }
}