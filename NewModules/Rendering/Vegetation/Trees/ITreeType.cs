using MHGameWork.TheWizards.Assets;

namespace MHGameWork.TheWizards.Rendering.Vegetation.Trees
{
   public interface ITreeType:IAsset
   {
       TreeTypeData GetData();
   }
}
