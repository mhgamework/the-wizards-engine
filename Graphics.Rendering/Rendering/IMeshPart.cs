using MHGameWork.TheWizards.Assets;

namespace MHGameWork.TheWizards.Rendering
{
    public interface IMeshPart : IAsset
    {
        MeshPartGeometryData GetGeometryData();
        //void SetGeometryData(MeshPartGeometryData data); // This is a factory class, not a storage class :D
    }
}
