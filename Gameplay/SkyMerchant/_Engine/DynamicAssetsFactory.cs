using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.SkyMerchant._Engine
{
    /// <summary>
    /// Responsible for creating dynamic assets. 
    /// </summary>
    public class DynamicAssetsFactory
    {
         public RAMMesh CreateEmptyDynamicMesh()
         {
             return new RAMMesh();
         }
    }
}