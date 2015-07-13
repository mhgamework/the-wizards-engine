using System.Collections.Generic;
using System.IO;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using System.Linq;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Showcase
{
    /// <summary>
    /// Responsible for creating the showcase scene
    /// </summary>
    public class ShowcaseSceneBuilder
    {
        private DeferredRenderer renderer;
        private ObjImporter importer;
        private OBJInterpreter interpreter;
        private List<PointLight> lights;

        private RAMMesh mesh;

        public void CreateScene(DeferredRenderer renderer)
        {
            this.renderer = renderer;

            loadShowcaseScene();
            clearState();

            loadScene();



        }

        private void loadScene()
        {
            var sub = getAllSubObjects().ToArray();
            var lights = sub.Where(s => s.Material.Name == "StubLight");

            var objects = sub.Except(lights);

            lights.ToList().ForEach(createLights);



            objects.ToList().ForEach(createMeshPart);

            var el = renderer.CreateMeshElement(mesh);

        }

        private void createLights(OBJGroup.SubObject obj)
        {
            var lightPositions = getLightPositions(obj.GetPositions(importer).Select(v => v.dx()).ToArray());

            foreach (var pos in lightPositions)
            {
                var l = renderer.CreatePointLight();
                l.ShadowsEnabled = false;
                l.LightPosition = pos;
                l.LightRadius = 5f;
                l.LightIntensity = 1;
                lights.Add(l);
            }



        }

        /// <summary>
        /// TODO: make this an indicesbuilder
        /// </summary>
        /// <param name="positions"></param>
        /// <returns></returns>
        private IEnumerable<Vector3> getLightPositions(Vector3[] positions)
        {
            // Find duplicate vertices and build indices list
            Dictionary<Vector3, int> ids = new Dictionary<Vector3, int>();
            var indexedPositions = new List<Vector3>();
            int[] indices = new int[positions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                int val;
                if (!ids.TryGetValue(positions[i], out val))
                {
                    val = indexedPositions.Count;
                    ids.Add(positions[i], val);
                    indexedPositions.Add(positions[i]);
                }
                indices[i] = val;
            }


            // Create adjacency information
            List<int>[] adjacency = new List<int>[indexedPositions.Count];
            for (int i = 0; i < adjacency.Length; i++) adjacency[i] = new List<int>();
            for (int i = 0; i < indices.Length; i += 3)
            {
                var i1 = indices[i + 0];
                var i2 = indices[i + 1];
                var i3 = indices[i + 2];
                adjacency[i1].Add(i2);
                adjacency[i1].Add(i3);

                adjacency[i2].Add(i1);
                adjacency[i2].Add(i3);

                adjacency[i3].Add(i1);
                adjacency[i3].Add(i2);

            }

            for ( int i = 0; i < adjacency.Length; i++ ) adjacency[ i ] = adjacency[ i ].Distinct().ToList();

            var ret = new List<Vector3>();



            var connected = new List<int>();

            bool[] visited = new bool[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                connected.Clear();
                findConnectedIndices(indices[i], adjacency, visited, connected);

                if (connected.Count == 0) continue;
                
                ret.Add(getAveragedPosition(connected.Select(index => indexedPositions[index]).ToList()));

            }

            return ret;
        }

        private void findConnectedIndices(int i, List<int>[] adjacency, bool[] visited, List<int> outConnected)
        {
            if (visited[i]) return;

            visited[i] = true;
            outConnected.Add(i);

            foreach (var next in adjacency[i])
                findConnectedIndices(next, adjacency, visited, outConnected);

        }

        private void loadShowcaseScene()
        {
            importer = new ObjImporter();
            importer.AddMaterialFileStream(Path.GetFileName(ShowcaseTest.ShowcaseMTL), File.OpenRead(ShowcaseTest.ShowcaseMTL));
            importer.ImportObjFile(ShowcaseTest.ShowcaseOBJ);
            interpreter = new OBJInterpreter(importer, new RAMTextureFactory());

        }

        private void clearState()
        {
            lights = new List<PointLight>();
            mesh = new RAMMesh();
        }


        private static Vector3 getAveragedPosition(List<Vector3> pos)
        {
            return pos.Aggregate(new Vector3(), (acc, v) => acc + v) / pos.Count;
        }

        private void createMeshPart(OBJGroup.SubObject subObject)
        {
            var part = interpreter.CreateMeshPart(subObject);
            mesh.GetCoreData().Parts.Add(part);
        }

        private IEnumerable<OBJGroup.SubObject> getAllSubObjects()
        {
            return importer.Groups.SelectMany(g => g.SubObjects).ToArray();
        }
    }
}
