using System;

namespace MHGameWork.TheWizards.Rendering.Vegetation.Trees
{
   public interface ITreeTypeFactory
   {
       ITreeType GetTreeType(Guid guid);
   }
}
