using MHGameWork.TheWizards.DirectX10;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.DirectX10
{
    [TestFixture]
    public class DirectX10Test
    {
        [Test]
        public void TestRenderTriangle()
        {
            DirectX10Game game = new DirectX10Game();
            game.Run();
        }
    }
}
