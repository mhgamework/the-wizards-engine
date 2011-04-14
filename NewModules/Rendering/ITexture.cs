using System;
using MHGameWork.TheWizards.Assets;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// This interface has to be moved to a correct namespace
    /// </summary>
    public interface ITexture : IAsset
    {
        TextureCoreData GetCoreData();
    }
}
