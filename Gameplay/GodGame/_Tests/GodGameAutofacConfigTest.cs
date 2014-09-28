using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac.Core;
using Castle.DynamicProxy;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.DeveloperCommands;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Configuration;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Networking;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.Scattered.Model;
using NSubstitute;
using NUnit.Framework;
using SlimDX;
using System.Linq;
using Autofac;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    /// <summary>
    /// Test for the full game + helpers for integration testing.
    /// </summary>
    [TestFixture]
    public class GodGameAutofacConfigTest
    {
        private ContainerBuilder builder;
        private Internal.Model.World world;
        private Textarea textarea;
        private WorldPersisterService worldPersister;

        [SetUp]
        public void Setup()
        {
            textarea = new Textarea();
            textarea.Text = "";

            builder = new Autofac.ContainerBuilder();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace != null && t.Namespace.StartsWith("MHGameWork.TheWizards.GodGame")
                && t.IsPublic))
            {
                builder.Register(delegate(IComponentContext ctx)
                {
                    textarea.Text += type.FullName + "\n";
                    return new object();
                }).As(type);
            }

            world = new Internal.Model.World(200, 10, (w, p) => new GameVoxel(w, p, new ProxyGenerator()));

            builder.RegisterInstance(world);
            builder.RegisterInstance(worldPersister);

            builder.Register(ctx => new IPlayerTool[0]).As<IEnumerable<IPlayerTool>>();
        }

        [Test]
        public void TestServerConfiguration()
        {
            builder.RegisterModule<CommonModule>();
            builder.RegisterModule<ServerModule>();

            var container = builder.Build();

            container.Resolve<GodGameServer>();

            textarea.Text += "\nComplete!";
        }

        [Test]
        public void TestClientConfiguration()
        {
            builder.RegisterModule<CommonModule>();
            builder.RegisterModule<ClientModule>();

            var container = builder.Build();
            container.Resolve<GodGameClient>();
            textarea.Text += "\nComplete!";

        }

    }
}