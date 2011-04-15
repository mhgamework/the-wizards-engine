using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Tests
{
    public static class TestFiles
    {
        public static string MerchantsHouseObj { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\MerchantsHouse.obj"; } }
        public static string MerchantsHouseMtl { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\MerchantsHouse.mtl"; } }

        public static string GuildHouseObj { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\GuildHouse01\GuildHouse01.obj"; } }
        public static string GuildHouseMtl { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\GuildHouse01\GuildHouse01.mtl"; } }

        public static string CrateObj { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\Crate01.obj"; } }
        public static string CrateMtl { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\Crate01.mtl"; } }

        public static string BarrelObj { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\Barrel01.obj"; } }
        public static string BarrelMtl { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\Barrel01.mtl"; } }


        public static string WoodPlanksBareJPG { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\maps\WoodPlanksBare0054_1_S.jpg"; } }
        public static string BrickRoundJPG { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\maps\BrickRound0030_7_S.jpg"; } }

    }
}
