using System.Linq;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Persistence
{
    /// <summary>
    /// Responsible for converting gameplay level object concepts like itemtypes and voxeltypes to string and back
    /// </summary>
    public class GameplayObjectsSerializer
    {
        public GameVoxelType GetVoxelType(string name)
        {
            return GameVoxelType.AllTypes.First(t => t.Name == name);
            
        }

        public ItemType GetItemType(string name)
        {
            return GameVoxelType.Ore.GetOreItemType(null);
        }

        public string Serialize(GameVoxelType type)
        {
            return type.Name;
        }

        public string Serialize(ItemType type)
        {
            return type.Name;
        }
    }
}