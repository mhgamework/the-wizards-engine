using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Texturing;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.CG
{
    /// <summary>
    /// Responsible for creating/caching CG.Texture2D's from ITexture objects
    /// </summary>
    public interface ITexture2DLoader
    {
        Texture2D Load(ITexture texture);
    }
}
