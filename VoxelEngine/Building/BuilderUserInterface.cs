using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using DirectX11;
using MHGameWork.TheWizards.DataTypes;
using MHGameWork.TheWizards.DualContouring.Rendering;
using MHGameWork.TheWizards.Engine.Tests.Facades;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX.DirectInput;
using MHGameWork.TheWizards.IO;
using System.Linq;

namespace MHGameWork.TheWizards.DualContouring.Building
{
    /// <summary>
    /// Displays the user interface and processes user input
    /// </summary>
    public class BuilderUserInterface
    {
        private readonly FiniteWorld world;

        private float voxelSize { get { return BuilderConfiguration.VoxelSize; } }

        public Vector3 HitPoint { get; private set; }

        private int placementGridSize = 4;
        private int MinPlacementSize = 1;
        private int MaxPlacementSize = 32;
        private List<DCVoxelMaterial> materials;
        public int activeMaterial = 0;
        public Vector3 HitNormal { get; private set; }

        public bool SphereMode = false;

        public ITool ActiveTool { get; set; }

        public DCVoxelMaterial ActiveMaterial { get { return materials[activeMaterial]; } }

        private Textarea Text;

        public BuilderUserInterface(FiniteWorld world)
        {
            this.world = world;

            ActiveTool = new FreePlacerTool();

            materials = new List<DCVoxelMaterial>();

            DirectoryInfo materialsDir = TWDir.GameData.GetChild("VoxelEngine\\Materials");
            materials = materialsDir.GetFiles()
                        .Select(
                            name =>
                            new DCVoxelMaterial() { Texture = TW.Assets.LoadTexture("VoxelEngine\\Materials\\" + name) }).ToList();

            Text = new Textarea();
        }

        public void processUserInput()
        {
            int matChange = 0;
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.NumberPadPlus))
                matChange = 1;
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.NumberPadMinus))
                matChange = -1;

            activeMaterial = TWMath.nfmod(activeMaterial + matChange, materials.Count);

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.D1))
                ActiveTool = new SizeBasedGridPlacerTool();
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.D2))
                ActiveTool = new GridPlacerTool();
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.D3))
                ActiveTool = new FreePlacerTool();
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.D4))
                SphereMode = !SphereMode;




            var ray = TW.Data.Get<CameraInfo>().GetCenterScreenRay();

            if (TW.Graphics.Mouse.RelativeScrollWheel > 0) { placementGridSize *= 2; }
            if (TW.Graphics.Mouse.RelativeScrollWheel < 0) { placementGridSize /= 2; }

            placementGridSize = (int)MathHelper.Clamp(placementGridSize, MinPlacementSize, MaxPlacementSize);

            var raycaster = new Raycaster<Chunk>();
            world.Chunks.ForEach((c, p) => c.Raycast(raycaster, ray));



            if (!raycaster.GetClosest().IsHit) return;

            HitPoint = ray.GetPoint(raycaster.GetClosest().Distance);
            HitNormal = -raycaster.GetClosest().HitNormal;

            TW.Graphics.LineManager3D.AddCenteredBox(HitPoint, 0.02f, Color.Yellow);
            TW.Graphics.LineManager3D.AddLine(HitPoint, HitPoint + HitNormal * 0.06f, Color.CadetBlue);


            ActiveTool.UpdateTool(world, this);
        }

        public void DrawUI()
        {
            TW.Graphics.TextureRenderer.Draw(TW.Graphics.AcquireRenderer().TexturePool.LoadTexture(materials[activeMaterial].Texture), new SlimDX.Vector2(10, 10), new SlimDX.Vector2(128, 128));
            Text.Position = new SlimDX.Vector2(10, 128 + 10 + 10);
            Text.Size = new SlimDX.Vector2(200,200);
            Text.Text = "Tool: " + ActiveTool.GetType().Name + "\n" + "SphereMode: " + SphereMode;
        }

        /// <summary>
        /// Returns the 0,0,0 voxel of the cube which contains worldPoint 
        /// </summary>
        /// <param name="worldPoint"></param>
        /// <returns></returns>
        public Point3 WorldToVoxelCoord(Vector3 worldPoint)
        {
            var halfVoxel = new Vector3(voxelSize / 2);
            return ((worldPoint - halfVoxel) / voxelSize).ToFloored();
        }
        /// <summary>
        /// Returns the world space position of given voxel coordinate
        /// </summary>
        /// <param name="voxelCoord"></param>
        /// <returns></returns>
        public Vector3 VoxelCoordToWorld(Point3 voxelCoord)
        {
            var halfVoxel = new Vector3(voxelSize / 2);
            return (Vector3)voxelCoord.ToVector3() * voxelSize + halfVoxel;
        }



        public Point3 AlignHitpointToGrid(Vector3 hitpoint, int gridCellSize)
        {
            var halfVoxel = new Vector3(voxelSize / 2);
            var placementWorldSize = gridCellSize * voxelSize;
            var placementCubeCoord = ((hitpoint - halfVoxel) / placementWorldSize).ToFloored().ToVector3();
            return placementCubeCoord.ToPoint3Rounded();
        }


        public void PlaceInWorld(HermiteDataGrid source, Point3 offset)
        {
            var bb = new BoundingBox(offset.ToVector3() * BuilderConfiguration.VoxelSize, (offset + source.Dimensions).ToVector3() * BuilderConfiguration.VoxelSize);
            /*var c = chunks[new Point3()];
            //c.SetGrid(source);
            c.SetGrid(HermiteDataGrid.CopyGrid(new UnionGrid(c.Grid, source, offset)));
            c.UpdateSurface(surfaceRenderer);*/
            world.Chunks.ForEach((c, p) =>
            {
                if (c.Box.xna().Contains(bb.xna()) == Microsoft.Xna.Framework.ContainmentType.Disjoint) return;
                c.SetGrid(HermiteDataGrid.CopyGrid(new UnionGrid(c.Grid, source, offset - c.ChunkCoord * BuilderConfiguration.ChunkNumVoxels)));
            });
        }
        public void RemoveFromWorld(HermiteDataGrid source, Point3 offset)
        {
            var bb = new BoundingBox(offset.ToVector3() * BuilderConfiguration.VoxelSize, (offset + source.Dimensions).ToVector3() * BuilderConfiguration.VoxelSize);
            /*var c = chunks[new Point3()];
            //c.SetGrid(source);
            c.SetGrid(HermiteDataGrid.CopyGrid(new UnionGrid(c.Grid, source, offset)));
            c.UpdateSurface(surfaceRenderer);*/
            world.Chunks.ForEach((c, p) =>
            {
                if (c.Box.xna().Contains(bb.xna()) == Microsoft.Xna.Framework.ContainmentType.Disjoint) return;
                c.SetGrid(HermiteDataGrid.CopyGrid(new DifferenceGrid(c.Grid, source, offset - c.ChunkCoord * BuilderConfiguration.ChunkNumVoxels)));
            });
        }


        public void ChangeMaterial(HermiteDataGrid grid, DCVoxelMaterial mat)
        {
            grid.ForEachGridPoint(p =>
                {
                    if (grid.GetSign(p))
                        grid.SetMaterial(p, mat);
                });
        }

        public interface ITool
        {
            void UpdateTool(FiniteWorld world, BuilderUserInterface ui);
        }

        public class SizeBasedGridPlacerTool : ITool
        {
            public void UpdateTool(FiniteWorld world, BuilderUserInterface ui)
            {
                var targetCube = ui.AlignHitpointToGrid(ui.HitPoint - ui.HitNormal * ui.voxelSize * 0.1f, ui.placementGridSize);

                var halfVoxel = new Vector3(ui.voxelSize / 2);
                var placementWorldSize = ui.placementGridSize * ui.voxelSize;
                var targetBoundingBox = new BoundingBox((Vector3)targetCube.ToVector3() * placementWorldSize + halfVoxel,
                                                         (targetCube + new Vector3(1, 1, 1)) * placementWorldSize + halfVoxel);

                TW.Graphics.LineManager3D.AddBox(targetBoundingBox, Color.GreenYellow);


                if (TW.Graphics.Mouse.LeftMouseJustPressed)
                {
                    var addCube = ui.AlignHitpointToGrid(ui.HitPoint + ui.HitNormal * 0.06f, ui.placementGridSize);
                    Point3 placeOffset = (addCube.ToVector3() * placementWorldSize / ui.voxelSize).ToPoint3Rounded();

                    var placer = createPlacer(ui);
                    ui.PlaceInWorld(placer, placeOffset);
                }
                if (TW.Graphics.Mouse.RightMouseJustPressed)
                {
                    Point3 placeOffset = (targetCube.ToVector3() * placementWorldSize / ui.voxelSize).ToPoint3Rounded();

                    var placer = createPlacer(ui);
                    ui.RemoveFromWorld(placer, placeOffset);
                }
            }

            private static HermiteDataGrid createPlacer(BuilderUserInterface ui)
            {
                HermiteDataGrid placer;
                if (ui.SphereMode)
                    placer = new BasicShapeBuilder().CreateSphere(ui.placementGridSize);
                else
                    placer = new BasicShapeBuilder().CreateCube(ui.placementGridSize);
                ui.ChangeMaterial(placer, ui.ActiveMaterial);
                return placer;
            }
        }

        public class GridPlacerTool : ITool
        {
            public void UpdateTool(FiniteWorld world, BuilderUserInterface ui)
            {
                var targetCube = ui.AlignHitpointToGrid(ui.HitPoint - ui.HitNormal * ui.voxelSize * 0.1f, ui.MinPlacementSize);

                var halfVoxel = new Vector3(ui.voxelSize / 2);
                var placementWorldSize = ui.placementGridSize * ui.voxelSize;
                var minPlacamentWorldSize = ui.MinPlacementSize * ui.voxelSize;
                var targetBoundingBox = new BoundingBox((Vector3)targetCube.ToVector3() * minPlacamentWorldSize + halfVoxel,
                                                         (Vector3)targetCube.ToVector3() * minPlacamentWorldSize + new Vector3(1, 1, 1) * placementWorldSize + halfVoxel);

                TW.Graphics.LineManager3D.AddBox(targetBoundingBox, Color.GreenYellow);


                if (TW.Graphics.Mouse.LeftMouseJustPressed)
                {
                    var addCube = ui.AlignHitpointToGrid(ui.HitPoint + ui.HitNormal * 0.06f, ui.MinPlacementSize);
                    Point3 placeOffset = (addCube.ToVector3() * minPlacamentWorldSize / ui.voxelSize).ToPoint3Rounded();

                    var placer = createShape(ui);
                    ui.PlaceInWorld(placer, placeOffset);
                }
                if (TW.Graphics.Mouse.RightMouseJustPressed)
                {
                    Point3 placeOffset = (targetCube.ToVector3() * minPlacamentWorldSize / ui.voxelSize).ToPoint3Rounded();

                    var placer = createShape(ui);
                    ui.RemoveFromWorld(placer, placeOffset);
                }
            }


            private static HermiteDataGrid createShape(BuilderUserInterface ui)
            {
                HermiteDataGrid placer;
                if (ui.SphereMode)
                    placer = new BasicShapeBuilder().CreateSphere(ui.placementGridSize);
                else
                    placer = new BasicShapeBuilder().CreateCube(ui.placementGridSize);
                ui.ChangeMaterial(placer, ui.ActiveMaterial);
                return placer;
            }
        }

        public class FreePlacerTool : ITool
        {
            public void UpdateTool(FiniteWorld world, BuilderUserInterface ui)
            {
                var placementWorldSize = ui.placementGridSize * ui.voxelSize;


                var targetPoint = ui.HitPoint;

                var placementBB = new BoundingBox(targetPoint - new Vector3(placementWorldSize / 2f),
                                                   targetPoint + new Vector3(placementWorldSize / 2f));

                var minWorldVoxel = ui.WorldToVoxelCoord(placementBB.Minimum);
                var maxWorldVoxel = ui.WorldToVoxelCoord(placementBB.Maximum) + new Point3(1, 1, 1); // this is to do a round up

                var gridSize = maxWorldVoxel.X - minWorldVoxel.X;


                var spherePosVoxelSpace = (targetPoint - ui.VoxelCoordToWorld(minWorldVoxel)) / ui.voxelSize;


                IIntersectableObject shape;
                if (ui.SphereMode)
                    shape = new IntersectableSphere();
                else
                    shape = new IntersectableCube();

                var grid = HermiteDataGrid.FromIntersectableGeometry(gridSize, gridSize,
                                                           Matrix.Scaling(new Vector3(ui.placementGridSize) / 2.0f) *
                                                           Matrix.Translation(spherePosVoxelSpace),
                                                           shape);



                TW.Graphics.LineManager3D.AddBox(new SlimDX.BoundingBox(ui.VoxelCoordToWorld(minWorldVoxel), ui.VoxelCoordToWorld(maxWorldVoxel)), Color.GreenYellow);
                TW.Graphics.LineManager3D.AddBox(placementBB, Color.Red);

                // add one since the 0,0,0 is not visible in a surface, and the (voxelcoord 0,0,0) corresponds to the 1,1,1 in the hermite grid
                // TODO: this should be improved very complex
                Point3 offset = minWorldVoxel + new Point3(1, 1, 1);
                ui.ChangeMaterial(grid, ui.ActiveMaterial);

                if (TW.Graphics.Mouse.LeftMouseJustPressed)
                {
                    ui.PlaceInWorld(grid, offset);
                }
                if (TW.Graphics.Mouse.RightMouseJustPressed)
                {
                    ui.RemoveFromWorld(grid, offset);
                }
            }

            private static HermiteDataGrid createShape(BuilderUserInterface ui)
            {
                var placer = new BasicShapeBuilder().CreateSphere(ui.placementGridSize);
                ui.ChangeMaterial(placer, ui.ActiveMaterial);
                return placer;
            }
        }


    }
}