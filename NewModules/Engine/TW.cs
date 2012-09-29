using MHGameWork.TheWizards.Audio;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering.Deferred;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards
{
    public static class TW
    {
        private static Context ctx;

        public static GraphicsWrapper Graphics { get { return ctx.Graphics; } }
        public static DataWrapper Data { get { return ctx.Data; } }
        public static PhysicsWrapper Physics { get { return ctx.Physics; } }
        public static AudioEngine Audio { get { return ctx.Audio; } }




        public static void SetContext(Context _ctx)
        {
            ctx = _ctx;
        }

        public class Context
        {
            public GraphicsWrapper Graphics { get; set; }
            public DataWrapper Data { get; set; }
            public PhysicsWrapper Physics { get; set; }
            public AudioWrapper Audio { get; set; }



        }
    }
}