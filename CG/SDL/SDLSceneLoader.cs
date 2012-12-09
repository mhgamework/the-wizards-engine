using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MHGameWork.TheWizards.CG.Cameras;
using MHGameWork.TheWizards.CG.GeometricSurfaces;
using MHGameWork.TheWizards.CG.Lighting;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.OBJParser;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;
using MHGameWork.TheWizards.CG.SceneObjects;
using MHGameWork.TheWizards.CG.Shading;

namespace MHGameWork.TheWizards.CG.SDL
{
    public class SDLSceneLoader
    {
        private GenericTraceableScene scene;
        private Dictionary<string, Lighting.PointLight> lights = new Dictionary<string, Lighting.PointLight>();
        private Dictionary<string, ICamera> cameras = new Dictionary<string, ICamera>();
        private Dictionary<string, IGeometry> geometries = new Dictionary<string, IGeometry>();
        private Dictionary<string, RAMTexture> textures = new Dictionary<string, RAMTexture>();
        private Dictionary<string, IShader> materials = new Dictionary<string, IShader>();
        private Sdl data;


        private ICamera mainCamera;
        private MultipleLightProvider lightProvider;

        public ITraceableScene LoadScene(string path)
        {
            var s = new XmlSerializer(typeof(Sdl));
            using (var fs = File.OpenRead(path))
                data = (Sdl)s.Deserialize(fs);

            scene = new GenericTraceableScene();
            lightProvider = new MultipleLightProvider();
            geometries.Clear();
            textures.Clear();
            materials.Clear();
            cameras.Clear();
            lights.Clear();

            loadCamera();

            prepareScene();

            loadLights();
            loadGeometry();
            loadTextures();
            loadMaterials();
            buildScene();

            return scene;
        }

        private void prepareScene()
        {
            mainCamera = cameras[data.Scene.camera];

        }


        private void loadCamera()
        {
            if (data.Cameras == null)
                return;
            foreach (var camData in data.Cameras)
            {
                var cam = new PerspectiveCamera();
                cam.Position = Vector3.Normalize(parseVector3(camData.position));
                cam.Up = Vector3.Normalize(parseVector3(camData.up));
                cam.Direction = Vector3.Normalize(parseVector3(camData.direction));
                //TODO: camData.fovy
                cameras.Add(camData.name, cam);
            }

        }

        private void buildScene()
        {
            foreach (var lightName in data.Scene.lights.Split(' '))
            {
                lightProvider.AddLight(lights[lightName]);
            }
            //TODO: background
            buildSceneItems(Matrix.Identity, data.Scene.Items);
        }

        private void buildSceneItems(Matrix identity, object[] iDatas)
        {
            foreach (var iData in iDatas)
            {
                loadSceneItem((dynamic)iData, identity);
            }
        }

        private void loadSceneItem(Shape iData, Matrix identity)
        {
            if (iData.material != null)
            {
                var geometrySceneObject = new GeometrySceneObject(geometries[iData.geometry], materials[iData.material]);
                scene.AddSceneObject(new TransformedSceneObject(geometrySceneObject) { Transformation = identity });
            }
            else if (iData.texture != null)
            {
                //TODO
            }
        }
        private void loadSceneItem(Translate iData, Matrix identity)
        {
            buildSceneItems(identity * Matrix.Translation(parseVector3(iData.vector)), iData.Items);
        }
        private void loadSceneItem(Rotate iData, Matrix identity)
        {
            buildSceneItems(identity * Matrix.RotationAxis(parseVector3(iData.axis), parseFloat(iData.angle)), iData.Items);
        }
        private void loadSceneItem(Scale iData, Matrix identity)
        {
            buildSceneItems(identity * Matrix.Scaling(parseVector3(iData.scale)), iData.Items);
        }

        private void loadMaterials()
        {
            if (data.Materials == null) return;
            foreach (var mData in data.Materials.Items)
            {
                loadMaterial((dynamic)mData);
            }
        }
        private void loadMaterial(DiffuseMaterial mat)
        {
            var sh = new DiffuseShader(scene, lightProvider);
            materials.Add(mat.name, sh);
        }
        private void loadMaterial(PhongMaterial mat)
        {
            var sh = new PhongShader(scene, mainCamera, lightProvider);
            materials.Add(mat.name, sh);
        }

        private void loadMaterial(LinearCombinedMaterial mat)
        {

        }

        private void loadTextures()
        {
            if (data.Textures == null) return;
            foreach (var tData in data.Textures)
                textures.Add(tData.name, new RAMTexture { DiskFilePath = tData.src });
        }

        private void loadGeometry()
        {
            if (data.Geometry == null) return;
            foreach (var gData in data.Geometry.Items)
            {
                loadGeometry((dynamic)gData);
            }
        }
        private void loadGeometry(Cone gData)
        {
            //TODO
        }
        private void loadGeometry(Cylinder gData)
        {
            //TODO
        }
        private void loadGeometry(IndexedTriangleSet gData)
        {
            Vector3[] positions = parseArray(gData.coordinates, parseVector3);
            int[] positionIndices = parseArray(gData.coordinateIndices, parseInt);

            Vector3[] normals = null;
            int[] normalIndices = null;
            if (gData.normals != null)
            {
                normals = parseArray(gData.normals, parseVector3);
                normalIndices = parseArray(gData.coordinateIndices, parseInt);
            }

            Vector2[] texcoords = null;
            int[] texcoordIndices = null;
            if (gData.textureCoordinates != null)
            {
                texcoords = parseArray(gData.textureCoordinates, parseVector2);
                texcoordIndices = parseArray(gData.coordinateIndices, parseInt);
            }

            var vertices =
                positionIndices.Select(
                    (rrr, i) => new TangentVertex(
                        positions[i],
                        texcoords == null ? texcoords[i] : new Vector2(),
                        normals == null ? normals[i] : new Vector3(),
                        new Vector3())).ToArray();

            if (normals == null)
            {
                //TODO: calc facetted normals or otherwise
            }

            geometries.Add(gData.name, new MeshGeometry(vertices));

        }
        private void loadGeometry(Sphere gData)
        {
            var sphere = new SphereGeometry(parseFloat(gData.radius));
        }
        private void loadGeometry(Teapot gData)
        {
            //TODO
        }
        private void loadGeometry(Torus gData)
        {
            //TODO
        }


        private void loadLights()
        {
            if (data.Lights == null)
                return;
            foreach (var lightData in data.Lights.Items)
            {
                loadLight((dynamic)lightData);
            }

        }
        private void loadLight(DirectionalLight lightData)
        {

        }
        private void loadLight(SpotLight lightData)
        {

        }
        private void loadLight(PointLight lightData)
        {
            var light = new Lighting.PointLight();
            light.Position = parseVector3(lightData.position);
            //TODO
            /*if (lightData.color != null)
                light.Position = parseVector3(lightData.color);
            if (lightData.intensity != null)
                light.Position = parseVector3(lightData.intensity);*/
            lights.Add(lightData.name, light);
        }


        private T[] parseArray<T>(String data, Func<string, T> conversion)
        {
            return data.Split(',').Select(conversion).ToArray();
        }
        private Vector3 parseVector3(String data)
        {
            var input = data.Split(' ').Select(parseFloat);
            return new Vector3(input.ElementAt(0), input.ElementAt(1), input.ElementAt(2));
        }
        private Vector2 parseVector2(String data)
        {
            var input = data.Split(' ').Select(parseFloat);
            return new Vector2(input.ElementAt(0), input.ElementAt(1));
        }
        private Color4 parseColor(String data)
        {
            var input = data.Split(' ').Select(parseFloat);
            return new Color4(input.ElementAt(0), input.ElementAt(1), input.ElementAt(2));
        }
        private float parseFloat(String data)
        {
            return float.Parse(data.Trim());
        }
        private int parseInt(string data)
        {
            return int.Parse(data.Trim());
        }
    }
}
