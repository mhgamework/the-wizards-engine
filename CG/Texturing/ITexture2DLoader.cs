using MHGameWork.TheWizards.CG.OBJParser;

namespace MHGameWork.TheWizards.CG.Texturing
{
    /// <summary>
    /// Responsible for creating/caching CG.Texture2D's from ITexture objects
    /// </summary>
    public interface ITexture2DLoader
    {
        Texture2D Load(ITexture texture);
    }
}
