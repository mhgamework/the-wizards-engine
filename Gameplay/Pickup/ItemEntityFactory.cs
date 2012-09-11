using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Pickup
{
    public class ItemEntityFactory
    {
        public List<ItemEntity> ItemList = new List<ItemEntity>();

        public void AddItemEntity(ItemEntity it)
        {
            ItemList.Add(it);
        }

        public ItemEntity CreateItemEntity(IMesh mesh, Matrix matrix)
        {
            var it = new ItemEntity();
            it.SetMesh(mesh);
            it.SetPosition(matrix);
            it.SetVisibility(true);
            it.RayTraceable = true;

            AddItemEntity(it);
            return it;
        }

        public void RemoveItemEntity(ItemEntity it)
        {
            if (!ItemList.Contains(it))
                return;

            it.Delete();
            ItemList.Remove(it);
        }

    }
}
