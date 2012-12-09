using MHGameWork.TheWizards.CG.Cameras;

namespace MHGameWork.TheWizards.CG.Tests.Other
{
    public class ComplexTestScene
    {
        public GenericTraceableScene Scene;
        public PerspectiveCamera Camera;

        public ComplexTestScene()
        {
            //Scene = new GenericTraceableScene();
            //Camera = new PerspectiveCamera();
            //Camera.Position = new Vector3(0, 4, -5);
            //Camera.Direction = Vector3.Normalize(new Vector3(0, 1, 0 - 15f) - Camera.Position);
            //Camera.ProjectionPlaneDistance = 1.3f;


            //PhongShader shader;

            //shader = new PhongShader(Scene, Camera);
            //Scene.AddGenericSurface(new PlaneGeometricSurface(new Plane(Vector3.UnitY, 0)));

            //shader = new PhongShader(Scene, Camera);
            //shader.Diffuse = new Color4(1, 0, 0);
            //var sphere1 = createSphereObject(shader, new BoundingSphere(new Vector3(1.5f, 1, 1.5f - 15f), 1f));

            //shader = new PhongShader(Scene, Camera);
            //shader.Diffuse = new Color4(0, 0.8f, 0);
            //var sphere2 = createSphereObject(shader,new BoundingSphere(new Vector3(0, 1, 0 - 15f), 1f));


            //shader = new PhongShader(Scene, Camera);
            //var sphere3 = createSphereObject(shader,new BoundingSphere(new Vector3(-2, 1, 1 - 15f), 1f));


            //Scene.AddSceneObject(sphere3);
            //Scene.AddSceneObject(sphere2);
            //Scene.AddSceneObject(sphere1);
        }

        //private ISceneObject createSphereObject(IShader shader, BoundingSphere p1, bool castsShadows = true)
        //{
        //    var surface = new SphereGeometricSurface(p1.Radius);
        //    var ret = new TransformedSceneObject(new GeometrySceneObject { Shader = shader, GeometricSurface = surface }) { Transformation = Matrix.Translation(p1.Center) };
        //    return ret;
        //}
    }
}
