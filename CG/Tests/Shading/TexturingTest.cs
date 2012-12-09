using NUnit.Framework;

namespace MHGameWork.TheWizards.CG.Tests
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
