using MHGameWork.TheWizards.CG.Tests.Other;
using NUnit.Framework;

namespace MHGameWork.TheWizards.CG.Tests.Shading
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
