using System;
using System.Drawing;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using SlimDX;
using SlimDX.DirectInput;

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

        public TerrainLodEnvironment()
        {
            tree = new LodOctree();

            var size = 32 * (1 << 4);
            rootNode = tree.Create(size, size);


            density = VoxelTerrainGenerationTest.createDensityFunction5Perlin(17, size / 2);
        }

        public void LoadIntoEngine(TWEngine engine)
        {
            TW.Graphics.SpectaterCamera.FarClip = 5000;
            engine.AddSimulator(UpdateQuadtreeClipmaps, "UpdateClipmaps");
            engine.AddSimulator(DrawQuadtreeLines, "UpdateLines");
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        public void DrawQuadtreeLines()
        {
            TW.Graphics.LineManager3D.WorldMatrix = Matrix.Translation(0, 300, 0);
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


            if (TW.Graphics.Keyboard.IsKeyDown(Key.F))
                pos = new Vector3(rootNode.size / 2f);

            TW.Graphics.LineManager3D.DrawGroundShadows = true;

            TW.Graphics.LineManager3D.AddCenteredBox(pos.ChangeY(0), 4, Color.Red);
            TW.Graphics.LineManager3D.WorldMatrix = Matrix.Translation(0, 300, 0);
            TW.Graphics.LineManager3D.AddCenteredBox(pos, 16, Color.Red);

            UpdateQuadtreeClipmaps(rootNode, pos);
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
                    tree.Split(node, false, 32);

                if (node.Children == null) return; // Minlevel

                for (int i = 0; i < 8; i++)
                {
                    UpdateQuadtreeClipmaps(node.Children[i], cameraPosition);
                }
            }
        }



    }
}