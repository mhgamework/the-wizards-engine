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

        [Test]
        public void TestSingle()
        {
            var Camera = new PerspectiveCamera();
            Camera.Position = new Vector3(0, 4, -5);
            Camera.Direction = Vector3.Normalize(new Vector3(0, 1, 0 - 15f) - Camera.Position);
            Camera.ProjectionPlaneDistance = 1.3f;

            var scene = new GenericTraceableScene();

            PhongShader p;

            p = new PhongShader(scene, Camera);
            scene.AddGenericSurface(new PlaneSurface(p, new Plane(Vector3.UnitY, 0)));

            var shader = new RefractionShader(scene);
            var sphere2 = new SphereSurface(shader, new BoundingSphere(new Vector3(0, 1, 0 - 12f), 1f));
            sphere2.DrawsShadows = false;
            scene.AddGenericSurface(sphere2);



            var window = new GraphicalRayTracer(new TracedSceneImage(scene, Camera), 1);
        }
    }
}
