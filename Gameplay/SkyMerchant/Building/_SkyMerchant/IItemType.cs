using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.SkyMerchant.Building._SkyMerchant
{
    /// <summary>
    /// Represents a type of item
    /// </summary>
    public interface IItemType
    {
         
    }

    public class SimpleItemType : IItemType
    {
        public string Name { get; set; }

        public IMesh Mesh { get; set; }
    }
}