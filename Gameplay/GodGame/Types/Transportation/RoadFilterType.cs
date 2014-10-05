using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class RoadFilterType : GameVoxelType
    {
        public RoadFilterType()
            : base("RoadFilter")
        {
        }

        public override void Tick(Internal.Model.IVoxelHandle handle)
        {
            var filterData = handle.Data.Get<IRoadFilterData>();
            bool someUserInput = true;
            if (someUserInput)
                filterData.Whitelist = !filterData.Whitelist;
        }
        public interface IRoadFilterData : IVoxelDataExtension
        {
            ItemType[] ItemTypes { get; set; }
            bool Whitelist { get; set; }
        }
    }

}