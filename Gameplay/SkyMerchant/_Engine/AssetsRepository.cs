using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.SkyMerchant._Engine
{
    /// <summary>
    /// Responsible for giving access to the GameData assets.
    /// TODO: use the EngineFileSystem
    /// </summary>
    public class AssetsRepository
    {
        public IMesh LoadMesh(string relativeCorePath)
        {
            return TW.Assets.LoadMesh(relativeCorePath);
        }
        public ITexture LoadTexture(string relativCorePath)
        {
            return TW.Assets.LoadTexture(relativCorePath);
        }
    }
}