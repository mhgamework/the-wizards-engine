using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;

namespace TreeGenerator
{
    public interface ITextureFactory
    {
         ITexture GetTexture(Guid guid);
    }
}
