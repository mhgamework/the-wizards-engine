using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG;
using MHGameWork.TheWizards.CG.Cameras;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Surfaces;
using MHGameWork.TheWizards.CG.Shading;

namespace MHGameWork.TheWizards.Tests.CG
{
    public class ComplexTestScene
    {
        public GenericTraceableScene Scene;
        public PerspectiveCamera Camera;

        public ComplexTestScene()
        {
            Scene = new GenericTraceableScene();
            Camera = new PerspectiveCamera();
            Camera.Position = new Vector3(0, 4, -5);
            Camera.Direction = Vector3.Normalize(new Vector3(0, 1, 0 - 15f) - Camera.Position);
            Camera.ProjectionPlaneDistance = 1.3f;


            PhongShader shader;

            shader = new PhongShader(Scene, Camera);
            Scene.AddGenericSurface(new PlaneSurface(shader, new Plane(Vector3.UnitY, 0)));

            shader = new PhongShader(Scene, Camera);
            shader.Diffuse = new Color4(1, 0, 0);
            var sphere1 = new SphereSurface(shader, new BoundingSphere(new Vector3(1.5f, 1, 1.5f - 15f), 1f));

            shader = new PhongShader(Scene, Camera);
            shader.Diffuse = new Color4(0, 0.8f, 0);
            var sphere2 = new SphereSurface(shader, new BoundingSphere(new Vector3(0, 1, 0 - 15f), 1f));


            shader = new PhongShader(Scene, Camera);
            var sphere3 = new SphereSurface(shader, new BoundingSphere(new Vector3(-2, 1, 1 - 15f), 1f));


            Scene.AddGenericSurface(sphere3);
            Scene.AddGenericSurface(sphere2);
            Scene.AddGenericSurface(sphere1);
        }
    }
}
