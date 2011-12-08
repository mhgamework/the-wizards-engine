using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Model;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Model
{
    [TestFixture]
    public class ModelTest
    {
        [Test]
        public void TestChangeEntity()
        {
            var container = new ModelContainer();

            var ent = new TheWizards.Model.Entity(container);
            ent.Mesh = new RAMMesh();

            container.AddObject(ent);

            int length;
            ModelContainer.ObjectChange[] array;
            container.GetEntityChanges(out array, out length);



            Assert.AreEqual(1,length);
            Assert.AreEqual(ent, array[0].ModelObject);
            

        }




     
    }
}
