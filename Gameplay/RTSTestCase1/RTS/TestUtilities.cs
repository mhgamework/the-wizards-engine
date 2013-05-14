using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    public class TestUtilities
    {
        public static Entity CreateGroundPlane()
        {
            var e =  new Entity()
                {
                    Mesh = TW.Assets.LoadMesh("Core\\Building\\Plane"),
                    WorldMatrix = Matrix.Scaling(1000, 1000, 1000)
                };

            e.CastsShadows = false;

            return e;

        }
        public static ITexture LoadWoodTexture()
        {
            return TW.Assets.LoadTexture("RTS\\bark.jpg");
        }
    }
}