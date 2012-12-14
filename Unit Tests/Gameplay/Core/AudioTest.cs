using MHGameWork.TheWizards.Audio;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.Tests.Gameplay.Various;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Gameplay.Core
{
    [TestFixture]
    public class AudioTest
    {
        [Test]
        public void TestSoundEmitter()
        {
            var engine = new LocalGame();
            engine.AddSimulator(new AudioSimulator());

            var emitter = new SoundEmitter();
            emitter.Sound = SoundFactory.Load("Core\\TestSound.wav");
            emitter.Playing = true;
            emitter.Loop = true;


            emitter = new SoundEmitter();
            emitter.Sound = SoundFactory.Load("Core\\TestSoundAmbient.wav");
            emitter.Playing = true;

            engine.Run();

        }
    }
}
