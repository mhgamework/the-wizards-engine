using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Persistence
{
    /// <summary>
    /// Responsible for converting gameplay level object concepts like itemtypes and voxeltypes to string and back
    /// </summary>
    public class GameplayObjectsSerializer
    {
        private readonly IEnumerable<IPlayerTool> allTools;

        public GameplayObjectsSerializer(PlayerToolsFactory toolsFactory)
        {
            allTools = toolsFactory.Tools;
        }

        public GameVoxelType GetVoxelType(string name)
        {
            return GameVoxelType.AllTypes.First(t => t.Name == name);
            
        }

        public ItemType GetItemType(string name)
        {
            return GameVoxelType.Ore.GetOreItemType(null);
        }

        public IPlayerTool GetPlayerTool(string name)
        {
            return allTools.First(t => t.Name == name);
        }

        public string Serialize(GameVoxelType type)
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