using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.GodGame._Engine;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Persistence
{
    /// <summary>
    /// Responsible for converting gameplay level object concepts like itemtypes and voxeltypes to string and back
    /// </summary>
    public class GameplayObjectsSerializer
    {
        private readonly VoxelTypesFactory typesFactory;
        private readonly ItemTypesFactory itemTypesFactory;
        private readonly IEnumerable<IPlayerTool> allTools;

        public GameplayObjectsSerializer(PlayerToolsFactory toolsFactory, VoxelTypesFactory typesFactory, ItemTypesFactory itemTypesFactory)
        {
            this.typesFactory = typesFactory;
            this.itemTypesFactory = itemTypesFactory;
            allTools = toolsFactory.Tools;
        }

        [ExceptionMessage("VoxelType with name not found")]
        public IGameVoxelType GetVoxelType(string name)
        {
            return typesFactory.AllTypes.First(t => t.Name == name);
        }

        [ExceptionMessage("ItemType with name not found")]
        public ItemType GetItemType(string name)
        {
            return itemTypesFactory.AllTypes.First(t => t.Name == name);
        }

        [ExceptionMessage("PlayerTool with name not found")]
        public IPlayerTool GetPlayerTool(string name)
        {
            return allTools.First(t => t.Name == name);
        }

        public string Serialize(IGameVoxelType type)
        {
            return type.Name;
        }

        public string Serialize(ItemType type)
        {
            return type.Name;
        }

        public string Serialize(IPlayerTool tool)
        {
            return tool.Name;
        }
    }
}