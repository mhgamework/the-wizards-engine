﻿using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

        static GameVoxelType()
        {
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

        public virtual bool CanAcceptItemType(IVoxelHandle voxelHandle, ItemType type)
        {
            return false;
        }

        public virtual IEnumerable<IVoxelInfoVisualizer> GetCustomVisualizers(IVoxelHandle handle)
        {
          yield break;
        }
    }
}