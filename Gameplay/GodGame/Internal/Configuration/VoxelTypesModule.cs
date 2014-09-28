using System;
using Autofac;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.Types;

namespace MHGameWork.TheWizards.GodGame.Internal.Configuration
{
    /// <summary>
    /// Responsible for configuring all voxel types
    /// </summary>
    public class VoxelTypesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Type[] types = ThisAssembly.GetTypes().Where(t => t.IsSubclassOf(typeof (GameVoxelType))).ToArray();
            builder.RegisterTypes(types)
                .Except<BuildingSiteType>()
                .AsSelf()
                .As<IGameVoxelType>()
                .OnActivated(o => ((GameVoxelType)o.Instance).InjectVoxelTypesFactory(o.Context.Resolve<VoxelTypesFactory>()))
                .SingleInstance();

            builder.RegisterType<MonumentVoxelBrain>();

            builder.RegisterType<VoxelTypesFactory>().SingleInstance();

            builder.RegisterType<ItemTypesFactory>().SingleInstance();

        }
    }
}