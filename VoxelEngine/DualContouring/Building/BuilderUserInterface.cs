using System;
using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.DataTypes;
using MHGameWork.TheWizards.DualContouring.Rendering;
using MHGameWork.TheWizards.Engine.Tests.Facades;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

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
        public Vector3 HitNormal { get; private set; }

        public ITool ActiveTool { get; set; }

        public BuilderUserInterface(FiniteWorld world)
        {
            this.world = world;

            ActiveTool = new GridPlacerTool();

        }

        public void processUserInput()
        {
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

                    var placer = new BasicShapeBuilder().CreateCube(ui.placementGridSize);
                    ui.PlaceInWorld(placer, placeOffset);
                }
                if (TW.Graphics.Mouse.RightMouseJustPressed)
                {
                    Point3 placeOffset = (targetCube.ToVector3() * placementWorldSize / ui.voxelSize).ToPoint3Rounded();

                    var placer = new BasicShapeBuilder().CreateCube(ui.placementGridSize);
                    ui.RemoveFromWorld(placer, placeOffset);
                }
            }
        }

        public class GridPlacerTool : ITool
        {
            public void UpdateTool(FiniteWorld world, BuilderUserInterface ui)
            {
                var targetCube = ui.AlignHitpointToGrid(ui.HitPoint - ui.HitNormal * ui.voxelSize * 0.1f, ui.MinPlacementSize);

                var halfVoxel = new Vector3(ui.voxelSize / 2);
                var placementWorldSize = ui.placementGridSize * ui.voxelSize;
                var minPlacamentWorldSize = ui.MinPlacementSize*ui.voxelSize;
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
                return new BasicShapeBuilder().CreateCube(ui.placementGridSize);
            }
        }


    }
}