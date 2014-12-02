using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public interface IGameVoxelType
    {
        string Name { get; }

        /// <summary>
        /// Called when a voxel should simulate its logic
        /// </summary>
        /// <param name="handle"></param>
        void Tick(IVoxelHandle handle);

        /// <summary>
        /// Called when the user attempts to interact with the voxels, 
        /// most likely through a right click
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        bool Interact(IVoxelHandle handle);

        /// <summary>
        /// Used to display info about a voxel. The user can request additional info about a voxel.
        /// This method should return a list of visualizers that display the necessary info to the user
        /// for given voxel.
        /// NOTE: the list of returned visualizers should remain the same for each voxel+type combination throughout the application,
        /// otherwise the list might not get updated as expected.
        /// </summary>
        IEnumerable<IRenderable> GetInfoVisualizers(IVoxelHandle handle);

        /// <summary>
        /// Should return true when the voxel can accept a single item of given type.
        /// </summary>
        /// <param name="voxelHandle"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool CanAcceptItemType(IVoxelHandle voxelHandle, ItemType type);

        /// <summary>
        /// Should return true when the voxel can accept a single item of given type from given handle.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="deliveringHandle"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool CanAcceptItemType(IVoxelHandle handle, IVoxelHandle deliveringHandle, ItemType type);

        /// <summary>
        /// Should provide visualizers which are enabled whenever the voxel is visible.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        IEnumerable<IRenderable> GetCustomVisualizers(IVoxelHandle handle);

        /// <summary>
        /// When true the simulator will call the OnCreated and OnDestroyed methods
        /// </summary>
        bool ReceiveCreationEvents { get; }
        /// <summary>
        /// Called when argument voxel's type is changed to this voxel type
        /// </summary>
        /// <param name="handle"></param>
        void OnCreated(IVoxelHandle handle);
        /// <summary>
        /// Called when a voxel with this voxel type is changed to another type
        /// </summary>
        /// <param name="handle"></param>
        void OnDestroyed(IVoxelHandle handle);

        /// <summary>
        /// When enabled, the simulator calls the OnChanged method
        /// </summary>
        bool ReceiveChangeEvents { get; }
        /// <summary>
        /// Called when some part of the voxel's data changes
        /// </summary>
        void OnChanged(IVoxelHandle handle);

        IMesh GetMesh(IVoxelHandle gameVoxel);

        /// <summary>
        /// Tick method called once each frame, before all voxel ticks
        /// </summary>
        void PerFrameTick();

        T GetAddon<T>(IVoxelHandle handle) where T : VoxelInstanceAddon;
        bool HasAddon<T>(IVoxelHandle handle) where T : VoxelInstanceAddon;
    }
}