using System;
using System.Linq;
using Autofac;
using Autofac.Core;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering.Lod;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Simulation.Spatial;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Bindings
{
    /// <summary>
    /// Responsible for registering the Bindings module for playing the game
    /// </summary>
    public class BindingsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ScatteredGame>().SingleInstance();

            builder.Register(c => new OptimizedWorldOctree<IRenderable>(new Vector3(10000, 2000, 10000), 5)).AsSelf().AsImplementedInterfaces().SingleInstance();

            configureRendering(builder);

            builder.RegisterType<Level>().SingleInstance();

            builder.Register(c => new Random(0));
        }

        private static void configureRendering(ContainerBuilder builder)
        {
            builder.Register(c =>
                {
                    var lvl = c.Resolve<Level>();
                    return new ScatteredRenderingSimulator(lvl, () => lvl.Islands.SelectMany(d => d.Addons),
                                                           c.Resolve<LinebasedLodRenderer>(), c.Resolve<WorldRenderingSimulator>());
                }).SingleInstance();

            builder.RegisterType<LinebasedLodRenderer>().SingleInstance()
                .OnActivated(a => { a.Instance.renderPhysicalRange = 400; });
        }
    }
}