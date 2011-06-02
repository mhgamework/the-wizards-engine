using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;
using Scene = StillDesign.PhysX.Scene;

namespace MHGameWork.TheWizards.Physics
{
    /// <summary>
    /// IMPORTANT!!!! CALL UPDATE
    /// </summary>
    public class MeshPhysicsPool
    {
        private Dictionary<MeshCollisionData.TriangleMeshData, TriangleMesh> triangleMeshEntries =
            new Dictionary<MeshCollisionData.TriangleMeshData, TriangleMesh>();

        private Dictionary<MeshCollisionData.Convex, ConvexMesh> convexMeshEntries =
            new Dictionary<MeshCollisionData.Convex, ConvexMesh>();

        private Queue<MeshCollisionData.TriangleMeshData> preloadQueue = new Queue<MeshCollisionData.TriangleMeshData>();

        private StillDesign.PhysX.Scene usingScene;
        private MeshCollisionData.TriangleMeshData currentPreloadingMesh;

        private Stream cookedStream;

        public MeshPhysicsPool()
        {
            var t = new Thread(preloadJob);
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// NOTE: should MeshCollisionData.TriangleMeshData be used here, or just IMESH?
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public TriangleMesh CreateTriangleMesh(StillDesign.PhysX.Scene scene, MeshCollisionData.TriangleMeshData model)
        {
            TriangleMesh ret;
            lock (this)
            {

                if (triangleMeshEntries.TryGetValue(model, out ret)) return ret;
                if (currentPreloadingMesh != model)
                {
                    //Insta-load the model!!!
                    ret = LoadTriangleMesh(model, scene, Matrix.Identity);
                    triangleMeshEntries[model] = ret;
                }

                //Wait for preload to finish
                while (currentPreloadingMesh == model)
                {
                    //Still update, otherwise deadlock
                    Update(scene);

                    Monitor.Wait(this);
                }

                ret = triangleMeshEntries[model];
            }
            return ret;
        }


        public TriangleMesh LoadTriangleMesh(MeshCollisionData.TriangleMeshData model, StillDesign.PhysX.Scene scene, Matrix transform)
        {
            MemoryStream stream = cookTriangleMeshStream(model, transform);

            return scene.Core.CreateTriangleMesh(stream);
        }

        private MemoryStream cookTriangleMeshStream(MeshCollisionData.TriangleMeshData model, Matrix transform)
        {
            TriangleMeshDescription triangleMeshDesc = new TriangleMeshDescription();

            triangleMeshDesc.AllocateVertices<Vector3>(model.Positions.Length);
            triangleMeshDesc.AllocateTriangles<int>(model.Indices.Length); // int indices, should be short but whatever

            Vector3[] transformedPositions = new Vector3[model.Positions.Length];
            Vector3.Transform(model.Positions.ToArray(), ref transform, transformedPositions);

            triangleMeshDesc.VerticesStream.SetData(model.Positions.ToArray());

            triangleMeshDesc.TriangleStream.SetData(model.Indices.ToArray());

            triangleMeshDesc.VertexCount = model.Positions.Length;
            triangleMeshDesc.TriangleCount = model.Positions.Length / 3;

            System.IO.MemoryStream stream = new System.IO.MemoryStream();

            Cooking.InitializeCooking();
            Cooking.CookTriangleMesh(triangleMeshDesc, stream);
            Cooking.CloseCooking();

            stream.Position = 0;
            return stream;
        }

        public void PreloadTriangleMesh(StillDesign.PhysX.Scene scene, MeshCollisionData.TriangleMeshData model)
        {
            usingScene = scene; // This is cheat
            lock (preloadQueue)
            {
                preloadQueue.Enqueue(model);
                Monitor.PulseAll(preloadQueue);
            }
        }

        private void preloadJob()
        {
            for (; ; )
            {
                MeshCollisionData.TriangleMeshData tMesh;
                lock (preloadQueue)
                {
                    while (preloadQueue.Count == 0) Monitor.Wait(preloadQueue);
                    tMesh = preloadQueue.Dequeue();
                }

                lock (this)
                {
                    if (triangleMeshEntries.ContainsKey(tMesh)) continue;
                    currentPreloadingMesh = tMesh;

                }
                var strm = cookTriangleMeshStream(tMesh, Matrix.Identity);
                lock (this)
                {
                    cookedStream = strm;
                    Monitor.PulseAll(this);
                    //Wait for processing in main thread
                    while (currentPreloadingMesh != null) Monitor.Wait(this);
                }

            }
        }

        public ConvexMesh CreateConvexMesh(StillDesign.PhysX.Scene scene, MeshCollisionData.Convex convex)
        {
            ConvexMesh ret;
            if (convexMeshEntries.TryGetValue(convex, out ret)) return ret;

            ret = loadConvexMesh(convex, scene);
            convexMeshEntries[convex] = ret;

            return ret;
        }

        private ConvexMesh loadConvexMesh(MeshCollisionData.Convex convexData, StillDesign.PhysX.Scene scene)
        {

            // Allocate memory for the points and triangles
            var convexMeshDesc = new ConvexMeshDescription()
                {
                    PointCount = convexData.Positions.Count
                };

            convexMeshDesc.Flags |= ConvexFlag.ComputeConvex;
            convexMeshDesc.AllocatePoints<Vector3>(convexData.Positions.Count);

            // Write in the points and triangles
            // We only want the Position component of the vertex. Also scale down the mesh
            for (int i = 0; i < convexData.Positions.Count; i++)
            {
                convexMeshDesc.PointsStream.Write(convexData.Positions[i]);

            }

            // Cook to memory or to a file
            MemoryStream stream = new MemoryStream();
            //FileStream stream = new FileStream( @"Convex Mesh.cooked", FileMode.CreateNew );

            Cooking.InitializeCooking(new ConsoleOutputStream());
            Cooking.CookConvexMesh(convexMeshDesc, stream);
            Cooking.CloseCooking();

            stream.Position = 0;

            return scene.Core.CreateConvexMesh(stream);

        }

        public void Update(StillDesign.PhysX.Scene scene)
        {
            lock (this)
            {
                if (currentPreloadingMesh == null) return;
                if (cookedStream == null) return;

                var mesh = scene.Core.CreateTriangleMesh(cookedStream);
                triangleMeshEntries[currentPreloadingMesh] = mesh;

                Console.WriteLine("Mesh succesfully preloaded!");

                currentPreloadingMesh = null;
                cookedStream = null;

                Monitor.PulseAll(this);

            }
        }
    }
}
