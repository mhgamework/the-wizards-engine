using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Types.Transportation.Generic
{
    public interface IRoadComponent
    {
        bool CanAcceptItem(IVoxel voxel, ItemType item);
    }
}