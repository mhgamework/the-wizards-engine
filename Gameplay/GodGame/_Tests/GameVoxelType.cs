using SlimDX;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    public class GameVoxelType
    {
        public static GameVoxelType Air;
        public static GameVoxelType Land;

        static GameVoxelType()
        {
            Air = new GameVoxelType("Air") { NoMesh = true };
            Land = new GameVoxelType("Land");
        }

        public bool NoMesh { get; private set; }

        public string Name { get; private set; }

        public Color4 Color { get; set; }

        public GameVoxelType(string name)
        {
            Name = name;
        }

    }
}