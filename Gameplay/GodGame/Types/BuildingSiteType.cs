using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class BuildingSiteType : GameVoxelType
    {
        private GameVoxelType building;
        private List<ItemAmount> neededResources;
        private int totalNbNeededResources;
        private int nbBuildingSiteModels = 4;

        private List<IMesh> meshes;

        public struct ItemAmount
        {
            public ItemType Type;
            public int Amount;
        }

        public BuildingSiteType(GameVoxelType building, List<ItemAmount> neededResources, string name)
            : base("BuildingSite" + name)
        {
            this.building = building;
            this.neededResources = neededResources;
            totalNbNeededResources = 0;
            foreach (var res in neededResources)
            {
                totalNbNeededResources += res.Amount;
            }

            searchSuggestedMeshes("BuildingSite");
            Color = Color.White;

            meshes = new List<IMesh>();
            for (int i = 0; i < nbBuildingSiteModels; i++)
            {
                var baseMesh = datavalueMeshes[i];
                MeshBuilder.AppendMeshTo(building.GetDataValueMesh(0), baseMesh, Matrix.Identity);
                meshes.Add(baseMesh);
            }

        }

        public override void Tick(IVoxelHandle handle)
        {
            handle.EachRandomInterval(0.5f, () =>
            {
                tryGatherResources(handle);
                updateAppearance(handle);
                checkComplete(handle);
            });
        }

        public override IMesh GetMesh(IVoxelHandle handle)
        {
            var tmp = meshes[handle.Data.DataValue];

            var meshBuilder = new MeshBuilder();
            meshBuilder.AddMesh(tmp, Matrix.Identity);
            var groundMesh = GetDefaultGroundMesh(handle.Data.Height);
            if (groundMesh == null) return tmp;
            meshBuilder.AddMesh(groundMesh, Matrix.Identity);
            return meshBuilder.CreateMesh();
        }

        private void tryGatherResources(IVoxelHandle handle)
        {
            var warehousesInRange = getWareHousesInRange(handle, 100);
            foreach (var resAmnt in neededResources.Where(e => countResourcesIncludingKanban(e.Type, handle) < e.Amount))
            {
                foreach (var warehouse in warehousesInRange.Where(warehouse => warehouse.Data.Inventory.GetAmountOfType(resAmnt.Type) > 0))
                {
                    var road = Road.IsConnected(warehouse, handle);
                    if (road != null)
                    {
                        Road.DeliverItem(road, warehouse, handle, resAmnt.Type);
                        break;
                    }

                }
            }
        }

        private int countResourcesIncludingKanban(ItemType type, IVoxelHandle handle)
        {
            var inventory = handle.Data.Inventory;
            return inventory.GetAmountOfType(type) + inventory.GetAmountOfType(Road.GetIncomingKanban(type)) + inventory.GetAmountOfType(Road.GetOutgoingKanban(type));
        }

        private IEnumerable<IVoxelHandle> getWareHousesInRange(IVoxelHandle handle, int range)
        {
            return handle.GetRange(range).Where(v => v.Type is WarehouseType);
        }

        private void updateAppearance(IVoxelHandle handle)
        {
            var nbStoredRes = handle.Data.Inventory.ItemCount;
            var completionRate = (float)nbStoredRes / (float)totalNbNeededResources;

            var newDataVal = (int)Math.Floor(completionRate * nbBuildingSiteModels);
            if (newDataVal >= nbBuildingSiteModels)
                newDataVal = nbBuildingSiteModels - 1;
            handle.Data.DataValue = newDataVal;

            //todo update dataVal instead, make models
            if (completionRate < 0.2f)
            {
                Color = Color.White;
                return;
            }
            if (completionRate < 0.4f)
            {
                Color = Color.LightGray;
                return;
            }
            if (completionRate < 0.6f)
            {
                Color = Color.DarkGray;
                return;
            }
            if (completionRate < 0.8f)
            {
                Color = Color.Gray;
            }

            Color = Color.Black;
        }

        private void checkComplete(IVoxelHandle handle)
        {
            if (neededResources.All(e => e.Amount == handle.Data.Inventory.GetAmountOfType(e.Type)))
                handle.ChangeType(building);
        }
    }
}
