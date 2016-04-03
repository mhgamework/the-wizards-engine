using DirectX11;

namespace MHGameWork.TheWizards.DualContouring.Terrain
{
    public interface IOctreeNodeFactory<T> where T : IOctreeNode<T>
    {
        void Destroy(T node);
        T Create( T parent, int size, int depth, Point3 pos );
    }
}