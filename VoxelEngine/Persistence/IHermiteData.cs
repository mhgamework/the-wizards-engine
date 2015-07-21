using DirectX11;

namespace MHGameWork.TheWizards.VoxelEngine.Persistence
{
    /// <summary>
    /// Signs go from 0,0,0 to NumCells,NumCells,NumCells
    /// </summary>
    public interface IHermiteData
    {
        Point3 NumCells { get; }
        /// <summary>
        /// Get intersection point between cell (0) and cell + dir (1), as a lerp factor between 0 and 1
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="dir">X:0,Y:1,Z:2</param>
        /// <returns></returns>
        float GetIntersection(Point3 cell, int dir);
        Vector3 GetNormal(Point3 cell, int dir);

        /// <summary>
        /// Air is null
        /// </summary>
        object GetMaterial(Point3 cell);
        void SetMaterial(Point3 cell, object mat);


        void SetIntersection( Point3 flattenedCoord, int dir, float intersect );
        void SetNormal( Point3 flattenedCoord, int dir, Vector3 normal );
    }
}