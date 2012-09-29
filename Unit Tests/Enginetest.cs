using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ModelContainer;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests
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
