using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Internal;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.GodGame.Types.Towns.Data;
using MHGameWork.TheWizards.IO;

namespace MHGameWork.TheWizards.GodGame.DeveloperCommands
{
    public class AllCommandProvider : DelegateCommandProvider
    {
        private readonly VoxelTypesFactory typesFactory;
        private readonly ItemTypesFactory itemTypesFactory;
        private readonly GenericDatastore genericDatastore;

        public AllCommandProvider(WorldPersisterService persister,
            Internal.Model.World world,
            VoxelTypesFactory typesFactory,
            ItemTypesFactory itemTypesFactory,
            UserInputProcessingService userInputProcessingService,
            GenericDatastore genericDatastore)
        {
            this.typesFactory = typesFactory;
            this.itemTypesFactory = itemTypesFactory;
            this.genericDatastore = genericDatastore;
            addDummy();

            addPersistence(persister, world);

            addCommand("clearinfestation", () =>
                {
                    world.ForEach((v, _) =>
                        {
                            if (v.Type != typesFactory.Get<InfestationVoxelType>()) return;
                            typesFactory.Get<InfestationVoxelType>().CureInfestation(v);
                        });

                    return "Cleared all infestation!";
                });

            addCommand("helpall", () =>
            {
                return "All Commands: " + string.Join(", ", CommandNames.ToArray());
            });
            addCommand("help", partialCommand =>
                {
                    return "Commands containing '" + partialCommand + "': " + string.Join(", ", CommandNames.Where(c => c.Contains(partialCommand)).ToArray());
                });
            addCommand("clearWorld", () =>
            {
                genericDatastore.ClearAll();
                    world.ForEach((v, _) =>
                        {
                            v.Data.Type = typesFactory.Get<LandType>();
                        });
                    return "World cleared!";
                });
            addCommand("listsaves", () =>
                {
                    var saves = TWDir.GameData.CreateChild("Saves\\GodGame").GetFiles().ToList();
                    var outputstring = String.Join(", ", saves.Select(e => e.Name.Substring(0, e.Name.Length - 4))); // Drop extension
                    return "Saved games: \n" + outputstring;
                });

            addCommand("addresource", (typeStr, amountStr) =>
                {
                    var itemType = itemTypesFactory.AllTypes.FirstOrDefault(t => t.Name.ToLower() == typeStr.ToLower());
                    if (itemType == null) return "Item type not found: " + typeStr;
                    var amount = 0;
                    if (!int.TryParse(amountStr, out amount)) return "Invalid item amount: " + amountStr;

                    var target = userInputProcessingService.GetTargetedVoxel();
                    if (target == null || target.Type != typesFactory.Get<WarehouseType>())
                        return "Not targeting a warehouse";

                    target.Data.Inventory.AddNewItems(itemType,amount);

                    return "Added items!";

                });

        }

        private void addDummy()
        {
            addCommand("play", () => "Playing!");
            addCommand("count", i => "I can count to " + i + "!");
        }

        private void addPersistence(WorldPersisterService persister, Internal.Model.World world)
        {
            addCommand("save", file =>
                {
                    persister.Save(world, TWDir.GameData.CreateChild("Saves\\GodGame").CreateFile(file + ".xml"));
                    return "Saved world to " + "Saves\\GodGame\\" + file + ".xml";
                });
            addCommand("load", file =>
                {
                    persister.Load(world, TWDir.GameData.CreateChild("Saves\\GodGame").CreateFile(file + ".xml"));
                    return "Loaded world to " + "Saves\\GodGame\\" + file + ".xml";
                });
        }
    }
}