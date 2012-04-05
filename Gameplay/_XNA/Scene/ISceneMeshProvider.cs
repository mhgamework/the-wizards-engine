using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.Scene
{
    public interface ISceneMeshProvider
    {
        IMesh GetMesh(string path);
    }
}