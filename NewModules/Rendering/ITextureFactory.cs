using System;
using MHGameWork.TheWizards.Assets;

namespace MHGameWork.TheWizards.Rendering
{
    public interface ITextureFactory : IAssetFactory
    {
         ITexture GetTexture(Guid guid);
    }
}
