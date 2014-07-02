using System;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    public interface IPlayerInputHandler
    {
         string Name { get; }
         void OnLeftClick(GameVoxel voxel);
         void OnRightClick(GameVoxel voxel);

      
    }

    class DelegatePlayerInputHandler : IPlayerInputHandler
    {
        private readonly Action<GameVoxel> onLeftClick;
        private readonly Action<GameVoxel> onRightClick;
        public string Name { get; private set; }

        public DelegatePlayerInputHandler(string name, Action<GameVoxel> onLeftClick, Action<GameVoxel> onRightClick)
        {
            this.onLeftClick = onLeftClick;
            this.onRightClick = onRightClick;
            Name = name;
        }

        public void OnLeftClick(GameVoxel voxel)
        {
            onLeftClick(voxel);
        }

        public void OnRightClick(GameVoxel voxel)
        {
            onRightClick(voxel);
        }
        public override string ToString()
        {
            return "Handler: " + Name;
        }

    }

    public class CreateLandInputHandler : IPlayerInputHandler
    {
        private readonly World world;

        public CreateLandInputHandler(World world)
        {
            this.world = world;
        }

        public string Name { get { return "CreateLand"; } }

        public void OnLeftClick(GameVoxel voxel)
        {
            voxel.ChangeType(GameVoxelType.Air);
        }
        public void OnRightClick(GameVoxel voxel)
        {
            voxel.ChangeType(GameVoxelType.Land);
        }
        public override string ToString()
        {
            return "Handler: " + Name;
        }

    }
}