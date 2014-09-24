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
            builder.RegisterTypes(ThisAssembly.GetTypes().Where(t => t.IsSubclassOf(typeof(GameVoxelType))).ToArray())
                .Except<BuildingSiteType>()
                .AsSelf()
                .As<GameVoxelType>()
                .OnActivated(o => ((GameVoxelType)o.Instance).InjectVoxelTypesFactory(o.Context.Resolve<VoxelTypesFactory>()))
                .SingleInstance();

            builder.RegisterType<MonumentVoxelBrain>();

            builder.RegisterType<VoxelTypesFactory>().SingleInstance();

            builder.RegisterType<ItemTypesFactory>().SingleInstance();

        }
    }
}