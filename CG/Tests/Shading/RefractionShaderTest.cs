﻿using MHGameWork.TheWizards.CG.Cameras;
using MHGameWork.TheWizards.CG.GeometricSurfaces;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.SceneObjects;
using MHGameWork.TheWizards.CG.Shading;
using MHGameWork.TheWizards.CG.Tests.Other;
using MHGameWork.TheWizards.CG.UI;
using NUnit.Framework;

namespace MHGameWork.TheWizards.CG.Tests.Shading
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
            var surface = new SphereGeometry(p1.Radius);
            var ret = new TransformedSceneObject(new GeometrySceneObject(surface, shader) { CastsShadows = castsShadows }) { Transformation = Matrix.Translation(p1.Center) };
            return ret;
        }

      

    }
}
