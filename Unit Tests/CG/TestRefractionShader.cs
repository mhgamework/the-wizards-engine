using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Shading;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.CG
{
    [TestFixture]
    public class TestRefractionShader
    {
        [Test]
        public void TestComplex()
        {
            var scene = new ComplexTestScene();

            var shader = new RefractionShader(scene.Scene);
            var sphere2 = new SphereSurface(shader, new BoundingSphere(new Vector3(0, 1, 0 - 12f), 1f));
            sphere2.DrawsShadows = false;
            scene.Scene.AddGenericSurface(sphere2);

            var sphere3 = new SphereSurface(shader, new BoundingSphere(new Vector3(-4, 1, 0 - 14f), 1f));
            sphere3.DrawsShadows = false;
            scene.Scene.AddGenericSurface(sphere3);

            

            var window = new GraphicalRayTracer( new TracedSceneImage(scene.Scene, scene.Camera), 1);
        }
    }
}
