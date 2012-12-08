using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.CG
{
    [TestFixture]
    public class TexturingTest
    {
        [Test]
        public void TestTexturedBarrel()
        {
            var f = new CGFactory();

            f.CreateMeshWithGridSurface(TWDir.GameData + "\\Core\\barrel.obj");

            f.Run();




        }
    }
}
