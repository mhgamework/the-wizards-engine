using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Audio;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering.Deferred;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// Provide easy access to the resources of the currently running TWEngine
    /// </summary>
    public static class TW
    {
        private static Context ctx;

        public static AssetsWrapper Assets { get { return ctx.Assets; } }
        public static GraphicsWrapper Graphics { get { return ctx.Graphics; } }
        public static DataWrapper Data { get { return ctx.Data; } }
        public static PhysicsWrapper Physics { get { return ctx.Physics; } }
        public static AudioEngine Audio { get { return ctx.Audio; } }
        public static DebugWrapper Debug { get { return ctx.Debug; } }

        public static T GetService<T>() where T : class { return ctx.GetService<T>(); }

        public static void SetContext(Context _ctx)
        {
            ctx = _ctx;
        }

        public class Context
        {
            public Context()
            {
                Debug = new DebugWrapper(null);
            }
            public GraphicsWrapper Graphics { get; set; }
            public DataWrapper Data { get; set; }
            public PhysicsWrapper Physics { get; set; }
            public AudioWrapper Audio { get; set; }
            public DebugWrapper Debug { get; set; }
            public AssetsWrapper Assets { get; set; }


            private Dictionary<Type, object> services = new Dictionary<Type, object>();

            public T GetService<T>() where T:class 
            {
                return services[typeof (T)] as T;
            }
            public void SetService<T>(object obj) where T : class 
            {
                if (!typeof(T).IsAssignableFrom(obj.GetType()))
                    throw new InvalidOperationException();
                services[typeof (T)] = obj;
            }
        }
    }
}