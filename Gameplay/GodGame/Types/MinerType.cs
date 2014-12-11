using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant._Tests.Ideas;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types
{
    /// <summary>
    /// Represents a miner
    /// Can hold a set of mined resources
    /// Chooses a target voxel to mine, and takes some time to mine a resource from that voxel
    /// </summary>
    public class Miner
    {
        /// <summary>
        /// Actual type should be IVoxel with IndustryBuildingAddon
        /// </summary>
        private IVoxel voxel;
        private int storedResources = 0; // Can only be described in function of time
        private IVoxel targetOreVoxel = null; // Can only be described in function of time (for example, a new closer ore is added, then targetore stays the old value)
        private float oreMiningProgress = 0;// Can only be described in function of time

        public const int MaxWorkers = 5;
        public const float TimeToMineOre = 5;
        public const int MinerRange = 3;

        public Miner(IVoxel voxel)
        {
            this.voxel = voxel;
        }

        public int StoredResources
        {
            get { return storedResources; }
            private set { storedResources = value; }
        }

        public IVoxel TargetOreVoxel
        {
            get { return targetOreVoxel; }
            private set { targetOreVoxel = value; }
        }

        public float OreMiningProgress
        {
            get { return oreMiningProgress; }
        }

        public int GetAllocatedWorkers() // Miner doesn't decide if worker allocation is time dependent or not
        {
            return voxel.Data.Type.GetAddon<IndustryBuildingAddon>(voxel).AllocatedWorkersCount;
        }

        public float CalculateEfficiency()
        {
            return GetAllocatedWorkers() / (float)MaxWorkers;
        }

        public bool CanStoreResource()
        {
            return StoredResources < 5;
        }

        private void storeResource()
        {
            if (!CanStoreResource()) return;
            StoredResources++;
        }

        public void ProgressMining(float delta)
        {
            var targetOre = GetTargetOre();
            if (targetOre == null)
            {
                oreMiningProgress = 0;
                return;
            }

            oreMiningProgress = OreMiningProgress + delta * CalculateEfficiency() / TimeToMineOre;
            if (OreMiningProgress < 1) return;
            oreMiningProgress = 1;

            if (!CanStoreResource()) return;
            targetOre.TakeResource();
            storeResource();

            TargetOreVoxel = null;
            oreMiningProgress = 0;
        }

        public Ore GetTargetOre()
        {
            if (IsMineableVoxel(TargetOreVoxel)) return TargetOreVoxel.GetPart<Ore>();
            TargetOreVoxel = FindMineableVoxel();
            return TargetOreVoxel.With(v => v.GetPart<Ore>());
        }

        public bool IsMineableVoxel(IVoxel voxel)
        {
            if (voxel == null) return false;
            if (!voxel.HasPart<Ore>()) return false;
            if (voxel.GetPart<Ore>().IsDepleted()) return false;
            return true;
        }

        public IVoxel FindMineableVoxel()
        {
            return voxel.GetRangeCircle(MinerRange).FirstOrDefault(IsMineableVoxel);
        }
    }

    /// <summary>
    /// Problem: if a voxel is changed type, the contents cease to exist. However, references to this content keep existing.
    ///     Some kind of automated mechanism to enforce use of deleted contents could be usefull
    /// </summary>
    public class Ore
    {
        public Ore()
        {
            ResourceCount = 10;
        }
        public int ResourceCount { get; set; }

        public void TakeResource()
        {
            if (IsDepleted()) throw new InvalidOperationException();
            ResourceCount--;
        }

        public bool IsDepleted()
        {
            return ResourceCount <= 0;
        }
    }

    public class MinerType : GameVoxelType
    {
        private int mineRadius = 3;

        public MinerType()
            : base("Miner")
        {
            RegisterAddonType(v => new IndustryBuildingAddon() { RequestedWorkersCount = 5 });
            ReceiveCreationEvents = true;

        }

        public override void OnCreated(IVoxel handle)
        {
            handle.SetPart(new Miner(handle));
            base.OnCreated(handle);
        }

        public override void OnDestroyed(IVoxel handle)
        {
            handle.SetPart<Miner>(null);
            base.OnDestroyed(handle);
        }

        public override void Tick(IVoxelHandle handle)
        {
            ((IVoxel)handle).GetPart<Miner>().ProgressMining(TW.Graphics.Elapsed);
            //handle.EachRandomInterval(1, () => tryOutput(handle));
        }

        private void tryOutput(IVoxelHandle handle)
        {
            if (handle.Data.Inventory.ItemCount == 0) return;

            for (int i = 0; i < 5; i++)
            {
                if (handle.Data.Inventory.ItemCount == 0) return;

                var type = handle.Data.Inventory.Items.First();
                var target = handle.Get4Connected().FirstOrDefault(v => v.CanAcceptItemType(type));
                if (target == null) break;

                if (target.Type is RoadType)
                    Road.DeliverItemClosest(target, handle, type);
                else
                    handle.Data.Inventory.TransferItemsTo(target.Data.Inventory, type, 1);
            }

        }

        public override IEnumerable<IRenderable> GetInfoVisualizers(IVoxelHandle handle)
        {
            foreach (var e in base.GetInfoVisualizers(handle))
                yield return e;

            yield return new RangeVisualizer(handle, mineRadius);
            //yield return new HighlightVoxelsVisualizer(handle, v => handle.Get<Miner>().);
            yield return new InventoryVisualizer(handle);
        }


        public override IEnumerable<IRenderable> GetCustomVisualizers(IVoxelHandle handle)
        {
            foreach (var e in base.GetCustomVisualizers(handle))
                yield return e;

            yield return new EntityVisualizer(e =>
            {
                e.Mesh = UtilityMeshes.CreateBoxColored(new Color4(0, 0, 1), new Vector3(1));
                var miner = ((IVoxel)handle).GetPart<Miner>();
                var targetVoxel = miner.TargetOreVoxel;
                e.Visible = targetVoxel != null;
                if (targetVoxel == null) return;

                var progress = miner.OreMiningProgress;
                var factor = progress < 0.5f ? progress * 2 : 1 - (progress - 0.5f) * 2;
                var toOre = handle.GetOffset(targetVoxel);
                var offset = (toOre.ToVector2() * factor).ToXZ();

                e.WorldMatrix = Matrix.Translation(((GameVoxel)handle).GetBoundingBox().GetCenter() + offset * 10);
            });

        }
    }

    public class EntityVisualizer : IRenderable
    {
        private readonly Action<Entity> updateAction;
        private Entity entity;

        public EntityVisualizer(Action<Entity> updateAction)
        {
            this.updateAction = updateAction;
        }

        public void Show()
        {
            entity = new Entity();
        }

        public void Update()
        {
            updateAction(entity);
        }

        public void Hide()
        {
            TW.Data.RemoveObject(entity);
        }
    }
}