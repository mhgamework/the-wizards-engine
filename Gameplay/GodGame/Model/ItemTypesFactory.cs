using System;
using System.Collections.Generic;
using System.Drawing;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Model
{
    /// <summary>
    /// Responsible for creating all items
    /// </summary>
    public class ItemTypesFactory
    {
        private readonly Internal.Model.World world;
        private List<PlayerTool> tools = new List<PlayerTool>();
        public IEnumerable<PlayerTool> Tools { get { return tools; } }

        private List<ItemType> rawTypes = new List<ItemType>();

        public ItemTypesFactory(Internal.Model.World world)
        {
            this.world = world;

            CropType = CreateItemType("Crop", Color.Orange);
            FishType = CreateItemType("Fish", Color.DeepSkyBlue);
            WoodType = CreateItemType("Wood", Color.SaddleBrown);
            PigmentType = CreateItemType("Pigment", Color.DarkViolet);
            CrystalType = CreateItemType("Crystal", Color.LightBlue);
            StoneType = CreateItemType("Stone", Color.Gray);

            rawTypes.ForEach(raw => createKanbanTypes(raw));

        }

        public ItemType CropType { get; private set; }
        public ItemType FishType { get; private set; }
        public ItemType WoodType { get; private set; }
        public ItemType PigmentType { get; private set; }
        public ItemType CrystalType { get; private set; }
        public ItemType StoneType { get; private set; }


        private ItemType CreateItemType(string name, Color color)
        {
            if (rawTypes.Any(t => t.Name == name)) throw new InvalidOperationException("Duplicate item name!!");
            var ret = new ItemType { Name = name, Mesh = UtilityMeshes.CreateBoxColored(color, new Vector3(1)) };
            rawTypes.Add(ret);
            return ret;
        }

        public IEnumerable<ItemType> AllTypes
        {
            get
            {
                return rawTypes
                    .Union(kanbanTypes.Select(t => t.IncomingKanban))
                    .Union(kanbanTypes.Select(t => t.OutgoingKanban));
            }
        }


        private List<ItemTypeKanbans> kanbanTypes = new List<ItemTypeKanbans>();


        #region Kanban Construction

        public ItemType GetIncomingKanban(ItemType type)
        {
            if (kanbanTypes.All(e => e.ItemType != type))
                return createKanbanTypes(type).IncomingKanban;

            return kanbanTypes.First(e => e.ItemType == type).IncomingKanban;
        }

        public ItemType GetOutgoingKanban(ItemType type)
        {
            if (kanbanTypes.All(e => e.ItemType != type))
                return createKanbanTypes(type).OutgoingKanban;

            return kanbanTypes.First(e => e.ItemType == type).OutgoingKanban;
        }

        private ItemTypeKanbans createKanbanTypes(ItemType type)
        {
            var ret = new ItemTypeKanbans
            {
                ItemType = type,
                IncomingKanban = new ItemType { Name = type.Name + "IncomingKanban", Mesh = UtilityMeshes.CreateBoxColored(Color.Green, new Vector3(0.5f)) },
                OutgoingKanban = new ItemType { Name = type.Name + "OutgoingKanban", Mesh = UtilityMeshes.CreateBoxColored(Color.Red, new Vector3(0.5f)) }
            };
            kanbanTypes.Add(ret);
            return ret;
        }

        /// <summary>
        /// Returns whether give itemtype is real or a kanban version.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsKanban(ItemType type)
        {
            return kanbanTypes.Any(e => e.IncomingKanban == type || e.OutgoingKanban == type);
        }

        /// <summary>
        /// Returns true if type (or one of its kanban variants) equals type01 (or one of its kanban variants).
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsItemOrKanbanOfType(ItemType type, ItemType type01)
        {
            var ret = type == type01 || type == GetIncomingKanban(type01) ||
                   type == GetOutgoingKanban(type01);

            if (ret)
                return true;

            return type01 == type || type01 == GetIncomingKanban(type) ||
                   type01 == GetOutgoingKanban(type);
        }

        #endregion Kanban Construction



        private struct ItemTypeKanbans
        {
            public ItemType ItemType;
            public ItemType IncomingKanban;
            public ItemType OutgoingKanban;
        }
    }


}