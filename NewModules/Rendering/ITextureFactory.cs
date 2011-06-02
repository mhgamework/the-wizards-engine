using System;
using MHGameWork.TheWizards.Assets;

namespace MHGameWork.TheWizards.Rendering
{
    public interface ITextureFactory : IAssetFactory
    {
        ITexture GetTexture(Guid guid);
        ITexture FindTexture(Predicate<ITexture> predicate);
        void AddTexture(ITexture texture);


    }
}
