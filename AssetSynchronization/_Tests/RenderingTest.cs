using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Default;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.XNA
{
    [TestFixture]
    public class RenderingTest
    {
        [Test]
        public void TestDiskRenderingAssetFactory()
        {
            var factory = new DiskRenderingAssetFactory();
            factory.SaveDir = TWDir.Test.CreateSubdirectory("Rendering\\DiskFactory");
            var mesh = DefaultMeshes.CreateGuildHouseMesh(new OBJToRAMMeshConverter(factory));
            factory.AddAsset(mesh);

            factory.SaveAllAssets();

            factory = new DiskRenderingAssetFactory();
            factory.SaveDir = TWDir.Test.CreateSubdirectory("Rendering\\DiskFactory");

            var loadMesh = factory.GetMesh(mesh.Guid);

        }
       
    }
}
