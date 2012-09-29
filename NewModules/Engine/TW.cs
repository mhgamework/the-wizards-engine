using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Audio;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering.Deferred;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards
{
    public static class TW
    {
        private static Context ctx;

        public static DX11Game Game
        {
            get { return ctx.Game; }
        }

        public static ModelContainer.ModelContainer Model
        {
            get { return ctx.Model; }
        }

        public static PhysicsEngine PhysX
        {
            get { return ctx.PhysX; }
        }

        public static Scene Scene
        {
            get { return ctx.Scene; }
        }

        public static AudioEngine Audio { get { return ctx.Audio; } }

        public static DeferredRenderer AcquireRenderer()
        {
            return ctx.AcquireRenderer();
        }


        public static void SetContext(Context _ctx)
        {
            ctx = _ctx;
        }

        public class Context
        {
            public DX11Game Game { get; set; }
            public ModelContainer.ModelContainer Model { get; set; }
            public PhysicsEngine PhysX { get; set; }
            public StillDesign.PhysX.Scene Scene { get; set; }
            public AudioEngine Audio { get; set; }

            private DeferredRenderer renderer;

            public DeferredRenderer AcquireRenderer()
            {
                if (renderer == null)
                    renderer = new DeferredRenderer(TW.Game);

                return renderer;
            }

        }
    }
}
