using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    public class TestUtilities
    {
        public static Entity CreateGroundPlane()
        {
            return new Entity()
                {
                    Mesh = TW.Assets.LoadMesh("Core\\Building\\Plane"),
                    WorldMatrix = Matrix.Scaling(1000, 1000, 1000)
                };
        }
    }
}