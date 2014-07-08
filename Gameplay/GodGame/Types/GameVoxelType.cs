using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using SlimDX;
using MHGameWork.TheWizards.IO;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class GameVoxelType
    {
        public static GameVoxelType Air;
        public static GameVoxelType Land;
        public static InfestationVoxelType Infestation = new InfestationVoxelType();
        public static ForestType Forest = new ForestType();
        public static VillageType Village = new VillageType();
        public static WarehouseType Warehouse = new WarehouseType();
        public static MonumentType Monument = new MonumentType();

        static GameVoxelType()
        {
            Air = new GameVoxelType("Air") { NoMesh = true };
            Land = new GameVoxelType("Land") { Color = Color.SandyBrown };
        }
        public GameVoxelType(string name)
        {
            Name = name;
            coloredBaseMesh = true;

            searchSuggestedMeshes();
        }

        public string Name { get; private set; }


        public bool NoMesh { get; private set; }

        #region Colored mesh

        public Color Color
        {
            get { return color; }
            set
            {
                if (color == value) return;
                color = value;
                updateColoredBaseMesh();
            }
        }

        private void updateColoredBaseMesh()
        {
            if (NoMesh) mesh = null;
            if (!ColoredBaseMesh) return;

            mesh = UtilityMeshes.CreateBoxColored(Color, new Vector3(0.5f, 0.05f, 0.5f));
        }

        public bool ColoredBaseMesh
        {
            get { return coloredBaseMesh; }
            set
            {
                if (coloredBaseMesh == value) return;
                coloredBaseMesh = value;
                updateColoredBaseMesh();
            }
        }
        #endregion

        private void searchSuggestedMeshes()
        {
            var tilesFolder = TWDir.GameData.GetChild("Scattered").GetChild("GodGame").GetChild("Tiles");
            if (tilesFolder.CreateFile(Name + ".obj").Exists) // CreateFile is a misnomer
            {
                mesh = TW.Assets.LoadMesh("Scattered\\GodGame\\Tiles\\" + Name);
                mesh = MeshBuilder.Transform(mesh, Matrix.Scaling(0.1f, 0.1f, 0.1f));
                ColoredBaseMesh = false;
            }

            var meshes = tilesFolder.GetFiles(Name + "*.obj");
            foreach (var f in meshes)
            {
                var dataValStr = Path.GetFileNameWithoutExtension(f.FullName).Substring(Name.Length);
                int dataVal;
                if (!int.TryParse(dataValStr, out dataVal)) continue;

                datavalueMeshes[dataVal] = TW.Assets.LoadMesh("Scattered\\GodGame\\Tiles\\" + Name + dataVal.ToString());
            }
        }

        public virtual void Tick(IVoxelHandle handle)
        {

        }

        protected IMesh mesh;
        protected Dictionary<int, IMesh> datavalueMeshes = new Dictionary<int, IMesh>();
        private Color color;
        private bool coloredBaseMesh;

        public virtual IMesh GetMesh(IVoxelHandle gameVoxel)
        {
            if (datavalueMeshes.ContainsKey(gameVoxel.Data.DataValue))
                return datavalueMeshes[gameVoxel.Data.DataValue];
            return mesh;
        }


        public bool Interact(IVoxelHandle handle)
        {
            return false;
        }
    }
}