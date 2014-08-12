using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;
using MHGameWork.TheWizards.IO;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class GameVoxelType
    {
        public static GameVoxelType Air = new GameVoxelType("Air") { NoMesh = true };
        public static GameVoxelType Land = new GameVoxelType("Land") { Color = Color.SandyBrown };
        public static InfestationVoxelType Infestation = new InfestationVoxelType();
        public static ForestType Forest = new ForestType();
        public static VillageType Village = new VillageType();
        public static WarehouseType Warehouse = new WarehouseType();
        public static MonumentType Monument = new MonumentType();
        public static WaterType Water = new WaterType();
        public static HoleType Hole = new HoleType();
        public static OreType Ore = new OreType();
        public static MinerType Miner = new MinerType();
        public static RoadType Road = new RoadType();
        public static CropType Crop = new CropType();
        public static FarmType Farm = new FarmType();
        public static MarketType Market = new MarketType();

        private static List<GameVoxelType> allTypes = new List<GameVoxelType>();
        public static IEnumerable<GameVoxelType> AllTypes { get { return allTypes; } }

        static GameVoxelType()
        {
            allTypes.Add(Air);
            allTypes.Add(Land);
            allTypes.Add(Infestation);
            allTypes.Add(Forest);
            allTypes.Add(Village);
            allTypes.Add(Warehouse);
            allTypes.Add(Monument);
            allTypes.Add(Water);
            allTypes.Add(Hole);
            allTypes.Add(Ore);
            allTypes.Add(Miner);
            allTypes.Add(Road);
            allTypes.Add(Crop);
            allTypes.Add(Farm);
            allTypes.Add(Market);

            var voxelTypes =
                Assembly.GetExecutingAssembly().GetTypes().Where(t => (typeof(GameVoxelType)).IsAssignableFrom(t)
                                                                      && t != typeof(GameVoxelType));
            foreach (var type in voxelTypes)
            {
                if (AllTypes.All(it => it.GetType() != type))
                    throw new InvalidOperationException(
                        "There exists a subtype of GameVoxelType, which is not added to the list of GameVoxelType.AllTypes " + type.Name);
            }

        }
        public GameVoxelType(string name)
        {
            Name = name;
            coloredBaseMesh = true;

            searchSuggestedMeshes();
        }

        public string Name { get; private set; }

        #region Mesh

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

        public bool NoMesh { get; private set; }

        public virtual bool DontShowDataValue { get { return false; } }

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

                datavalueMeshes[dataVal] = MeshBuilder.Transform(TW.Assets.LoadMesh("Scattered\\GodGame\\Tiles\\" + Name + dataVal.ToString()), Matrix.Scaling(0.1f, 0.1f, 0.1f));
            }
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

        #endregion

        /// <summary>
        /// Called when a voxel should simulate its logic
        /// </summary>
        /// <param name="handle"></param>
        public virtual void Tick(IVoxelHandle handle) { }

        /// <summary>
        /// Called when the user attempts to interact with the voxels, 
        /// most likely through a right click
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public virtual bool Interact(IVoxelHandle handle) { return false; }

        /// <summary>
        /// Used to display info about a voxel. The user can request additional info about a voxel.
        /// This method should return a list of visualizers that display the necessary info to the user
        /// for given voxel.
        /// NOTE: the list of returned visualizers should remain the same for each voxel+type combination throughout the application,
        /// otherwise the list might not get updated as expected.
        /// </summary>
        public virtual IEnumerable<IVoxelInfoVisualizer> GetInfoVisualizers(IVoxelHandle handle)
        {
            yield return new InventoryVisualizer(handle);
        }

        /// <summary>
        /// Should return true when the voxel can accept a single item of given type.
        /// </summary>
        /// <param name="voxelHandle"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool CanAcceptItemType(IVoxelHandle voxelHandle, ItemType type)
        {
            return false;
        }

        /// <summary>
        /// Should provide visualizers which are enabled whenever the voxel is visible.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public virtual IEnumerable<IVoxelInfoVisualizer> GetCustomVisualizers(IVoxelHandle handle)
        {
            yield break;
        }
    }
}