using System;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Gameplay;
using NUnit.Framework;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Ideas
{
    [EngineTest]
    [TestFixture]
    public class UnsortedIdeasTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestWriteDebugText()
        {
           engine.AddSimulator(new BasicSimulator(delegate
               {
                   //TW.Debug.WriteText("Some text"); // This text is added to a text output box, and reset after each frame (like the linemanager)
                   throw new NotImplementedException();
               }));

        }

        [Test]
        public void TestEngineDisposables()
        {
            engine.AddSimulator(new BasicSimulator(delegate
            {
                IDisposable myDisposableOnGameExit; // Add disposable here which should be disposed on exit of the engine (eg a foreground thread)
                //engine.AddDisposable(myDisposableOnGameExit); // This object should be disposed when the engine shuts down
            }));

        }

    }
}