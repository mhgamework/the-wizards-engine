using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Gameplay
{
    [TestFixture]
    public class EngineTest
    {
        [Test]
        public void StartEngine()
        {
            var eng = new Engine.TWEngine();
            eng.HotloadingEnabled = true;
            eng.Start();
        }


    }
}
