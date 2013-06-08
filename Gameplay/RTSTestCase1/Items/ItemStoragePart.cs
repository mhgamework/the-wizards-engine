using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using System.Linq;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins.Components
{
    [ModelObjectChanged]
    public class ItemStoragePart : EngineModelObject, IObjectPart
    {
        public ItemStoragePart()
        {
            Items = new List<IItem>();
        }

        public IItemStorage Parent { get; set; }
        public BoundingBox ContainerArea { get; set; }

        public List<IItem> Items { get; set; }

        public int Capacity { get; set; }



        public void UpdateItemLocations()
        {
            var min = ContainerArea.Minimum;
            var max = ContainerArea.Maximum;

            var step = 0.45f;

            var iItem = 0;

            for (float y = min.Y; y < max.Y + 100; y += step) // Infinite Y;
                for (float z = min.Z; z < max.Z; z += step)
                    for (float x = min.X; x < max.X; x += step)
                    {
                        if (iItem >= Items.Count) return;

                        ((IPhysical)Items[iItem]).Physical.WorldMatrix = ((IPhysical)Parent).Physical.WorldMatrix *
                                                            Matrix.Translation(x, y, z);
                        iItem++;
                    }
        }

        public bool IsEmpty
        {
            get { return Items.Count == 0; }
            set { }
        }

        public bool IsFull
        {
            get { return Items.Count >= Capacity; }
            set { }
        }
    }
}