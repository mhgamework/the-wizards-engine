using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MHGameWork.TheWizards.CG.Cameras;
using MHGameWork.TheWizards.CG.GeometricSurfaces;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.OBJParser;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG.SDL
{
    public class SDLSceneLoader
    {
        private GenericTraceableScene scene;
        private Dictionary<string, Shading.PointLight> lights = new Dictionary<string, Shading.PointLight>();
        private Dictionary<string, ICamera> cameras = new Dictionary<string, ICamera>();
        private Dictionary<string, IGeometricSurface> geometries = new Dictionary<string, IGeometricSurface>();
        private Dictionary<string, RAMTexture> textures = new Dictionary<string, RAMTexture>();
        private Dictionary<string, Material> materials = new Dictionary<string, Material>();
        private Sdl data;

        public ITraceableScene LoadScene(string path)
        {
            var s = new XmlSerializer(typeof(Sdl));
            using (var fs = File.OpenRead(path))
                data = (Sdl)s.Deserialize(fs);

            scene = new GenericTraceableScene();
            geometries.Clear();
            textures.Clear();
            materials.Clear();
            cameras.Clear();
            lights.Clear();

            loadCamera();
            loadLights();
            loadGeometry();
            loadTextures();
            loadMaterials();
            buildScene();

            return scene;
        }

        private void buildScene()
        {
            throw new NotImplementedException();
        }

        private void loadMaterials()
        {
            throw new NotImplementedException();
        }

        private void loadTextures()
        {
            throw new NotImplementedException();
        }

        private void loadGeometry()
        {
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
            //TODO
        }
        private void loadGeometry(Sphere gData)
        {
            var sphere = new SphereGeometricSurface(parseFloat(gData.radius));
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
            var light = new Shading.PointLight();
            light.Position = parseVector3(((PointLight)lightData).position);
            light.Position = parseVector3(((PointLight)lightData).color);
            light.Position = parseVector3(((PointLight)lightData).intensity);
            lights.Add(((PointLight)lightData).name, light);
        }

        private void loadCamera()
        {
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

        private Vector3 parseVector3(String data)
        {
            var input = data.Split().Select(str => float.Parse(str));
            return new Vector3(input.ElementAt(0), input.ElementAt(1), input.ElementAt(2));
        }
        private Color4 parseColor(String data)
        {
            var input = data.Split().Select(str => float.Parse(str));
            return new Color4(input.ElementAt(0), input.ElementAt(1), input.ElementAt(2));
        }
        private float parseFloat(String data)
        {
            return float.Parse(data);
        }
    }
}
