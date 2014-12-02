using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.GodGame.Types.Towns.Workers;
using MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.SkyMerchant._Tests.Ideas;
using SlimDX;
using MHGameWork.TheWizards.IO;

namespace MHGameWork.TheWizards.GodGame.Types
{
    /// <summary>
    /// Represents a type of voxel content, should better by named TileType
    /// 
    /// Idea: generate RoadVoxel from RoadType, where it has wrappers for the roadtype methods taking a gamevoxel. 
    /// This way, logic can again be written on voxels instead of types
    /// Another idea: generate RoadType methods from RoadVoxel?
    /// http://www.hanselman.com/blog/T4TextTemplateTransformationToolkitCodeGenerationBestKeptVisualStudioSecret.aspx
    /// </summary>
    public class GameVoxelType : IGameVoxelType
    {
        protected IGameVoxelType Air { get { return typesFactory.Get<AirType>(); } }
        protected GameVoxelType Land { get { return typesFactory.Get<LandType>(); } }
        protected InfestationVoxelType Infestation { get { return typesFactory.Get<InfestationVoxelType>(); } }
        protected ForestType Forest { get { return typesFactory.Get<ForestType>(); } }
        protected WarehouseType Warehouse { get { return typesFactory.Get<WarehouseType>(); } }
        protected MonumentType Monument { get { return typesFactory.Get<MonumentType>(); } }
        protected WaterType Water { get { return typesFactory.Get<WaterType>(); } }
        protected HoleType Hole { get { return typesFactory.Get<HoleType>(); } }
        protected OreType Ore { get { return typesFactory.Get<OreType>(); } }
        protected MinerType Miner { get { return typesFactory.Get<MinerType>(); } }
        protected RoadType Road { get { return typesFactory.Get<RoadType>(); } }
        protected CropType Crop { get { return typesFactory.Get<CropType>(); } }
        protected FarmType Farm { get { return typesFactory.Get<FarmType>(); } }
        protected FisheryType Fishery { get { return typesFactory.Get<FisheryType>(); } }
        protected MarketType Market { get { return typesFactory.Get<MarketType>(); } } //order of construction important
        protected HouseType House { get { return typesFactory.Get<HouseType>(); } } //order of construction important
        protected WoodworkerType Woodworker { get { return typesFactory.Get<WoodworkerType>(); } }
        protected QuarryType Quarry { get { return typesFactory.Get<QuarryType>(); } }
        protected GrinderType Grinder { get { return typesFactory.Get<GrinderType>(); } }


        protected BuildingSiteType FisheryBuildSite { get { return typesFactory.GetBuildingSite<FisheryType>(); } }
        protected BuildingSiteType MarketBuildSite { get { return typesFactory.GetBuildingSite<MarketType>(); } }

        /// <summary>
        /// This types factory is used for legacy access to the voxel types in the gamevoxeltype.
        /// It is injected on creation of the type,
        /// </summary>
        protected VoxelTypesFactory typesFactory { get; private set; }
        public void InjectVoxelTypesFactory(VoxelTypesFactory factory)
        {
            typesFactory = factory;
        }

        /*private static List<GameVoxelType> allTypes = new List<GameVoxelType>();
        public static IEnumerable<GameVoxelType> AllTypes { get { return allTypes; } }*/

        static GameVoxelType()
        {
            /*allTypes.Add(Air);
            allTypes.Add(Land);
            allTypes.Add(Infestation);
            allTypes.Add(Forest);
            allTypes.Add(House);
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
            allTypes.Add(MarketBuildSite);
            allTypes.Add(Fishery);
            allTypes.Add(FisheryBuildSite);
            allTypes.Add(Woodworker);
            allTypes.Add(Quarry);
            allTypes.Add(Grinder);

            var voxelTypes =
                Assembly.GetExecutingAssembly().GetTypes().Where(t => (typeof(GameVoxelType)).IsAssignableFrom(t)
                                                                      && t != typeof(GameVoxelType));
            foreach (var type in voxelTypes)
            {
                if (AllTypes.All(it => it.GetType() != type))
                    throw new InvalidOperationException(
                        "There exists a subtype of GameVoxelType, which is not added to the list of GameVoxelType.AllTypes " + type.Name);
            }*/

        }
        public GameVoxelType(string name)
        {
            Name = name;
            coloredBaseMesh = true;

            searchSuggestedMeshes(Name);
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

            mesh = VoxelMeshBuilder.createColoredMesh(Color);
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

        public bool NoMesh { get; protected set; }

        public virtual bool DontShowDataValue { get { return false; } }

        protected void searchSuggestedMeshes(string name)
        {
            mesh = null;
            datavalueMeshes.Clear();

            var tilesFolder = TWDir.GameData.GetChild("Scattered").GetChild("GodGame").GetChild("Tiles");
            if (tilesFolder.CreateFile(name + ".obj").Exists) // CreateFile is a misnomer
            {
                mesh = TW.Assets.LoadMesh("Scattered\\GodGame\\Tiles\\" + name);
                mesh = MeshBuilder.Transform(mesh, Matrix.Scaling(0.1f, 0.1f, 0.1f));
                ColoredBaseMesh = false;
            }

            var meshes = tilesFolder.GetFiles(name + "*.obj");
            foreach (var f in meshes)
            {
                var dataValStr = Path.GetFileNameWithoutExtension(f.FullName).Substring(name.Length);
                int dataVal;
                if (!int.TryParse(dataValStr, out dataVal)) continue;

                datavalueMeshes[dataVal] = MeshBuilder.Transform(TW.Assets.LoadMesh("Scattered\\GodGame\\Tiles\\" + name + dataVal.ToString()), Matrix.Scaling(0.1f, 0.1f, 0.1f));
            }
        }


        protected IMesh mesh;
        protected Dictionary<int, IMesh> datavalueMeshes = new Dictionary<int, IMesh>();
        private Color color;
        private bool coloredBaseMesh;


        public virtual IMesh GetMesh(IVoxelHandle gameVoxel)
        {
            var tmp = mesh;
            if (datavalueMeshes.ContainsKey(gameVoxel.Data.DataValue))
                tmp = datavalueMeshes[gameVoxel.Data.DataValue];

            if (tmp == null) return null;

            var meshBuilder = new MeshBuilder();
            meshBuilder.AddMesh(tmp, Matrix.Identity);

            var groundMesh = GetDefaultGroundMesh(gameVoxel.Data.Height);
            if (groundMesh == null) return tmp;

            meshBuilder.AddMesh(groundMesh, Matrix.Identity);
            return meshBuilder.CreateMesh();
        }

        public IMesh GetDefaultGroundMesh(float height, Color c)
        {
            const float groundHeight = 15f * 0.1f;
            var meshheight = height * 0.1f;
            if (meshheight < 0)
                return null;

            var ret = UtilityMeshes.CreateBoxColored(c, new Vector3(0.5f, meshheight + groundHeight, 0.5f));
            return MeshBuilder.Transform(ret, Matrix.Translation(0, -meshheight - 0.1f - groundHeight, 0));
        }

        public IMesh GetDefaultGroundMesh(float height)
        {
            return GetDefaultGroundMesh(height, Color.SaddleBrown);
        }

        public IMesh GetDataValueMesh(int dataValue)
        {
            if (datavalueMeshes.ContainsKey(dataValue))
                return datavalueMeshes[dataValue];
            return mesh;
        }

        #endregion

        /// <summary>
        /// Tick method called once each frame, before all voxel ticks
        /// </summary>
        public virtual void PerFrameTick()
        {

        }

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
        public virtual IEnumerable<IRenderable> GetInfoVisualizers(IVoxelHandle handle)
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
        /// Should return true when the voxel can accept a single item of given type from given handle.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="deliveringHandle"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool CanAcceptItemType(IVoxelHandle handle, IVoxelHandle deliveringHandle, ItemType type)
        {
            return false;
        }

        /// <summary>
        /// Should provide visualizers which are enabled whenever the voxel is visible.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public virtual IEnumerable<IRenderable> GetCustomVisualizers(IVoxelHandle handle)
        {
            yield break;
        }

        /// <summary>
        /// Returns true if the voxel can accept a worker.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanAddWorker(IVoxelHandle handle)
        {
            return false;
        }




        private bool receiveCreationEvents;
        /// <summary>
        /// True when creation events are requested by the type, either by the user or by addons
        /// </summary>
        public bool ReceiveCreationEvents
        {
            get { return receiveCreationEvents || (addons.Count > 0); }
            protected set { receiveCreationEvents = value; }
        }

        public virtual void OnCreated(IVoxelHandle handle)
        {
            foreach (var addon in addons.Values)
            {
                var inst = addon.CreateAddon(handle);
                addon.Instances[handle] = inst;
                inst.OnCreated(handle);
            }
        }
        public virtual void OnDestroyed(IVoxelHandle handle)
        {
            foreach (var addon in addons.Values)
            {
                var inst = addon.CreateAddon(handle);
                addon.Instances[handle] = inst;
                inst.OnDestroyed(handle);
            }
        }


        private Dictionary<Type, ConfiguredAddon> addons = new Dictionary<Type, ConfiguredAddon>();
        private ConfiguredAddon configuredAddon;

        protected void RegisterAddonType<T>(Func<IVoxelHandle, T> create) where T : VoxelInstanceAddon
        {
            if (addons.ContainsKey(typeof(T))) throw new InvalidOperationException("Already configured");

            addons[typeof(T)] = new ConfiguredAddon(typeof(T), create);
        }

        public T GetAddon<T>(IVoxelHandle handle) where T : VoxelInstanceAddon
        {
            configuredAddon = addons[typeof(T)];

            return (T)configuredAddon.Instances[handle];
        }

        public bool HasAddon<T>(IVoxelHandle handle) where T : VoxelInstanceAddon
        {

            if (!addons.ContainsKey(typeof(T))) return false;

            // This makes has addon return false when the addon instance has not yet been initialized
            if (!addons[typeof(T)].Instances.ContainsKey(handle)) return false;
            return true;
        }

        public string GetDebugDescription(IVoxelHandle handle)
        {
            var ret = "";
            ret += "Type: " + Name + "\n";
            ret += getDebugDescription(handle) + "\n";


            var voxelAddons =
                addons.Values.Where(v => v.Instances.ContainsKey(handle)).Select(v => v.Instances[handle]).ToArray();
            if (voxelAddons.Length > 0)
            {
                foreach (var addon in voxelAddons)
                {
                    ret += "Addon: " + addon.GetType().Name + "\n";
                    ret += addon.GetDebugDescription() + "\n";

                }
            }
            return ret;
        }

        protected virtual string getDebugDescription(IVoxelHandle handle)
        {
            return "";
        }

        /// <summary>
        /// Represents a type of addon added to this gamevoxeltype
        /// </summary>
        private sealed class ConfiguredAddon
        {
            public Type AddonType { get; private set; }
            public Func<IVoxelHandle, VoxelInstanceAddon> CreateAddon { get; private set; }

            public Dictionary<IVoxelHandle, VoxelInstanceAddon> Instances =
                new Dictionary<IVoxelHandle, VoxelInstanceAddon>();

            public ConfiguredAddon(Type addonType, Func<IVoxelHandle, VoxelInstanceAddon> createAddon)
            {
                AddonType = addonType;
                CreateAddon = createAddon;
            }
        }


        public bool ReceiveChangeEvents { get; private set; }

        public virtual void OnChanged(IVoxelHandle handle)
        {

        }
    }
}