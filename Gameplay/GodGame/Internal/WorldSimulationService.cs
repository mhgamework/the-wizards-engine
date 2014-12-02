using System.Collections.Generic;
using System.Reactive;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.Types;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Service that allows simulating a voxel world
    /// </summary>
    public class WorldSimulationService : ISimulator
    {
        private readonly Model.World world;
        private readonly VoxelTypesFactory voxelTypesFactory;

        public const float TickInterval = 1 / 20f;
        private float nextTick;

        public WorldSimulationService(Model.World world, VoxelTypesFactory voxelTypesFactory)
        {
            this.world = world;
            this.voxelTypesFactory = voxelTypesFactory;


            nextTick = TW.Graphics.TotalRunTime + TickInterval;
        }

        public void Simulate()
        {
            simulateCreateAndDestroy();
            if (nextTick > TW.Graphics.TotalRunTime) return;
            nextTick += TickInterval; //TODO: Check for timing problems

            simulatePerTypeTick();

            simulatePerVoxelTick();


        }

        private void simulateCreateAndDestroy()
        {
            var monitoringTypes = new HashSet<IGameVoxelType>(voxelTypesFactory.AllTypes.Where(t => t.ReceiveCreationEvents).ToArray());

            //TODO: if voxels are changed in the ondestroyed on oncreated events, then these events are not called for cet changes!!!
            world.ChangedVoxels.ToArray().ForEach(v =>
                {
                    if (monitoringTypes.Contains(v.PreviousType))
                        v.PreviousType.OnDestroyed(v);

                    if (monitoringTypes.Contains(v.Data.Type))
                        v.Data.Type.OnCreated(v);


                });
        }

        private void simulatePerVoxelTick()
        {
            world.ForEach((v, p) =>
                {
                    if (v.Type == null) return;
                    v.Type.Tick(v);
                });
        }

        private void simulatePerTypeTick()
        {
            voxelTypesFactory.AllTypes.ForEach(t => t.PerFrameTick());
        }
    }
}