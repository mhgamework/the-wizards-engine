using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic
{
    class CrystalSpawner
    {
        public Vector3 MinPosition { get; set; }
        public Vector3 MaxPosition { get; set; }
        private float nextTime;
        private Random randomizer = new Random();
        private static float SpawnDensity = 0;
        public CrystalSpawner()
        {
            MinPosition = new Vector3(-20, 0, -20);
            MaxPosition = new Vector3(20, 0, 20);
        }

        public IEnumerable<ICrystal> DoSpawn(IEnergyDensityExpert densityExpert, float elapsedTime)
        {
            var retList = new List<ICrystal>();
            nextTime += elapsedTime;
            if (nextTime < 1)
                return retList;
            nextTime -= 1;
            var randPosition = new Vector3(randomizer.Next((int)MinPosition.X, (int)MaxPosition.X),
                                           randomizer.Next((int)MinPosition.Y, (int)MaxPosition.Y),
                                           randomizer.Next((int)MinPosition.Z, (int)MaxPosition.Z));
            //float density = densityExpert.GetDensity(randPosition);
            float density = 0;
            if (density <= SpawnDensity)
            {
                //Console.WriteLine("found density " + density + "  at " + randPosition.X + "," + randPosition.Z);
                retList.Add(createCrystal(randPosition));
            }

            return retList;
        }
        private ICrystal createCrystal(Vector3 position)
        {
            return new SimpleCrystal { Position = position, Capacity = 1000};
        }
    }
}
