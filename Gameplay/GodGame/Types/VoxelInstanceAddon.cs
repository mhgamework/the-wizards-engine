using MHGameWork.TheWizards.GodGame.Internal.Model;

namespace MHGameWork.TheWizards.GodGame.Types
{
    /// <summary>
    /// Represents an object which is associated with a specific voxel+type combination
    /// Note that this interface facilitates 2 unrelated things:
    ///     1. it allows addon extension of voxel types
    ///     2. it allows object instances per voxel+type combination
    /// These 2 aspects might have to be decoupled at a later point
    /// </summary>
    public abstract class VoxelInstanceAddon
    {
        public virtual void OnCreated(IVoxelHandle handle)
        {

        }
        public virtual void OnDestroyed(IVoxelHandle handle)
        {

        }

        public virtual string GetDebugDescription()
        {
            return "";
        }
    }
}