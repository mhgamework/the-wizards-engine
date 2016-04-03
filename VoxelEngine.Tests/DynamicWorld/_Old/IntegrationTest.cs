using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.DualContouring.Rendering;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.Meshes;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.VoxelEngine.DynamicWorld.OctreeDC;
using MHGameWork.TheWizards.VoxelEngine.EngineServices;
using NUnit.Framework;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld.Tests
{

    public class IntegrationTest : EngineTestFixture
    {
        private SignedOctreeNode worldTree;
        private ClipMapsOctree<SignedOctreeNode> treeHelper;


        private List<SignedOctreeNode> visibilityChangedNodes = new List<SignedOctreeNode>();
        private OctreeDCAlgorithm dcAlgo;
        private VoxelCustomRenderer vRenderer;
        private int surfaceDepth = 4;//3;
        private int minNodeSize = 16;//8;
        private int worldSize = 16;


        [SetUp]
        public void Setup()
        {
            dcAlgo = new OctreeDCAlgorithm();
            vRenderer = new VoxelRenderingService(TW.Graphics).VoxelRenderer;
        }

        [Test]
        public void Run()
        {
            var numChunks = new Point3(9, 9, 9);
            var chunkSize = 128;
            var holder = new WorldHolder(numChunks);


            var gen = new FlatWorldGenerator(1, chunkSize);
            Point3.ForEach(numChunks, p =>
            {
                var chunk = holder.GetChunk(p);
                gen.GenerateChunk(chunk);
                var el = generateMeshElement(chunk.SignedOctree);
                if (el == null) return;
                el.WorldMatrix = Matrix.Translation(chunk.Coord.ToVector3() * (chunkSize * 1.01f));

            });


            engine.AddSimulator(new WorldRenderingSimulator());

        }

        private VoxelSurface generateMeshElement(SignedOctreeNode node, int depth = int.MaxValue)
        {
            var tris = dcAlgo.GenerateTrianglesForOctree(node, depth);
            if (tris.Length == 0) return null;

            var rawmesh = new RawMeshData(
                tris.Select(v => v.dx()).ToArray(),
                tris.Select(v => Vector3.UnitY.dx()).ToArray(),
                tris.Select(v => new Vector2().dx()).ToArray(),
                tris.Select(v => new Vector3().dx()).ToArray());

            var s = new VoxelSurface(vRenderer);
            s.MeshesWithMaterial.Add(new MeshWithMaterial(rawmesh, null));

            vRenderer.AddSurface(s);
            return s;
        }

        [Test]
        public void TestRenderFlatWorldOctree()
        {
            worldTree = CreateFlatWorldOctree(worldSize);

            /*var tris = dcAlgo.GenerateTrianglesForOctree(worldTree);
            if (tris.Length != 0)
            {
                var m = new MeshBuilder().Alter(b => b.AddCustom(tris.Select(t => t.dx()).ToArray())).CreateMesh();
                var el = ren.CreateMeshElement(m);

            }*/
            engine.AddSimulator(new WorldRenderingSimulator());


            engine.AddSimulator(createShowOctreeAction(), "OctreeLines");
            engine.AddSimulator(createClipmapsRenderer(), "ClipMapsUpdater");

            engine.AddSimulator(createHermiteDataVisualizer(), "HermiteDataVisualizer");

        }

        private Action createHermiteDataVisualizer()
        {
            return () =>
            {
                treeHelper.VisitDepthFirst(worldTree, n =>
                {
                    if ( n.Children == null && !( n.Signs.All( b => b ) || n.Signs.All( b => !b ) ) )
                    {
                        TW.Graphics.LineManager3D.AddCenteredBox(n.QEF * n.Size + n.LowerLeft, n.Size * 0.25f, Color.Red.dx());

                    }
                        
                });
            };
        }

        private Action createShowOctreeAction()
        {
            var visible = false;
            Resolve<TestInfoUserinterface>().Text += "\nPress N to show octree";

            treeHelper = new ClipMapsOctree<SignedOctreeNode>();

            return () =>
            {
                if (TW.Graphics.Keyboard.IsKeyPressed(Key.N)) visible = !visible;

                if (!visible) return;
                treeHelper.DrawLines(worldTree, TW.Graphics.LineManager3D, 3);

                treeHelper.DrawLines(worldTree, TW.Graphics.LineManager3D,
                    delegate (SignedOctreeNode n, out bool visitChildren, out Color c)
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

            return () =>
            {
                if (TW.Graphics.Keyboard.IsKeyPressed(Key.C)) followCam = !followCam;

                if (followCam)
                {
                    pos = TW.Graphics.SpectaterCamera.CameraPosition;

                    UpdateQuadtreeClipmaps(worldTree, pos, minNodeSize);

                    foreach (var n in visibilityChangedNodes)
                    {
                        if (n.IsVisibilityLeaf)
                            ensureMesh(n);
                        else
                            removeMesh(n);
                    }
                    visibilityChangedNodes.Clear();
                }
                else
                {
                    // Draw cam
                    TW.Graphics.LineManager3D.AddCenteredBox(pos, 2, Color.Red.dx());
                }


            };
        }

        private void removeMesh(SignedOctreeNode n)
        {
            if (n.Mesh == null) return;
            n.Mesh.Delete();
            n.Mesh = null;
        }

        private void ensureMesh(SignedOctreeNode n)
        {
            if (n.Mesh != null) return;
            n.Mesh = generateMeshElement(n, n.Depth + surfaceDepth);
            //if (n.Mesh != null)
            //    n.Mesh.WorldMatrix = Matrix.Translation(new Vector3(0, n.Depth * 0.1f, 0));
        }

        private void UpdateQuadtreeClipmaps(SignedOctreeNode node, Vector3 cameraPosition, int minNodeSize)
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

        private void setAsVisiblityLeaf(SignedOctreeNode node)
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


        public SignedOctreeNode CreateFlatWorldOctree(int worldSize)
        {
            var builder = new SignedOctreeBuilder();

            var tree = builder.ConvertHermiteGridToOctree(new DensityFunctionHermiteGrid(p =>
           {
               p -= new Vector3(worldSize * 0.5f);
               return p.Length() - worldSize * 0.4f;
               /*p.X += worldSize / 2.0f;
               p.Y += worldSize / 2.0f;
               return p.Y*p.Y + p.X*p.X;*/
           }, new Point3(worldSize + 1, worldSize + 1, worldSize + 1)));
            return tree;
        }
    }
}