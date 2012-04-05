using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards._XNA.Scene
{
    public interface ISceneMeshProvider
    {
        IMesh GetMesh(string path);
    }
}