using System;
using System.Collections.Generic;
using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.Scattered._Engine;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types
{
    /// <summary>
    /// Displays a max of 50 items from the inventory
    /// </summary>
    public class InventoryVisualizer : IVoxelInfoVisualizer
    {
        private readonly GameVoxel handle;
        private Entity[] entityCache = new Entity[50];

        public Func<int, Matrix> ItemRelativeTransformationProvider { get; set; }

        public InventoryVisualizer(IVoxelHandle handle)
        {
            // WARNING: cannot keep handle because it can be shared across voxels!!
            // IDEA: should autoconvert between the gameplay voxel type and the rendering voxel type
            this.handle = handle.GetInternalVoxel();

            ItemRelativeTransformationProvider = getItemCircleTransformation;
        }

        private Matrix getItemCircleTransformation(int i)
        {
            var radius = handle.GetBoundingBox().Maximum.X - handle.GetBoundingBox().Minimum.X;
            var angle = MathHelper.TwoPi / handle.Data.Inventory.ItemCount * i;
            return Matrix.Translation( new Vector3(Math.Cos(angle).ToF(), 0.5f, Math.Sin(angle).ToF()) * radius);
        }


        public void Show()
        {

        }

        public void Update()
        {
            var center = handle.GetBoundingBox().GetCenter();
            var items = handle.Data.Inventory.Items.ToArray();

            for (int i = 0; i < items.Length; i++)
            {
                var ent = getEntity(i);
                ent.Mesh = items[i].Mesh;
                ent.WorldMatrix = ItemRelativeTransformationProvider(i) * Matrix.Translation(center);
            }

        }

        /// <summary>
        /// If full, returns last entity!
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private Entity getEntity(int i)
        {
            if (i > entityCache.GetUpperBound(0)) i = entityCache.GetUpperBound(0);
            if (entityCache[i] == null)
            {
                entityCache[i] = new Entity();
            }
            return entityCache[i];
        }

        public void Hide()
        {
            foreach (var entity in entityCache.Distinct())
            {
                TW.Data.RemoveObject(entity);
            }
            entityCache = new Entity[entityCache.Length];
        }
    }
}