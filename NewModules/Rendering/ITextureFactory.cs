using System;

namespace MHGameWork.TheWizards.Rendering
{
    public interface ITextureFactory
    {
         ITexture GetTexture(Guid guid);
    }
}
