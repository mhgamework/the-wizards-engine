using DirectX11;

namespace MHGameWork.TheWizards.DualContouring.Terrain
{
    public interface IOctreeNode<T>
    {
        T[] Children { get; set; }
        Point3 LowerLeft { get; set; }
        int Size { get;set; }
        int Depth { get; set; }

        void Initialize(T parent);
        void Destroy();
    }
}