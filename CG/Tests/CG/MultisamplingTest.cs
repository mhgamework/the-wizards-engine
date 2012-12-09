using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG;
using MHGameWork.TheWizards.CG.Cameras;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Surfaces;
using MHGameWork.TheWizards.CG.Shading;
using MHGameWork.TheWizards.CG.UI;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.CG
{
    [TestFixture]
    public class MultisamplingTest
    {
        [Test]
        public void TestComplex()
        {
            var scene = new GenericTraceableScene();
            var cam = new PerspectiveCamera();
            cam.Position = new Vector3(0, 1, 0);
            cam.ProjectionPlaneDistance = 1.3f;


            var shader = new PhongShader(scene, cam);

            scene.AddGenericSurface(new PlaneGeometricSurface(shader, new Plane(Vector3.UnitY, 0)));
            scene.AddGenericSurface(new SphereGeometricSurface(shader, new BoundingSphere(new Vector3(0, 1, 0 -5f), 1f)));

            var tracedSceneImage = new TracedSceneImage(scene, cam);

            var multisample = new MultisampledImage(tracedSceneImage, new Vector2(1280, 720),
                                                    new TheWizards.CG.Sampling.JitteredSampler(4));

            var window = new GraphicalRayTracer(multisample, 1);


        }
    }
}
