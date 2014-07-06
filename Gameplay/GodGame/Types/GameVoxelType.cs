using System.Drawing;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame
{
    public class GameVoxelType
    {
        public static GameVoxelType Air;
        public static GameVoxelType Land;
        public static InfestationVoxelType Infestation = new InfestationVoxelType();
        public static ForestType Forest = new ForestType();

        static GameVoxelType()
        {
            Air = new GameVoxelType("Air") { NoMesh = true };
            Land = new GameVoxelType("Land") { Color = Color.SandyBrown };
        }

        public bool NoMesh { get; private set; }

        public string Name { get; private set; }

        public Color Color
        {
            get { return color; }
            set
            {
                if (color == value) return;
                color = value;
                if (NoMesh) mesh = null;

                mesh = UtilityMeshes.CreateBoxColored(Color, new Vector3(0.5f, 0.05f, 0.5f));
            }
        }

        public GameVoxelType(string name)
        {
            Name = name;
        }

        public virtual void Tick(GameVoxel v, ITickHandle handle)
        {

        }

        protected IMesh mesh;
        private Color color;

        public virtual IMesh GetMesh(GameVoxel gameVoxel)
        {
            return mesh;
        }




    }
}