using DirectX11;
using NSubstitute.Routing.Handlers;

namespace MHGameWork.TheWizards.GodGame.Internal.Model
{
    /// <summary>
    /// Represents a voxel of the world
    /// NOTE/IDEA: if the amount of instances of IVoxel causes memory problems,
    ///    this interface can be implemented by a struct with only a point2
    ///    if the struct contains an additonal reference to some sort of repository
    ///    holding the actual data
    /// </summary>
    public interface IVoxel
    {
        /// <summary>
        /// Returns the voxel at a relative position offset with respect to this
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        IVoxel GetRelative(Point2 offset);
        /// <summary>
        /// Returns the position of other relative to this
        /// </summary>
        Point2 GetOffset(IVoxel other);
        IVoxelData Data { get; }
        /*ITileType Type { get; set; }
        ITile Tile { get; set; }
        float Height { get; set; }*/
        /// <summary>
        /// Check if the voxel has given contents
        /// </summary>
        bool HasPart<T>();
        /// <summary>
        /// Get contents of type T
        /// </summary>
        T GetPart<T>();

        /// <summary>
        /// Probably should not be on this interface, but inside a service
        /// </summary>
        void SetPart<T>(T value) where T : class;
    }
}