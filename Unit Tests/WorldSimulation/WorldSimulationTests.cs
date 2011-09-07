using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.WorldSimulation;
using MHGameWork.TheWizards.WorldSimulation.Actions;
using MHGameWork.TheWizards.WorldSimulation.Priorities;
using NUnit.Framework;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Tests.WorldSimulation
{
    [TestFixture]
    public class WorldSimulationTests
    {
        [Test]
        public void SinglePriorityTest()
        {
            Seeder seeder = new Seeder(468);
            DX11Game game = new DX11Game();
            
            List<Resource> resources = new List<Resource>();
            BuildingBluePrints print = new BuildingBluePrints();
            print.ConstructionTime = 7;
            print.Requirements.Add(ResourceTypes.Wood, 50f);
            print.RequirementTypes.Add(ResourceTypes.Wood);
            for (int i = 0; i < 5; i++)
            {
                var res = new Resource();
                res.Position = seeder.NextVector3(new Vector3(-200, 0, -200).xna(), new Vector3(200, 0, 200).xna()).dx();
                if (i % 2 == 0) 
                    res.Type = ResourceTypes.Wood;
                else 
                    res.Type = ResourceTypes.Food;
                res.Radius = 1;
                resources.Add(res);

            } 
            List<Creature> creatures = new List<Creature>();
            for (int i = 0; i < 1; i++)
            {
                var creature = new Creature(i);
            creature.Behaviour = new GoblinBehavior(creature,print);
            creature.Position = seeder.NextVector3(new Vector3(-200, 0, -200).xna(), new Vector3(200, 0, 200).xna()).dx();
            //creature.SetProperty(Housing.housingLocation, null);
            creatures.Add(creature);

            }
           
            

            Simulater sim = new Simulater(creatures, resources);
            game.GameLoopEvent += delegate
                                      {

                                          sim.Update(game.Elapsed);

                                          if(game.Keyboard.IsKeyDown(Key.NumberPad2))
                                          {
                                              sim.Update(game.Elapsed);
                                          }
                                          if (game.Keyboard.IsKeyDown(Key.NumberPad3))
                                          {
                                              sim.Update(game.Elapsed);
                                              sim.Update(game.Elapsed);
                                          }






                                          ///////////// Visualisation///////////////
                                          for (int i = 0; i < resources.Count; i++)
                                          {
                                              game.LineManager3D.AddCenteredBox(resources[i].Position,
                                                                                resources[i].ResourceLevel*0.005f,
                                                                                new SlimDX.Color4(1, 0, 0));
                                          }

                                          for (int i = 0; i < creatures.Count; i++)
                                          {
                                              game.LineManager3D.AddCenteredBox(creatures[i].Position,
                                                                                1,
                                                                                new SlimDX.Color4(0, 1, 0));
                                          }
                                          if(creatures[0]!=null)
                                              game.AddToWindowTitle("FoodLevel: " + ((IBellyFillable)creatures[0].Behaviour).FoodLevel + creatures[0].Behaviour.Priorities[0].Priority.ToString() + ": " + creatures[0].Behaviour.Priorities[0].Level + creatures[0].Behaviour.Priorities[0].Priority.ToString() + ": " + creatures[0].Behaviour.Priorities[1].Level);

                                      };
            game.Run();
        }

        [Test]
        public void MultiplePriorityTestPlusBuildingDrawingTest()
        {
            Seeder seeder = new Seeder(468);
            DX11Game game = new DX11Game();

            List<Resource> resources = new List<Resource>();
            List<Building> buildings = new List<Building>();
            BuildingBluePrints print = new BuildingBluePrints();
            print.ConstructionTime = 7;
            print.Requirements.Add(ResourceTypes.Wood, 50f);
            print.RequirementTypes.Add(ResourceTypes.Wood);
            print.Size = 4;
            for (int i = 0; i < 20; i++)
            {
                var res = new Resource();
                res.Position = seeder.NextVector3(new Vector3(-200, 0, -200).xna(), new Vector3(200, 0, 200).xna()).dx();
                if (i % 2 == 0)
                    res.Type = ResourceTypes.Wood;
                else
                    res.Type = ResourceTypes.Food;
                res.Radius = 1;
                resources.Add(res);

            }
            List<Creature> creatures = new List<Creature>();
            for (int i = 0; i <20; i++)
            {
                var creature = new Creature(i);
                creature.Behaviour = new GoblinBehavior(creature, print);
                creature.Position = seeder.NextVector3(new Vector3(-200, 0, -200).xna(), new Vector3(200, 0, 200).xna()).dx();
                //creature.SetProperty(Housing.housingLocation, null);
                creatures.Add(creature);

            }



            Simulater sim = new Simulater(creatures, resources,buildings);
            game.GameLoopEvent += delegate
            {

                sim.Update(game.Elapsed);

                if (game.Keyboard.IsKeyDown(Key.NumberPad2))
                {
                    sim.Update(game.Elapsed);
                }
                if (game.Keyboard.IsKeyDown(Key.NumberPad3))
                {
                    sim.Update(game.Elapsed);
                    sim.Update(game.Elapsed);
                }






                ///////////// Visualisation///////////////
                for (int i = 0; i < resources.Count; i++)
                {
                    game.LineManager3D.AddCenteredBox(resources[i].Position,
                                                      resources[i].ResourceLevel * 0.005f,
                                                      new SlimDX.Color4(1, 0, 0));
                }
                for (int i = 0; i < buildings.Count; i++)
                {
                    game.LineManager3D.AddCenteredBox(buildings[i].Position,
                                                      buildings[i].Size,
                                                      new SlimDX.Color4(0, 0, 1));
                }
                for (int i = 0; i < creatures.Count; i++)
                {
                    game.LineManager3D.AddCenteredBox(creatures[i].Position,
                                                      1,
                                                      new SlimDX.Color4(0, 1, 0));
                }
                if (creatures[0] != null)
                    game.AddToWindowTitle("FoodLevel: " + ((IBellyFillable)creatures[0].Behaviour).FoodLevel + creatures[0].Behaviour.Priorities[0].Priority.ToString() + ": " + creatures[0].Behaviour.Priorities[0].Level + creatures[0].Behaviour.Priorities[0].Priority.ToString() + ": " + creatures[0].Behaviour.Priorities[1].Level);

            };
            game.Run();
        }
    
    }
}
