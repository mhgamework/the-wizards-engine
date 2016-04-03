using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.DualContouring.Rendering;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.VoxelEngine.DualContouring.Generation;
using MHGameWork.TheWizards.VoxelEngine.DynamicWorld;
using MHGameWork.TheWizards.VoxelEngine.DynamicWorld.OctreeDC;
using MHGameWork.TheWizards.VoxelEngine.EngineServices;
using NUnit.Framework;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.VoxelEngine.SkyWorld
{
    /// <summary>
    /// Provides a procedural world visualisation + generation framework, with user input possibilities
    /// </summary>
    public class ProceduralWorldEnvironment : EngineTestFixture
    {

        private DCVoxelMaterial mat1;
        private DCVoxelMaterial mat2;

        public ProceduralWorldEnvironment()
        {

        }

        private DCVoxelMaterial materialFunction(Vector3 vector3, float sampleInterval)
        {
            var y = vector3.Y + noiseGenerator.CalculatePerlinNoise1D(vector3.Y * (1 / 10f));
            if (y > 300) return mat2;
            return mat1;
        }

        private float densityFunction(Vector3 p, float sampleInterval)
        {
            var center = new Vector3(400, 200, 400);
            var radius = 100f;

            if (p.Y > center.Y)
            {
                var height = (p.Y - center.Y);
                var radialDist = (center - new Vector3(p.X, center.Y, p.Z)).Length() / radius;

                radialDist = MathHelper.Clamp(1 - radialDist, 0, 1);
                if (radialDist <= 0) return (p.Y - center.Y);

                //p.Y  -= radialDist * 25f;
                //p.Y = (p.Y - center.Y) * 5f + center.Y;
                return (p.Y - center.Y) - fBm((p * 1 / 50f), 3, 2f, 0.5f, 1) * 60 * radialDist;// p.Y- center.Y;
            }

            return (p - center).Length() - radius + fBm((p * 1 / 50f), 3, 2f, 0.5f, 1) * 20f;


            return densityFbmTerrain(p);

            return densitySinWithSphere(p, sampleInterval);
        }

        private float densityFbmTerrain(Vector3 p)
        {
            var gain = 0.5f;
            var lacunarity = 2f;
            var filterWidth = 1;


            var ret = p.Y - 200f;


            //ret += fBm(p * (1 / 256f), 10, 2, 0.5f, 1) * 200f;
            ret += fBm(p * (1 / 128f), 10, 2, 0.4f, 1) * 64f;

            //var numOctaves = 10;
            //var startGain = 500;

            //var freq = 1 / 1000f;
            //var ampl = 120f;

            //for (int i = 0; i < 10; i++)
            //{
            //    if (freq > 1 / sampleInterval) break;
            //    //if (i != 2) continue;
            //    ret += noiseGenerator.CalculatePerlinNoise(p * freq) * ampl;

            //    freq /= lacunarity; 2f;
            //    ampl *= 0.5f;
            //}


            return ret;
        }

        /// <summary>
        /// Normalized fbm noise, ampl -1 to 1, one perlin gridpoint every 1 unit
        /// </summary>
        /// <param name="vInputCoords"></param>
        /// <param name="nNumOctaves"></param>
        /// <param name="fLacunarity"></param>
        /// <param name="fInGain"></param>
        /// <param name="fFilterWidth"></param>
        /// <returns></returns>

        float fBm(Vector3 vInputCoords, float nNumOctaves, float fLacunarity, float fInGain, float fFilterWidth)
        {
            float fNoiseSum = 0;
            float fAmplitude = 1;
            float fAmplitudeSum = 0;
            float fFilterWidthPerBand = fFilterWidth;
            Vector3 vSampleCoords = vInputCoords;
            for (int i = 0; i < nNumOctaves; i += 1)
            {
                fNoiseSum += fAmplitude * noiseGenerator.CalculatePerlinNoise(vSampleCoords);    // Without removal of high-frequencies
                //fNoiseSum += fAmplitude * filterednoise(vSampleCoords, fFilterWidthPerBand);    // With removal of high-frequencies

                fAmplitudeSum += fAmplitude;
                fFilterWidthPerBand *= fLacunarity;
                fAmplitude *= fInGain;
                vSampleCoords *= fLacunarity;
            }
            fNoiseSum /= fAmplitudeSum;
            return fNoiseSum;
        }

        private float filterednoise(Vector3 x, float w)
        {
            return fadeout(noiseGenerator.CalculatePerlinNoise(x), 0.5f, 1, w);
        }

        float fadeout(float f, float fAverage, float fFeatureSize, float fWidth)
        {
            // Copied from Tarachunk-Noise GDC07, dont understand the 0.2 and 0.6, maybe its related to how the smoothstep works for factor < 0 and > 1
            return MathHelper.Lerp(f, fAverage, MathHelper.SmoothStep(0.2f, 0.6f, fWidth / fFeatureSize));
        }

        private static float densitySinWithSphere(Vector3 p, float sampleInterval)
        {
            var ret = p.Y - 200 + (float)Math.Sin(p.X * 0.05) * 10 + (float)Math.Cos(p.Z * .05) * 8;

            if (sampleInterval < 6)
                ret += (float)Math.Sin(p.X * 0.5) * 1; // alternates every 2* 3.14 x


            return (float)Math.Min(ret, (p - new Vector3(512, 200, 512)).Length() - 200);
        }


        [Test]
        public void Run()
        {
            //dcAlgo = new OctreeDCAlgorithm();
            vRenderer = new VoxelRenderingService(TW.Graphics).VoxelRenderer;

            mat1 = new DCVoxelMaterial()
            {
                Texture = DCFiles.UVCheckerMap10_512
            };
            mat2 = new DCVoxelMaterial()
            {
                Texture = DCFiles.UVCheckerMap11_512
            };
            //var numChunks = new Point3(9, 9, 9);
            //var chunkSize = 128;
            //var holder = new WorldHolder(numChunks);


            //var gen = new FlatWorldGenerator(1, chunkSize);
            //Point3.ForEach(numChunks, p =>
            //{
            //    var chunk = holder.GetChunk(p);
            //    gen.GenerateChunk(chunk);
            //    var el = generateMeshElement(chunk.SignedOctree);
            //    if (el == null) return;
            //    el.WorldMatrix = Matrix.Translation(chunk.Coord.ToVector3() * (chunkSize * 1.01f));

            //});


            //engine.AddSimulator(new WorldRenderingSimulator());

            //worldTree = CreateFlatWorldOctree(worldSize);
            TW.Graphics.SpectaterCamera.FarClip = 2048 * 2;
            treeHelper = new ClipMapsOctree<ProceduralOctreeNode>();

            //worldTree = treeHelper.Create(1024, 4);
            worldTree = treeHelper.Create(512, 16);

            engine.AddSimulator(new WorldRenderingSimulator());


            engine.AddSimulator(createWalker(), "Walker");

            engine.AddSimulator(createShowOctreeAction(), "OctreeLines");
            engine.AddSimulator(createClipmapsRenderer(), "ClipMapsUpdater");
            //engine.AddSimulator(createProgressiveRenderer(), "BasicProgressiveRenderer");
            engine.AddSimulator(createLeafRenderer(1), "LeafRenderer");

            //engine.AddSimulator(createHermiteDataVisualizer(), "HermiteDataVisualizer");
        }

        private Action createWalker()
        {
            float walkHeight = 1.7f;

            Resolve<TestInfoUserinterface>().Text += "\nPress Y to go to walk mode";

            bool walking = false;

            return () =>
            {
                if (TW.Graphics.Keyboard.IsKeyPressed(Key.Y))
                    walking = !walking;

                if (!walking) return;

                var feet = (Vector3)TW.Graphics.SpectaterCamera.CameraPosition - Vector3.UnitY * walkHeight;

                var move = new Vector3();

                var sdf = densityFunction(feet, 0.001f);

                move = -Vector3.UnitY * (MathHelper.Min(Math.Abs(sdf), TW.Graphics.Elapsed * 9.81f)) * (float)Math.Sign(sdf);
                if (Math.Abs(sdf) < 0.5f) return;

                TW.Graphics.SpectaterCamera.CameraPosition += move.dx();

            };
        }

        private NoiseGenerator noiseGenerator = new NoiseGenerator();
        private ConcurrentQueue<VoxelSurface> surfacesToAdd = new ConcurrentQueue<VoxelSurface>();

        private Action createLeafRenderer(int numToProcessPerFrame)
        {
            // Basic algorithm: Always update visible leafs (does not prevent holes)

            var nodeGridSize = 16;


            var list = new Queue<ProceduralOctreeNode>();


            return () =>
            {
                var count = 0;

                while (!surfacesToAdd.IsEmpty)
                {
                    VoxelSurface vRes;
                    if (surfacesToAdd.TryDequeue(out vRes))
                    {
                        vRenderer.AddSurface(vRes);
                        count++;
                        return;
                    }
                }
                //if (count > 0)
                //    Console.WriteLine("Added to renderer: " + count);



                foreach (var n in visibilityChangedNodes) list.Enqueue(n);

                var numLeft = numToProcessPerFrame;

                while (list.Count > 0)
                {
                    var n = list.Dequeue();
                    //Console.WriteLine("Nodes in list to process: " + list.Count);

                    if (!n.IsVisibilityLeaf)
                    {
                        if (n.RendererSurface != null) // This might have some nasty multithreading issues
                            n.RendererSurface.Delete();
                        n.RendererSurface = null;
                    }
                    else
                    {
                        if (n.RendererSurface != null) return;
                        ThreadPool.QueueUserWorkItem(
                            o =>
                            {
                                //Console.WriteLine("Begin Task");
                                var surf = createRendererSurface(n, nodeGridSize);
                                n.RendererSurface = surf;
                                surfacesToAdd.Enqueue(surf);
                                //Console.WriteLine("End Task");
                            });
                    }

                    numLeft--;
                    if (numLeft <= 0) break;
                }

            };

        }

        private VoxelSurface createRendererSurface(ProceduralOctreeNode n, int nodeGridSize)
        {
            var cellSize = n.Size / (float)nodeGridSize;


            return vRenderer.CreateVoxelSurfaceAsync(sampleGrid((Vector3)n.LowerLeft.ToVector3() - new Vector3(cellSize * 0.5f), n.Size / nodeGridSize, nodeGridSize + 1),
                Matrix.Scaling(new Vector3(n.Size / nodeGridSize)) * Matrix.Translation(n.LowerLeft - new Vector3(cellSize * 0.5f)));
        }

        private Action createProgressiveRenderer()
        {
            // Basic algorithm: creates content bread first and only sets visible when full lvl is complete

            var list = new Queue<ProceduralOctreeNode>();

            return () =>
            {
                //visibilityChangedNodes.Sort((a, b) => a.Depth - b.Depth);
                //{*/
                //var c = visibilityChangedNodes.FirstOrDefault();
                //if (c == null) return;
                //if (c.IsVisibilityLeaf)

                // Walk breadfirst until reaching a visiblity leaf

                list.Enqueue(worldTree);

                while (list.Any())
                {
                    var c = list.Dequeue();
                    if (c.RendererSurface == null)
                    {
                        c.RendererSurface = createRendererSurface(c, nodeGridSize);
                        return;
                    }

                    if (c.IsVisibilityLeaf) continue;
                    if (c.Children == null) continue;
                    foreach (var el in c.Children)
                        list.Enqueue(el);


                }

            };

        }

        private AbstractHermiteGrid sampleGrid(Vector3 lowerLeft, float sampleInterval, int gridSize)
        {
            gridSize += 1;
            return new DensityFunctionHermiteGrid(p => densityFunction(p * sampleInterval + lowerLeft, sampleInterval), new Point3(gridSize, gridSize, gridSize), p => materialFunction(p * sampleInterval + lowerLeft, sampleInterval));
        }



        private ProceduralOctreeNode worldTree;
        private ClipMapsOctree<ProceduralOctreeNode> treeHelper;


        private List<ProceduralOctreeNode> visibilityChangedNodes = new List<ProceduralOctreeNode>();
        private DCOctreeAlgorithm dcAlgo;
        private VoxelCustomRenderer vRenderer;
        private int surfaceDepth = 4;//3;
        private int minNodeSize = 16;//8;
        private int worldSize = 16;
        private int nodeGridSize = 16;


        //private VoxelSurface generateMeshElement(ProceduralOctreeNode node, int depth = int.MaxValue)
        //{
        //    var tris = dcAlgo.GenerateTrianglesForOctree(node, depth);
        //    if (tris.Length == 0) return null;

        //    var rawmesh = new RawMeshData(
        //        tris.Select(v => v.dx()).ToArray(),
        //        tris.Select(v => Vector3.UnitY.dx()).ToArray(),
        //        tris.Select(v => new Vector2().dx()).ToArray(),
        //        tris.Select(v => new Vector3().dx()).ToArray());

        //    var s = new VoxelSurface(vRenderer);
        //    s.MeshesWithMaterial.Add(new MeshWithMaterial(rawmesh, null));

        //    vRenderer.AddSurface(s);
        //    return s;
        //}

        /* private Action createHermiteDataVisualizer()
         {
             return () =>
             {
                 treeHelper.VisitDepthFirst(worldTree, n =>
                 {
                     if (n.Children == null && !(n.Signs.All(b => b) || n.Signs.All(b => !b)))
                     {
                         TW.Graphics.LineManager3D.AddCenteredBox(n.QEF * n.Size + n.LowerLeft, n.Size * 0.25f, Color.Red.dx());

                     }

                 });
             };
         }*/

        private Action createShowOctreeAction()
        {
            var visible = false;
            Resolve<TestInfoUserinterface>().Text += "\nPress N to show octree";

            treeHelper = new ClipMapsOctree<ProceduralOctreeNode>();

            return () =>
            {
                if (TW.Graphics.Keyboard.IsKeyPressed(Key.N)) visible = !visible;

                if (!visible) return;
                //treeHelper.DrawLines(worldTree, TW.Graphics.LineManager3D, 3);

                treeHelper.DrawLines(worldTree, TW.Graphics.LineManager3D,
                    delegate (ProceduralOctreeNode n, out bool visitChildren, out Color c)
                    {
                        visitChildren = !n.IsVisibilityLeaf;
                        c = Color.Orange;
                        return true;
                    });
            };
        }

        private Action createClipmapsRenderer()
        {
            var renderTree = false;
            var followCam = true;


            var pos = TW.Graphics.SpectaterCamera.CameraPosition;

            Resolve<TestInfoUserinterface>().Text += "\nPress C to switch to debug camera";



            var currentTargetResolution = 100f;



            return () =>
            {
                if (TW.Graphics.Keyboard.IsKeyDown(Key.NumberPadPlus))
                    currentTargetResolution += TW.Graphics.Elapsed * 10;
                if (TW.Graphics.Keyboard.IsKeyDown(Key.NumberPadMinus))
                {
                    currentTargetResolution = TW.Graphics.Elapsed * 10;

                }

                if (TW.Graphics.Keyboard.IsKeyPressed(Key.C)) followCam = !followCam;

                visibilityChangedNodes.Clear();


                if (!followCam)
                {
                    // Draw cam
                    TW.Graphics.LineManager3D.AddCenteredBox(pos, 2, Color.Red.dx());
                }
                pos = TW.Graphics.SpectaterCamera.CameraPosition;

                //UpdateQuadtreeClipmaps(worldTree, pos, minNodeSize);
                UpdateQuadtreeClipmapsProgressive(worldTree, pos, minNodeSize, currentTargetResolution);


                foreach (var n in visibilityChangedNodes)
                {
                    if (n.IsVisibilityLeaf)
                        ensureMesh(n);
                    else
                        removeMesh(n);
                }
            };
        }

        private void removeMesh(ProceduralOctreeNode n)
        {
            /*if (n.Mesh == null) return;
            n.Mesh.Delete();
            n.Mesh = null;*/
        }

        private void ensureMesh(ProceduralOctreeNode n)
        {
            /*if (n.Mesh != null) return;
            n.Mesh = generateMeshElement(n, n.Depth + surfaceDepth);*/
            //if (n.Mesh != null)
            //    n.Mesh.WorldMatrix = Matrix.Translation(new Vector3(0, n.Depth * 0.1f, 0));
        }

        private void UpdateQuadtreeClipmapsProgressive(ProceduralOctreeNode node, Vector3 cameraPosition, int minNodeSize, float targetResolution)
        {


            var center = (Vector3)node.LowerLeft.ToVector3() + new Vector3(1) * node.Size * 0.5f;
            var dist = Vector3.Distance(cameraPosition, center);

            var pointsPerUnitLength = nodeGridSize / node.Size;
            var sizeOfUnit = node.Size / nodeGridSize;
            // If dist = 1 then sizeOfUnit is 1 on screen. => 

            // Should take into account the fact that if minNodeSize changes, the quality of far away nodes changes so the threshold maybe should change too
            if (dist > node.Size * 1.2f)
            {
                // This is a valid node size at this distance, so remove all children
                //Merge(node);
                if (node.IsVisibilityLeaf) return; // already visible
                visibilityChangedNodes.Add(node);
                setAsVisiblityLeaf(node);
            }
            else
            {
                //if (node.Children == null)
                //    Split(node, false, minNodeSize);

                //if (node.Children == null) return; // Minlevel
                if (node.Size <= minNodeSize)
                {
                    if (node.IsVisibilityLeaf) return; // already visible
                    visibilityChangedNodes.Add(node);
                    setAsVisiblityLeaf(node);
                    return;  // Min level
                }
                if (node.Children == null)
                {
                    if (node.IsVisibilityLeaf) return; // already visible
                    visibilityChangedNodes.Add(node);
                    node.IsVisibilityLeaf = true;
                    return;
                }
                for (int i = 0; i < 8; i++)
                {
                    //Invar: this node must be an visibility leaf, or the leafs must be in the children, so make the children leaf
                    if (node.IsVisibilityLeaf)
                    {
                        visibilityChangedNodes.Add(node.Children[i]); // This overestimates the list but thats fine
                        node.Children[i].IsVisibilityLeaf = true; // If this was visibile then make child visible
                    }

                    UpdateQuadtreeClipmaps(node.Children[i], cameraPosition, minNodeSize);
                }
                if (node.IsVisibilityLeaf)
                    visibilityChangedNodes.Add(node);
                node.IsVisibilityLeaf = false;


            }
        }

        private void UpdateQuadtreeClipmaps(ProceduralOctreeNode node, Vector3 cameraPosition, int minNodeSize)
        {
            var center = (Vector3)node.LowerLeft.ToVector3() + new Vector3(1) * node.Size * 0.5f;
            var dist = Vector3.Distance(cameraPosition, center);

            // Should take into account the fact that if minNodeSize changes, the quality of far away nodes changes so the threshold maybe should change too
            if (dist > node.Size * 1.2f)
            {
                // This is a valid node size at this distance, so remove all children
                //Merge(node);
                if (node.IsVisibilityLeaf) return; // already visible
                visibilityChangedNodes.Add(node);
                setAsVisiblityLeaf(node);
            }
            else
            {
                //if (node.Children == null)
                //    Split(node, false, minNodeSize);

                //if (node.Children == null) return; // Minlevel
                if (node.Size <= minNodeSize)
                {
                    if (node.IsVisibilityLeaf) return; // already visible
                    visibilityChangedNodes.Add(node);
                    setAsVisiblityLeaf(node);
                    return;  // Min level
                }
                if (node.Children == null)
                {
                    if (node.IsVisibilityLeaf) return; // already visible
                    visibilityChangedNodes.Add(node);
                    node.IsVisibilityLeaf = true;
                    return;
                }
                for (int i = 0; i < 8; i++)
                {
                    //Invar: this node must be an visibility leaf, or the leafs must be in the children, so make the children leaf
                    if (node.IsVisibilityLeaf)
                    {
                        visibilityChangedNodes.Add(node.Children[i]); // This overestimates the list but thats fine
                        node.Children[i].IsVisibilityLeaf = true; // If this was visibile then make child visible
                    }

                    UpdateQuadtreeClipmaps(node.Children[i], cameraPosition, minNodeSize);
                }
                if (node.IsVisibilityLeaf)
                    visibilityChangedNodes.Add(node);
                node.IsVisibilityLeaf = false;


            }
        }

        private void setAsVisiblityLeaf(ProceduralOctreeNode node)
        {
            // Already correctly set

            if (node.IsVisibilityLeaf) return;
            node.IsVisibilityLeaf = true;
            treeHelper.VisitTopDown(node, (n) =>
            {
                if (node == n) return VisitOptions.Continue;
                if (n.IsVisibilityLeaf)
                {
                    // Found a leaf, unset and stop
                    n.IsVisibilityLeaf = false;
                    visibilityChangedNodes.Add(n);
                    return VisitOptions.SkipChildren;
                }
                // keep searching
                return VisitOptions.Continue;
            });
        }


        //public ProceduralOctreeNode CreateFlatWorldOctree(int worldSize)
        //{
        //    var builder = new SignedOctreeBuilder();

        //    var tree = builder.ConvertHermiteGridToOctree(new DensityFunctionHermiteGrid(p =>
        //    {
        //        p -= new Vector3(worldSize * 0.5f);
        //        return p.Length() - worldSize * 0.4f;
        //        /*p.X += worldSize / 2.0f;
        //        p.Y += worldSize / 2.0f;
        //        return p.Y*p.Y + p.X*p.X;*/
        //    }, new Point3(worldSize + 1, worldSize + 1, worldSize + 1)));
        //    return tree;
        //}
    }
}