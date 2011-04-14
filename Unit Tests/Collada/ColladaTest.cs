using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MHGameWork.TheWizards.Collada.Files;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Collada
{
    [TestFixture]
    public class ColladaTest
    {
        [Test]
        public void TestImportColladaXML()
        {

            MHGameWork.TheWizards.Collada.COLLADA140.COLLADA collada;
            XmlSerializer serializer = null;
            try
            {
                serializer = new XmlSerializer(typeof(MHGameWork.TheWizards.Collada.COLLADA140.COLLADA));

            }
            catch (Exception e)
            {

                throw e;
            }

            using (var fs = ColladaFiles.GetSimplePlaneDAE())
            {
                collada = (MHGameWork.TheWizards.Collada.COLLADA140.COLLADA)serializer.Deserialize(fs);
            }


            //MHGameWork.TheWizards.Collada.COLLADA140.library_geometries libGeom = collada.library_geometries[0];


            //file = new MHGameWork.TheWizards.Collada.ColladaFile("SimplePlane.DAE");
        }
    }
}
