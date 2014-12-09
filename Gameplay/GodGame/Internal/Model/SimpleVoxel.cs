using DirectX11;

namespace MHGameWork.TheWizards.GodGame.Internal.Model
{
    /// <summary>
    /// IVoxel implementation for testing
    /// </summary>
    public class SimpleVoxel :IVoxel
    {
        private SimpleWorld world;

        public SimpleVoxel(SimpleWorld world)
        {
            this.world = world;
        }

        public IVoxel GetRelative(Point2 offset)
        {
            return world.GetVoxel(world.GetPos(this) + offset);
        }

        public Point2 GetOffset(IVoxel other)
        {
            return world.GetPos(other) - world.GetPos(this);
        }

        public IVoxelData Data { get; private set; }
        public bool HasPart<T>()
        {
            throw new System.NotImplementedException();
        }

        public T GetPart<T>()
        {
            throw new System.NotImplementedException();
        }

        public void SetPart<T>(T value) where T : class
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return world.GetPos(this).ToString();
        }
    }
}