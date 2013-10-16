using MHGameWork.TheWizards.SkyMerchant._Engine.Spatial;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects
{
    /// <summary>
    /// Implements the position component of a game object.
    /// Responsible for providing the space this object occupies in the world.
    /// Responsible for manipulating this objects world orientation
    /// </summary>
    public interface IPositionComponent : IMutableSpatial, IGameObjectComponent
    {

    }

    public static class PositionComponentExtensions
    {
        public static Matrix GetWorldMatrix(this IPositionComponent obj)
        {
            return Matrix.RotationQuaternion(obj.Rotation) * Matrix.Translation(obj.Position);
        }
    }
}