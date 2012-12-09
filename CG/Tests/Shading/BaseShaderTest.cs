using MHGameWork.TheWizards.CG.GeometricSurfaces;
using MHGameWork.TheWizards.CG.Lighting;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.SceneObjects;
using MHGameWork.TheWizards.CG.Shading;
using MHGameWork.TheWizards.CG.Tests.Other;
using NUnit.Framework;

namespace MHGameWork.TheWizards.CG.Tests.Shading
{
    [TestFixture]
    public class BaseShaderTest
    {
        [Test]
        public void TestNormalShader()
        {
            var f = new CGFactory();
            SetupScene(f, new NormalsShader());


            f.Run();
        }

        [Test]
        public void TestDiffuseShader()
        {
            var f = new CGFactory();
            BaseShaderTest.SetupScene(f, new DiffuseShader(f.GetScene(), new SimpleLightProvider()));
            f.Run();
        }
        [Test]
        public void TestSpecularShader()
        {
            var f = new CGFactory();
            BaseShaderTest.SetupScene(f, new SpecularShader(f.GetScene(),f.GetCamera(), new SimpleLightProvider()));
            f.Run();
        }

        [Test]
        public void TestPhongShader()
        {
            var f = new CGFactory();
            BaseShaderTest.SetupScene(f, new PhongShader(f.GetScene(), f.GetCamera(), new SimpleLightProvider()));
            f.Run();
        }
        [Test]
        public void TestSingle()
        {
            var f = new CGFactory();
            BaseShaderTest.SetupScene(f, new RefractionShader(f.GetScene()));
            f.Run();
        }

        public static void SetupScene(CGFactory f, IShader shader)
        {
            var Camera = f.CreatePerspectiveCamera(new Vector3(0, 4, 10), new Vector3(0, 1, 0));
            Camera.ProjectionPlaneDistance = 1.3f;

            var scene = f.GetScene();

            f.AddGroundPlane(-1);

            var sphere2 = new GeometrySceneObject(new SphereGeometry(1), shader);
            sphere2.CastsShadows = false;
            scene.AddSceneObject(new TransformedSceneObject(sphere2) { Transformation = Matrix.Translation(0, 1, -1) });
        }
    }
}
