using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.World
{
    [TestFixture]
    public class WorldTest
    {
        [Test]
        public void TestSaveWorld()
        {
            TheWizards.World.World world = new TheWizards.World.World();

            var ent = world.Create<SimpleWorldEntity>();

            ent.Health = 1000;
            ent.Position = new Microsoft.Xna.Framework.Vector3(1, 2, 3);
            ent.Orientation = Matrix.Identity;

            ent = world.Create<SimpleWorldEntity>();

            ent.Health = 2;
            ent.Position = new Microsoft.Xna.Framework.Vector3(1, 100, 3);
            ent.Orientation = Matrix.Identity;

            string dataDir = System.Windows.Forms.Application.StartupPath + "\\Test\\TestSaveWorld";

            world.Save(new DirectoryInfo(dataDir));
        }

        [Test]
        public void TestLoadWorld()
        {
            string dataDir = System.Windows.Forms.Application.StartupPath + "\\Test\\TestSaveWorld";

            TheWizards.World.World world = new TheWizards.World.World();
            world.Load(new DirectoryInfo(dataDir));
        }
    }
}
