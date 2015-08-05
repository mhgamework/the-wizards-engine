using DirectX11;

namespace MHGameWork.TheWizards.DualContouring.Terrain
{
    public interface IOctreeNode<T>
    {
        T[] Children { get; set; }
        Point3 LowerLeft { get; set; }
        int size { get;set; }
        int depth { get; set; }

        void Initialize();
        void Destroy();
    }
}