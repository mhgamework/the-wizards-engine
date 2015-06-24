using System;
using System.Collections.Generic;
using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.Rendering;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DirectInput;
using Debug = System.Diagnostics.Debug;

namespace MHGameWork.TheWizards.DualContouring.Terrain
{
    /// <summary>
    /// Shows a terrain with lod based on clipmaps but with an octree like procworld (so no stitching chunks or continuous scrolling)
    /// </summary>
    public class TerrainLodEnvironment
    {
        public DualContouringMeshBuilder meshBuilder = new DualContouringMeshBuilder();
        private LodOctree tree;
        private LodOctreeNode rootNode;
        private Func<Vector3, float> density;
        private float angle = 0;
        private DensityFunctionHermiteGrid densityGrid;
        private readonly int minNodeSize;
        private Vector3 octreeOffset;

        public TerrainLodEnvironment()
        {
            //octreeOffset = new Vector3(0, 300, 0);
            octreeOffset = new Vector3(0, 0, 0);

            tree = new LodOctree();

            var size = 32 * (1 << 2);
            rootNode = tree.Create(size, size);


            density = VoxelTerrainGenerationTest.createDensityFunction5Perlin(17, size / 2);
            density = v => DensityHermiteGridTest.SineXzDensityFunction(v, 1/5f, size/2, 3);
            densityGrid = new DensityFunctionHermiteGrid(density, new Point3(size, size, size));
            minNodeSize = 8;
        }

        public void LoadIntoEngine(TWEngine engine)
        {
            //TW.Graphics.FixedTimeStepEnabled = true;
            //TW.Graphics.FixedTimeStep = 1 / 10f;
            TW.Graphics.SpectaterCamera.FarClip = 5000;
            engine.AddSimulator(UpdateQuadtreeClipmaps, "UpdateClipmaps");
            engine.AddSimulator(DrawQuadtreeLines, "UpdateLines");
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new RecordingSimulator());

        }

        public void DrawQuadtreeLines()
        {
            TW.Graphics.LineManager3D.WorldMatrix = Matrix.Translation(octreeOffset);
            TW.Graphics.LineManager3D.DrawGroundShadows = true;
            tree.DrawLines(rootNode, TW.Graphics.LineManager3D);
            TW.Graphics.LineManager3D.DrawGroundShadows = false;

        }

        public void UpdateQuadtreeClipmaps()
        {
            angle += TW.Graphics.Elapsed * 1f;
            //UpdateQuadtreeClipmaps(rootNode, TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.xna().Translation.dx());

            var pos = new Vector3(rootNode.size / 2f);

            //pos += new Vector3((float)Math.Sin(angle), (float)Math.Sin(angle), (float)Math.Cos(angle)) * rootNode.size * 0.4f;
            pos += new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle)) * rootNode.size * 0.4f;


            if (!TW.Graphics.Keyboard.IsKeyDown(Key.F))
                pos = new Vector3(rootNode.size / 2f);
            TW.Graphics.LineManager3D.DrawGroundShadows = true;

            TW.Graphics.LineManager3D.AddCenteredBox(pos.ChangeY(0), 4, Color.Red);
            TW.Graphics.LineManager3D.WorldMatrix = Matrix.Translation(octreeOffset);
            TW.Graphics.LineManager3D.AddCenteredBox(pos, 16, Color.Red);


            lock (rootNode)
            {
                UpdateQuadtreeClipmaps(rootNode, pos);
            }
            generateMissingMeshes();

        }
        public void UpdateQuadtreeClipmaps(LodOctreeNode node, Vector3 cameraPosition)
        {
            var center = node.LowerLeft.ToVector3() + new Vector3(1) * node.size * 0.5f;
            var dist = Vector3.Distance(cameraPosition, center);

            if (dist > node.size * 2f)
            {
                // This is a valid node size at this distance, so remove all children
                tree.Merge(node);
            }
            else
            {
                if (node.Children == null)
                    tree.Split(node, false, minNodeSize);

                if (node.Children == null) return; // Minlevel

                for (int i = 0; i < 8; i++)
                {
                    UpdateQuadtreeClipmaps(node.Children[i], cameraPosition);
                }
            }
        }


        public void generateMeshesJob()
        {
            bool stop = false;
            TW.Engine.RegisterOnClearEngineState(() => stop = true);

            while (!stop)
            {
                generateMissingMeshes();
            }
        }

        private void generateMissingMeshes()
        {
            var meshLessNodes = new List<LodOctreeNode>();

            lock (rootNode)
            {
                addMeshLessNodes(rootNode, meshLessNodes);
            }
            foreach (var node in meshLessNodes)
            {
                if (node.Children != null) continue;
                node.Mesh = calculateNodeMesh(node);
                var el = TW.Graphics.AcquireRenderer().CreateMeshElement(node.Mesh);
                float setApart = 1.1f;
                setApart = 1; // Disable spacing between cells
                el.WorldMatrix = Matrix.Scaling(new Vector3(node.size / minNodeSize)) * Matrix.Translation(node.LowerLeft.ToVector3()*setApart);

            }
        }

        private IMesh calculateNodeMesh(LodOctreeNode node)
        {
            var currScaling = node.size / minNodeSize;

            // Then we add another +1 to be able to connect the gaps between the hermite grids
            //TODO: do lod stitching here
            var gridSize = minNodeSize + 1; 

            var grid = HermiteDataGrid.CopyGrid(
                new DensityFunctionHermiteGrid(v => density(v * currScaling + node.LowerLeft.ToVector3()),
                                               new Point3(gridSize, gridSize, gridSize)));
            var mesh = meshBuilder.buildMesh(grid);

            return mesh;
        }

        private void addMeshLessNodes(LodOctreeNode n, List<LodOctreeNode> list)
        {
            if (n.Mesh == null) list.Add(n);

            if (n.Children == null) return;
            for (int i = 0; i < 8; i++)
            {
                addMeshLessNodes(n.Children[i], list);
            }
        }
    }
}