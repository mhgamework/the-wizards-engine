﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DirectX11;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.DualContouring.Rendering;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.VoxelEngine.Worlding
{
    /// <summary>
    /// Shows a terrain with lod based on clipmaps but with an octree like procworld (so no stitching chunks or continuous scrolling)
    /// </summary>
    public class LargeWorldRenderer
    {
        private readonly VoxelCustomRenderer voxelCustomRenderer;
        public LodOctreeMeshBuilder meshBuilder = new LodOctreeMeshBuilder();
        private LodOctree<WorldNode> tree;
        private WorldNode rootNode;
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

        public LargeWorldRenderer(VoxelCustomRenderer voxelCustomRenderer)
        {
            this.voxelCustomRenderer = voxelCustomRenderer;

            tree = new LodOctree<WorldNode>(new LargeWorldNodeFactory());
            minNodeSize = 8;

            /*var size = 32 * (1 << 10);
            rootNode = tree.Create(size, size);

            //octreeOffset = new Vector3(0, size, 0);
            octreeOffset = new Vector3(0, 0, 0);


            density = VoxelTerrainGenerationTest.createDensityFunction5Perlin(17, size / 2);
            //density = v => DensityHermiteGridTest.SineXzDensityFunction(v, 1/5f, size/2, 3);
            density = densityFunction;
            densityGrid = new DensityFunctionHermiteGrid(density, new Point3(size, size, size));
            minNodeSize = 32;*/


            var t = new Thread(generateMeshesJob);
            t.Name = "GenerateMeshesJob";
            t.IsBackground = false;
            t.Start();



        }

        private Array3D<float> noise;// = VoxelTerrainGenerationTest.generateNoise(15);

        public float densityFunction(Vector3 v)
        {
            v *= globalScaling;
            /*noise = new Array3D<float>(new Point3(2, 2, 2));
            noise[new Point3(1, 1, 1)] = 3;
            noise[new Point3(0, 0, 0)] = -3;*/
            var sampler = new Array3DSampler<float>();
            var density = (float)(rootNode.Size / 2 * globalScaling) - v.Y;
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
                var radius = rootNode.Size * 0.4f;
                angle += TW.Graphics.Elapsed * 1f / radius;
                var pos = new Vector3(rootNode.Size / 2f);
                pos += new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle)) * radius;
                eyePosition = pos;
            }
            if (cameraControlled)
                eyePosition = TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.GetTranslation();


            TW.Graphics.LineManager3D.DrawGroundShadows = true;

            TW.Graphics.LineManager3D.AddCenteredBox(eyePosition.ChangeY(0), 4, Color.Red);
            TW.Graphics.LineManager3D.WorldMatrix = Matrix.Translation(octreeOffset) * Matrix.Scaling(new Vector3(globalScaling));
            TW.Graphics.LineManager3D.AddCenteredBox(eyePosition, 16, Color.Red);

        }

        public void DrawDebugOutput()
        {
            TW.Graphics.LineManager3D.WorldMatrix = Matrix.Translation(octreeOffset) * Matrix.Scaling(new Vector3(globalScaling));
            TW.Graphics.LineManager3D.DrawGroundShadows = true;
            tree.DrawLines(rootNode, TW.Graphics.LineManager3D);
            TW.Graphics.LineManager3D.DrawGroundShadows = false;

            TW.Graphics.LineManager3D.WorldMatrix = Matrix.Identity;
            //tree.DrawLines(rootNode, TW.Graphics.LineManager3D, n => n.Mesh == null, n => Color.Red);
            var leafNodeCount = 0;
            tree.VisitDepthFirst(rootNode, n => { if (n.Children == null) leafNodeCount++; });
            //TextOutput.Text = "Dirty nodes: " + dirtyNodesCount + "\n"
            //    + "MeshElements: " + visibleMeshes + "\n"
            //    + "LeafNodes: " + leafNodeCount;
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
            // First priority, have correct visuals at least where we are standing( like minecraft)
            //   Do this by minimally constructing the meshes for closest octree nodes, some kind of 'minimal' clipmapping
            //   where nodes increase in size as fast as allowed by octree
            // Second priority, achieve error-to pixel ratio, by using better clipmapping division


            if (hasTask)
            {
                tree.DrawSingleNode(taskData.Node, TW.Graphics.LineManager3D, Color.Orange);
                if (!asyncSurfaceCalculationReady()) return;

                var data = getAndClearResult();

                if (!data.Node.Destroyed)
                {
                    voxelCustomRenderer.AddSurface(data.Surface);
                    data.Node.VoxelSurface = data.Surface;
                }

            }
            var cameraNode = getWorldNode(TW.Graphics.SpectaterCamera.CameraPosition);
            if (cameraNode != null)
            {
                TW.Graphics.LineManager3D.AddBox(cameraNode.BoundingBox, Color.Yellow);
                if (TW.Graphics.Keyboard.IsKeyPressed(Key.U))
                {
                    if (cameraNode.VoxelSurface != null) cameraNode.VoxelSurface.Delete();
                    cameraNode.VoxelSurface = null;

                    launchAsyncSurfaceCalculation(cameraNode);

                    return;
                }
            }

            tree.VisitDepthFirst(rootNode, n =>
                {
                    if (n.Children != null)
                    {
                        if (n.VoxelSurface != null)
                        {
                            n.VoxelSurface.Delete();
                            n.VoxelSurface = null;
                        }

                        return VisitOptions.Continue;
                    }
                    if (n.VoxelSurface == null)
                    {
                        launchAsyncSurfaceCalculation(n);
                        return VisitOptions.AbortVisit;
                    }
                    return VisitOptions.Continue;
                });

            /* LodOctreeNode node;
             while (nodesWithUpdatedMesh.TryDequeue(out node))
             {
                 if (node.IsDestroyed) return;
                 if (node.RenderElement != null) node.RenderElement.Delete();
                 node.RenderElement = meshBuilder.CreateRenderElementForNode(node, minNodeSize, node.Mesh);

             }*/
            visibleMeshes = 0;
            //UpdateCanRenderWithoutHoles(rootNode);
            //UpdateMeshElements(rootNode);

        }



        private WorldNode getWorldNode(Vector3 cameraPosition)
        {
            WorldNode ret = null;
            cameraPosition /= globalScaling;

            tree.VisitDepthFirst(rootNode, n =>
                {
                    if (n.BoundingBox.Contains(cameraPosition) == ContainmentType.Disjoint) return VisitOptions.SkipChildren;
                    if (n.Children == null)
                    {
                        ret = n;
                        return VisitOptions.AbortVisit;
                    }
                    return VisitOptions.Continue;
                });
            return ret;

        }

        private VoxelSurface generateVoxelSurface(WorldNode n)
        {
            try
            {
                if (n.VoxelSurface != null) return null;
                //var grid = n.VoxelData ?? n.VoxelDataGenerator;


                var resolution = n.Size / minNodeSize;
                // Hacktime, only works for densitygrids
                var g = (DensityFunctionHermiteGrid)n.VoxelDataGenerator;
                var offset = (Vector3)n.LowerLeft.ToVector3() -
                             new Vector3(0.5f) * resolution;

                var nGrid = new DensityFunctionHermiteGrid(
                    v =>
                    {
                        //Currently assume density in voxel space, which is probably not what we want in long term
                        return
                            g.DensityFunction((v * resolution + offset));
                    },
                    new Point3(minNodeSize + 1, minNodeSize + 1,
                                minNodeSize + 1));


                var world = Matrix.Translation(new Vector3(-0.5f)) *
                            Matrix.Scaling(resolution, resolution,
                                            resolution) *
                            Matrix.Translation(n.LowerLeft) *
                            Matrix.Scaling(new Vector3(globalScaling));

                return voxelCustomRenderer.CreateVoxelSurfaceAsync(
                    nGrid, world);
            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "Generatevoxelsurface");
                throw;
            }
        }


        //public void UpdateCanRenderWithoutHoles(WorldNode node)
        //{
        //    if (node.Children == null)
        //    {
        //        node.CanRenderWithoutHoles = node.Mesh != null;
        //        /*if (!node.CanRenderWithoutHoles)
        //        {
        //            TW.Graphics.LineManager3D.WorldMatrix = Matrix.Identity;
        //            tree.DrawSingleNode(node, TW.Graphics.LineManager3D, Color.Orange);

        //        }*/
        //        return;
        //    }

        //    var canRenderChildrenWithoutHoles = true;
        //    for (int i = 0; i < 8; i++)
        //    {
        //        var child = node.Children[i];
        //        UpdateCanRenderWithoutHoles(child);
        //        if (!child.CanRenderWithoutHoles)
        //            canRenderChildrenWithoutHoles = false;
        //    }
        //    // only render when children cant and i have mesh
        //    node.CanRenderWithoutHoles = canRenderChildrenWithoutHoles || node.Mesh != null;

        //}
        //public void UpdateMeshElements(WorldNode node)
        //{
        //    if (node.Children == null)
        //    {
        //        if (node.Mesh != null)
        //            ensureMeshElementCreated(node);
        //        return;
        //    }
        //    if (node.CanRenderWithoutHoles)
        //    {
        //        var canRenderChildrenWithoutHoles = true;
        //        for (int i = 0; i < 8; i++)
        //        {
        //            var child = node.Children[i];
        //            UpdateCanRenderWithoutHoles(child);
        //            if (!child.CanRenderWithoutHoles)
        //                canRenderChildrenWithoutHoles = false;
        //        }
        //        if (canRenderChildrenWithoutHoles)
        //        {
        //            //Render children
        //            DestroyMeshElement(node);
        //            for (int i = 0; i < 8; i++)
        //            {
        //                UpdateMeshElements(node.Children[i]);
        //            }
        //        }
        //        else
        //        {
        //            ensureMeshElementCreated(node);
        //            if (node.Children != null)
        //                for (int i = 0; i < 8; i++) DestroyMeshElementRecursive(node.Children[i]);
        //        }
        //    }
        //    else
        //    {
        //        DestroyMeshElementRecursive(node);
        //        /*DestroyMeshElement(node);
        //        if (node.Children != null)
        //            for (int i = 0; i < 8; i++) UpdateMeshElements(node.Children[i]);*/
        //    }


        //}

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
        private ConcurrentQueue<WorldNode> nodesToProcess = new ConcurrentQueue<WorldNode>();
        private volatile int dirtyNodesCount;

        private object taskLock = new object();
        private bool hasTask = false;
        private bool hasResult = false;
        private SurfaceTaskData taskData;


        private SurfaceTaskData getAndClearResult()
        {

            lock (taskLock)
            {
                if (!asyncSurfaceCalculationReady()) throw new InvalidOperationException();

                var ret = taskData;
                hasTask = false;
                hasResult = false;
                taskData = new SurfaceTaskData();
                return ret;
            }
        }

        private bool asyncSurfaceCalculationReady()
        {
            lock ( taskLock )
                return hasResult;
        }

        private void launchAsyncSurfaceCalculation(WorldNode n)
        {
            lock ( taskLock )
            {
                if (hasTask) throw new InvalidOperationException();

                taskData = new SurfaceTaskData() { Node = n };
                hasTask = true;
                hasResult = false;
                Monitor.Pulse(taskLock);
            }
          
        }

        private struct SurfaceTaskData
        {
            public WorldNode Node;
            public VoxelSurface Surface;

        }

        public void generateMeshesJob()
        {
            try
            {


                bool stop = false;

                TW.Engine.RegisterOnClearEngineState(() => stop = true);


                //var meshLessNodes = new List<LodOctreeNode>();

                while (!stop)
                {
                    lock (taskLock)
                    {
                        while (!hasTask || hasResult)
                            Monitor.Wait(taskLock);
                    }

                    var ret = generateVoxelSurface(taskData.Node);
                    lock ( taskLock )
                    {
                        if ( !hasResult )
                        {
                            taskData.Surface = ret;
                            hasResult = true;
                        }
                        
                    }

                    /*        meshLessNodes.Clear();
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
                            nodes = nodes.OrderBy(n => n.Depth).ToArray();
                            //var eye = eyePosition;


                            //nodes = nodes.OrderBy(n => Vector3.DistanceSquared(eye, n.LowerLeft + new Point3(1, 1, 1) * n.size)).ToArray();

                            for (int i = 0; i < Math.Min(nodes.Length, 30); i++)
                            {
                                nodes[i].BuildingMesh = true;
                                nodesToProcess.Enqueue(nodes[i]);
                            }*/
                    //nodesWithUpdatedMesh.Enqueue(node);
                }

            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "GenerateMeshesJob");
            }
        }

        public void meshBuilderJob()
        {
            //try
            //{


            //    bool stop = false;

            //    TW.Engine.RegisterOnClearEngineState(() => stop = true);

            //    while (!stop)
            //    {
            //        LodOctreeNode node;
            //        if (nodesToProcess.TryDequeue(out node))
            //        {
            //            if (node.IsDestroyed) continue;
            //            node.Mesh = meshBuilder.CalculateNodeMesh(node, minNodeSize, density);


            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    DI.Get<IErrorLogger>().Log(ex, "GeneratedMeshesJob");
            //}
        }

        public void Update()
        {

            ProcessUserInput();

            UpdateQuadtreeClipmaps();
            CreateUpdatedMeshElements();

            DrawDebugOutput();

        }

        public void LoadWorld(int worldSize, Func<Vector3, float> getDensity)
        {
            //var size = 32 * (1 << 10);
            rootNode = tree.Create(worldSize, worldSize);

            //octreeOffset = new Vector3(0, size, 0);
            octreeOffset = new Vector3(0, 0, 0);


            density = getDensity;
            //density = v => DensityHermiteGridTest.SineXzDensityFunction(v, 1/5f, size/2, 3);
            //density = densityFunction;
            densityGrid = new DensityFunctionHermiteGrid(density, new Point3(worldSize, worldSize, worldSize));

            rootNode.VoxelDataGenerator = densityGrid;
            //minNodeSize = 32;
        }
    }
}