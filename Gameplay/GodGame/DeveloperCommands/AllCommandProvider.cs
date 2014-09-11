﻿using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Internal;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.IO;

namespace MHGameWork.TheWizards.GodGame.DeveloperCommands
{
    public class AllCommandProvider : DelegateCommandProvider
    {
        private readonly VoxelTypesFactory typesFactory;

        public AllCommandProvider(WorldPersisterService persister, Internal.Model.World world,VoxelTypesFactory typesFactory)
        {
            this.typesFactory = typesFactory;
            addDummy();

            addPersistence(persister, world);

            addCommand("clearinfestation", () =>
                {
                    world.ForEach((v, _) =>
                        {
                            if (v.Type != typesFactory.Get<InfestationVoxelType>()) return;
                            typesFactory.Get<InfestationVoxelType>().CureInfestation(new IVoxelHandle(v));
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
                    world.ForEach((v, _) => { v.ChangeType(typesFactory.Get<LandType>()); });
                    return "World cleared!";
                });
            addCommand("listsaves", () =>
                {
                    var saves = TWDir.GameData.CreateChild("Saves\\GodGame").GetFiles().ToList();
                    var outputstring = String.Join("\n", saves.Select(e => e.Name));
                    return "Saved games: \n" + outputstring;
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