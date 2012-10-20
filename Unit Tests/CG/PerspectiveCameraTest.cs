using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerGraphics;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.CG
{
    [TestFixture]
    public class PerspectiveCameraTest
    {
        [Test]
        public void TestGenerateRays()
        {
            PerspectiveCamera.Test();
        }
    }
}
