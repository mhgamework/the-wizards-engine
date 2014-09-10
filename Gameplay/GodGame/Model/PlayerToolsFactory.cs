using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Types;

namespace MHGameWork.TheWizards.GodGame.Model
{
    /// <summary>
    /// Responsible for creating all player tools
    /// </summary>
    public class PlayerToolsFactory
    {
        private readonly Internal.Model.World world;
        private readonly PlayerState state;
        private List<IPlayerTool> tools = new List<IPlayerTool>();
        public IEnumerable<IPlayerTool> Tools { get { return tools; } }

        public PlayerToolsFactory(Internal.Model.World world, PlayerState state)
        {
            this.world = world;
            this.state = state;
            tools.AddRange(createPlayerInputs());
        }


        private IEnumerable<IPlayerTool> createPlayerInputs()
        {
            yield return new CreateLandTool(world);
            yield return new ChangeHeightTool(world);
            yield return createTypeInput(GameVoxelType.Forest);
            yield return createTypeInput(GameVoxelType.Village);
            yield return createTypeInput(GameVoxelType.Warehouse);
            yield return createTypeInput(GameVoxelType.Infestation);
            yield return createTypeInput(GameVoxelType.Monument);
            yield return createTypeInput(GameVoxelType.Water);
            yield return createTypeInput(GameVoxelType.Hole);
            yield return createOreInput();
            yield return createTypeInput(GameVoxelType.Miner);
            yield return createTypeInput(GameVoxelType.Road);
            yield return createTypeInput(GameVoxelType.Crop);
            yield return createTypeInput(GameVoxelType.Farm);
            yield return createTypeInput(GameVoxelType.Market);
            yield return createTypeInput(GameVoxelType.MarketBuildSite, "MarketBuildSite");
            yield return createTypeInput(GameVoxelType.Fishery);
            yield return createTypeInput(GameVoxelType.FisheryBuildSite, "FisheryBuildSite");
            yield return createTypeInput(GameVoxelType.Woodworker);
            yield return createTypeInput(GameVoxelType.Quarry);
            yield return createTypeInput(GameVoxelType.Grinder);
            yield return new LightGodPowerTool();
        }

        private static IPlayerTool createTypeInput(GameVoxelType type, string name)
        {
            return new DelegatePlayerTool(name,
                v => v.ChangeType(GameVoxelType.Land),
                v =>
                {
                    if (v.Type == GameVoxelType.Land)
                        v.ChangeType(type);
                });
        }
        private static IPlayerTool createTypeInput(GameVoxelType type)
        {
            return createTypeInput(type, type.Name);
        }
        private static IPlayerTool createOreInput()
        {
            return new DelegatePlayerTool(GameVoxelType.Ore.Name,
                v => v.ChangeType(GameVoxelType.Land),
                v =>
                {
                    if (v.Type == GameVoxelType.Land)
                    {
                        v.ChangeType(GameVoxelType.Ore);
                        v.Data.DataValue = 20;
                    }
                });
        }

    }


}