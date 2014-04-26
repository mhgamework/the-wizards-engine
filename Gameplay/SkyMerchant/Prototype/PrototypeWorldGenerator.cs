using System;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype
{
    public class PrototypeWorldGenerator
    {
        #region Injection
        public PrototypeObjectsFactory Factory { get; set; }
        public Random Random { get; set; }
        #endregion

        public PrototypeWorldGenerator(PrototypeObjectsFactory factory, Random random)
        {
            Factory = factory;
            Random = random;
        }


        public void GenerateWorld(float size)
        {
            var worldMin = -new Vector3(size);
            var worldMax = new Vector3(size);
            worldMin.Y = 0;
            worldMax.Y = 20;

            var density = 1 / (100f);

            for (int i = 0; i < size * size * density; i++)
            {
                var n = Factory.CreateIsland();
                n.Seed = Random.Next(3);
                n.Physical.Position = nextVector3(worldMin, worldMax);
                n.TargetHeight = n.Physical.Position.Y;


                populateIsland(n);
            }
            density = 1 / 100f;
            //for (int i = 0; i < size * size * density; i++)
            for (int i = 0; i < density; i++)
            {
                var robot = Factory.CreateDrone();
                robot.Physical.Position = nextVector3(worldMin, worldMax);
                robot.GuardPosition = robot.Physical.Position;
            }

            density = 1 / 100f;
            //for (int i = 0; i < size * size * density; i++)
            for (int i = 0; i < density; i++)
            {
                var robot = Factory.CreatePirate();
                robot.Physical.Position = nextVector3(worldMin, worldMax);
            }
        }

        private void populateIsland(IslandPart n)
        {
            if (Random.NextDouble() < 0.1f)
            {
                var tViz = Factory.CreateTrader();
                var t = tViz.TraderPart;
                t.GivesAmount = 1;
                t.GivesType = Factory.CogType;

                t.WantsAmount = 5;
                t.WantsType = Factory.WoodType;

                tViz.PositionComponent.Position = n.Physical.Position;
                
            }
            ItemPart item;
            while (Random.NextDouble() < 0.1f)
            {
                item = Factory.CreateCog();
                item.PlaceOnIsland(n);
            }

            while (Random.NextDouble() < 0.2f)
            {
                item = Factory.CreateTube();
                item.PlaceOnIsland(n);
            }

            if (Random.NextDouble() < 0.4f)
            {
                var source = Factory.CreateTree();
                source.Physical.Position = n.Physical.Position;
            }
        }

        private Vector3 nextVector3(Vector3 min, Vector3 max)
        {
            return new Vector3(nextFloat(min.X, max.X), nextFloat(min.Y, max.Y), nextFloat(min.Z, max.Z));
        }
        private float nextFloat(float min, float max)
        {
            return (float)Random.NextDouble() * (max - min) + min;
        }
    }
}