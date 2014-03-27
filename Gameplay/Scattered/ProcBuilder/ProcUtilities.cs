using Microsoft.Xna.Framework;
using ProceduralBuilder.Shapes;
using Matrix = SlimDX.Matrix;
using Ray = SlimDX.Ray;

namespace MHGameWork.TheWizards.Scattered.ProcBuilder
{
    public class ProcUtilities
    {
        public static float? RaycastFace(Face f, Ray r)
        {
            r = r.Transform(Matrix.Invert(f.GetWorldMatrix()));
            return r.xna().Intersects(new BoundingBox(new Vector3(0, 0, 0), new Vector3(f.Size.X, f.Size.Y, 0)));
        }
    }
}