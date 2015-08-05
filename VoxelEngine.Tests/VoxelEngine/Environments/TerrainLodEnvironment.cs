using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using DirectX11;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX.DirectInput;
using System.Linq;

namespace MHGameWork.TheWizards.VoxelEngine.Environments
{
    /// <summary>
    /// Shows a terrain with lod based on clipmaps but with an octree like procworld (so no stitching chunks or continuous scrolling)
    /// </summary>
    public class TerrainLodEnvironment
    {
        public LodOctreeMeshBuilder meshBuilder = new LodOctreeMeshBuilder();
        private LodOctree<LodOctreeNode> tree;
        private LodOctreeNode rootNode;
        private Func<Vector3, float> density;
        private float angle = 0;
        private DensityFunctionHermiteGrid densityGrid;
        private float globalScaling = .1f;
        private readonly int minNodeSize;
        private Vector3 octreeOffset;

        private bool cameraControlled = false;
        private bool automaticMovement = false;
        private Vector3 eyePosition = new Vector3(0, 0, 0);
        private Textarea TextOutput;

        public TerrainLodEnvironment()
        {

            tree = new LodOctree<LodOctreeNode>();

            var size = 32 * (1 << 10);
            rootNode = tree.Create(size, size);

            //octreeOffset = new Vector3(0, size, 0);
            octreeOffset = new Vector3(0, 0, 0);


            density = VoxelTerrainGenerationTest.createDensityFunction5Perlin(17, size / 2);
            //density = v => DensityHermiteGridTest.SineXzDensityFunction(v, 1/5f, size/2, 3);
            density = densityFunction;
            densityGrid = new DensityFunctionHermiteGrid(density, new Point3(size, size, size));
            minNodeSize = 32;




        }
        Array3D<float> noise = VoxelTerrainGenerationTest.generateNoise(15);

        public float densityFunction(Vector3 v)
        {
            v *= globalScaling;
            /*noise = new Array3D<float>(new Point3(2, 2, 2));
            noise[new Point3(1, 1, 1)] = 3;
            noise[new Point3(0, 0, 0)] = -3;*/
            var sampler = new Array3DSampler<float>();
            var density = (float)(rootNode.size / 2 * globalScaling) - v.Y;
            v *= 1 / 8f;
            //v *= (1/8f);
            //density += sampler.sampleTrilinear(noise, v * 4.03f) * 0.25f;
            density += sampler.sampleTrilinear(noise, v * 1.96f) * 0.5f;
            //density += sampler.sampleTrilinear(noise, v * 1.01f) * 1;
            density += sampler.sampleTrilinear(noise, v * 0.55f) * 10;
            density += sampler.sampleTrilinear(noise, v * 0.21f) * 30;
            density += sampler.sampleTrilinear(noise, v * 0.05f) * 100;
            density += sampler.sampleTrilinear(noise, v * 0.01f) * 400;
            //density += noise.GetTiled(v.ToFloored());
            return density;
        }

        public void LoadIntoEngine(TWEngine engine)
        {
            TextOutput = new Textarea() { Position = new Vector2(10, 10), Size = new Vector2(300, 200) };
            // Autostops when engine hotloads
            new Thread(generateMeshesJob).Alter(f => f.Name = "GenerateLodMeshes").Alter(f => f.IsBackground = true).Start();
            new Thread(meshBuilderJob).Alter(f => f.Name = "MeshBuildJob1").Alter(f => f.IsBackground = true).Start();
            new Thread(meshBuilderJob).Alter(f => f.Name = "MeshBuildJob2").Alter(f => f.IsBackground = true).Start();
            new Thread(meshBuilderJob).Alter(f => f.Name = "MeshBuildJob3").Alter(f => f.IsBackground = true).Start();

            //TW.Graphics.FixedTimeStepEnabled = true;
            //TW.Graphics.FixedTimeStep = 1 / 10f;
            TW.Graphics.SpectaterCamera.FarClip = 5000;
            engine.AddSimulator(ProcessUserInput, "ProcessUserInput");
            engine.AddSimulator(UpdateQuadtreeClipmaps, "UpdateClipmaps");
            engine.AddSimulator(DrawDebugOutput, "UpdateLines");
            engine.AddSimulator(CreateUpdatedMeshElements, "CreateMeshElements");
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new RecordingSimulator());

        }

        public void ProcessUserInput()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.F))
                automaticMovement = !automaticMovement;
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.C))
                cameraControlled = !cameraControlled;
            if (automaticMovement)
            {
                var radius = rootNode.size * 0.4f;
                angle += TW.Graphics.Elapsed * 1f / radius;
                var pos = new Vector3(rootNode.size / 2f);
                pos += new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle)) * radius;
                eyePosition = pos;
            }
            /*else
            {
                eyePosition = new Vector3(rootNode.size / 2f);

            }*/
            if (cameraControlled)
                eyePosition = TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.GetTranslation();

            //UpdateQuadtreeClipmaps(rootNode, TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.xna().Translation.dx());


            //pos += new Vector3((float)Math.Sin(angle), (float)Math.Sin(angle), (float)Math.Cos(angle)) * rootNode.size * 0.4f;


            TW.Graphics.LineManager3D.DrawGroundShadows = true;

            TW.Graphics.LineManager3D.AddCenteredBox(eyePosition.ChangeY(0), 4, Color.Red);
            TW.Graphics.LineManager3D.WorldMatrix = Matrix.Translation(octreeOffset) * Matrix.Scaling(new Vector3(globalScaling));
            TW.Graphics.LineManager3D.AddCenteredBox(eyePosition, 16, Color.Red);

        }

        public void DrawDebugOutput()
        {
            TW.Graphics.LineManager3D.WorldMatrix = Matrix.Translation(octreeOffset) * Matrix.Scaling(new Vector3(globalScaling));
            TW.Graphics.LineManager3D.DrawGroundShadows = true;
            //tree.DrawLines(rootNode, TW.Graphics.LineManager3D);
            TW.Graphics.LineManager3D.DrawGroundShadows = false;

            TW.Graphics.LineManager3D.WorldMatrix = Matrix.Identity;
            //tree.DrawLines(rootNode, TW.Graphics.LineManager3D, n => n.Mesh == null, n => Color.Red);
            var leafNodeCount = 0;
            tree.VisitDepthFirst(rootNode, n => { if (n.Children == null) leafNodeCount++; });
            TextOutput.Text = "Dirty nodes: " + dirtyNodesCount + "\n"
                + "MeshElements: " + visibleMeshes + "\n"
                + "LeafNodes: " + leafNodeCount;
        }

        public void UpdateQuadtreeClipmaps()
        {


            lock (rootNode)
            {
                tree.UpdateQuadtreeClipmaps(rootNode, eyePosition / globalScaling, minNodeSize);
            }
        }

        public void CreateUpdatedMeshElements()
        {
            /* LodOctreeNode node;
             while (nodesWithUpdatedMesh.TryDequeue(out node))
             {
                 if (node.IsDestroyed) return;
                 if (node.RenderElement != null) node.RenderElement.Delete();
                 node.RenderElement = meshBuilder.CreateRenderElementForNode(node, minNodeSize, node.Mesh);

             }*/
            visibleMeshes = 0;
            UpdateCanRenderWithoutHoles(rootNode);
            UpdateMeshElements(rootNode);

        }
        public void UpdateCanRenderWithoutHoles(LodOctreeNode node)
        {
            if (node.Children == null)
            {
                node.CanRenderWithoutHoles = node.Mesh != null;
                /*if (!node.CanRenderWithoutHoles)
                {
                    TW.Graphics.LineManager3D.WorldMatrix = Matrix.Identity;
                    tree.DrawSingleNode(node, TW.Graphics.LineManager3D, Color.Orange);

                }*/
                return;
            }

            var canRenderChildrenWithoutHoles = true;
            for (int i = 0; i < 8; i++)
            {
                var child = node.Children[i];
                UpdateCanRenderWithoutHoles(child);
                if (!child.CanRenderWithoutHoles)
                    canRenderChildrenWithoutHoles = false;
            }
            // only render when children cant and i have mesh
            node.CanRenderWithoutHoles = canRenderChildrenWithoutHoles || node.Mesh != null;

        }
        public void UpdateMeshElements(LodOctreeNode node)
        {
            if (node.Children == null)
            {
                if (node.Mesh != null)
                    ensureMeshElementCreated(node);
                return;
            }
            if (node.CanRenderWithoutHoles)
            {
                var canRenderChildrenWithoutHoles = true;
                for (int i = 0; i < 8; i++)
                {
                    var child = node.Children[i];
                    UpdateCanRenderWithoutHoles(child);
                    if (!child.CanRenderWithoutHoles)
                        canRenderChildrenWithoutHoles = false;
                }
                if (canRenderChildrenWithoutHoles)
                {
                    //Render children
                    DestroyMeshElement(node);
                    for (int i = 0; i < 8; i++)
                    {
                        UpdateMeshElements(node.Children[i]);
                    }
                }
                else
                {
                    ensureMeshElementCreated(node);
                    if (node.Children != null)
                        for (int i = 0; i < 8; i++) DestroyMeshElementRecursive(node.Children[i]);
                }
            }
            else
            {
                DestroyMeshElementRecursive(node);
                /*DestroyMeshElement(node);
                if (node.Children != null)
                    for (int i = 0; i < 8; i++) UpdateMeshElements(node.Children[i]);*/
            }


        }

        private void DestroyMeshElementRecursive(LodOctreeNode node)
        {
            DestroyMeshElement(node);

            if (node.Children == null) return;
            for (int i = 0; i < 8; i++)
            {
                DestroyMeshElementRecursive(node.Children[i]);
            }
        }

        private static void DestroyMeshElement(LodOctreeNode node)
        {
            if (node.RenderElement != null) node.RenderElement.Delete();
            node.RenderElement = null;
        }

        private int visibleMeshes = 0;
        private void ensureMeshElementCreated(LodOctreeNode node)
        {
            visibleMeshes++;
            /*TW.Graphics.LineManager3D.WorldMatrix = Matrix.Identity;
            tree.DrawSingleNode(node, TW.Graphics.LineManager3D, Color.Red);*/
            var mesh = node.Mesh;
            if (mesh == null) throw new InvalidOperationException("Cannot ensure!");
            if (node.RenderElement == null)
            {
                node.RenderElement = meshBuilder.CreateRenderElementForNode(node, minNodeSize, mesh);
                node.RenderElement.WorldMatrix = (Matrix)node.RenderElement.WorldMatrix * Matrix.Scaling(new Vector3(globalScaling));
            }
        }


        //private ConcurrentQueue<LodOctreeNode> nodesWithUpdatedMesh = new ConcurrentQueue<LodOctreeNode>();
        private ConcurrentQueue<LodOctreeNode> nodesToProcess = new ConcurrentQueue<LodOctreeNode>();
        private volatile int dirtyNodesCount;
        public void generateMeshesJob()
        {
            try
            {


                bool stop = false;

                TW.Engine.RegisterOnClearEngineState(() => stop = true);


                var meshLessNodes = new List<LodOctreeNode>();

                while (!stop)
                {
                    meshLessNodes.Clear();
                    lock (rootNode)
                    {
                        meshBuilder.ListMeshLessNodes(rootNode, meshLessNodes);
                    }
                    dirtyNodesCount = meshLessNodes.Count;
                    var nodes = meshLessNodes.Where(m => m.BuildingMesh == false).ToArray();
                    if (nodes.Length == 0)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    nodes = nodes.OrderBy(n => n.depth).ToArray();
                    //var eye = eyePosition;


                    //nodes = nodes.OrderBy(n => Vector3.DistanceSquared(eye, n.LowerLeft + new Point3(1, 1, 1) * n.size)).ToArray();

                    for (int i = 0; i < Math.Min(nodes.Length, 30); i++)
                    {
                        nodes[i].BuildingMesh = true;
                        nodesToProcess.Enqueue(nodes[i]);
                    }
                    //nodesWithUpdatedMesh.Enqueue(node);
                }

            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "GeneratedMeshesJob");
            }
        }

        public void meshBuilderJob()
        {
            try
            {


                bool stop = false;

                TW.Engine.RegisterOnClearEngineState(() => stop = true);

                while (!stop)
                {
                    LodOctreeNode node;
                    if (nodesToProcess.TryDequeue(out node))
                    {
                        if (node.IsDestroyed) continue;
                        node.Mesh = meshBuilder.CalculateNodeMesh(node, minNodeSize, density);


                    }
                }
            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "GeneratedMeshesJob");
            }
        }

    }
}