using MHGameWork.TheWizards.CG.Cameras;
using MHGameWork.TheWizards.CG.GeometricSurfaces;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.SceneObjects;
using MHGameWork.TheWizards.CG.Shading;
using MHGameWork.TheWizards.CG.UI;
using NUnit.Framework;

namespace MHGameWork.TheWizards.CG.Tests
{
    [TestFixture]
    public class RefractionShaderTest
    {

        [Test]
        public void TestComplex()
        {
            var scene = new ComplexTestScene();

            var shader = new RefractionShader(scene.Scene);
            var sphere2 = createSphereObject(shader, new BoundingSphere(new Vector3(0, 1, 0 - 12f), 1f), false);
            scene.Scene.AddSceneObject(sphere2);

            var sphere3 = createSphereObject(shader, new BoundingSphere(new Vector3(-4, 1, 0 - 14f), 1f), false);
            scene.Scene.AddSceneObject(sphere3);



            var window = new GraphicalRayTracer(new TracedSceneImage(scene.Scene, scene.Camera));
        }

        private ISceneObject createSphereObject(RefractionShader shader, BoundingSphere p1, bool castsShadows = true)
        {
            var surface = new SphereGeometricSurface(p1.Radius);
            var ret = new TransformedSceneObject(new GeometrySceneObject(surface, shader)) { Transformation = Matrix.Translation(p1.Center) };
            return ret;
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
            scene.AddSceneObject(new GeometrySceneObject(new PlaneGeometricSurface(new Plane(Vector3.UnitY, 0)), p));

            var shader = new RefractionShader(scene);
            var sphere2 = createSphereObject(shader, new BoundingSphere(new Vector3(0, 1, 0 - 12f), 1f), false);
            scene.AddSceneObject(sphere2);



            var window = new GraphicalRayTracer(new TracedSceneImage(scene, Camera), 1);
        }
    }
}
