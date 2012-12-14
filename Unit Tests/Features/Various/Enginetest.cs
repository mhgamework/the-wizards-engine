using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Various
{
    [TestFixture]
    public class EngineTest
    {
        [Test]
        public void StartEngine()
        {
            var eng = new Engine.TWEngine();
            eng.Start();
        }


    }
}
