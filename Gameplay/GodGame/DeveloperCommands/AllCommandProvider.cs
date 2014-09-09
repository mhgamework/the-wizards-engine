﻿using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Internal;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.IO;

namespace MHGameWork.TheWizards.GodGame.DeveloperCommands
{
    public class AllCommandProvider : DelegateCommandProvider
    {
        public AllCommandProvider(WorldPersisterService persister, Internal.Model.World world)
        {
            addDummy();

            addPersistence(persister, world);

            addCommand("clearinfestation", () =>
                {
                    world.ForEach((v, _) =>
                        {
                            if (v.Type != GameVoxelType.Infestation) return;
                            GameVoxelType.Infestation.CureInfestation(new IVoxelHandle(v));
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