using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Entity;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Model
{
    [TestFixture]
    public class ModelTest
    {
        [Test]
        public void TestChangeEntity()
        {
            var ent = new TheWizards.Model.Entity();
            ent.Mesh = new RAMMesh();

        }
    }
}
