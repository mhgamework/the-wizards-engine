using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTS;
using MHGameWork.TheWizards.RTS.Commands;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// This testclass contains tests that demonstrate bugs in the engine, add found bugs here when you post them to the bugtracker
    /// </summary>
    [TestFixture]
    [EngineTest]
    public class BugTests
    {
        /// <summary>
        /// TODO: fix
        /// </summary>
        [Test]
        public void TestRenderingBug()
        {
            throw new NotImplementedException("ReEnable!");
          //new GoblinControlTest().TestCommunicateBig();
        }
    
    }
}
