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
    public class BuilderUserInterface
    {
        private readonly IDeveloperConsole devConsole;
        private readonly FiniteWorld world;
        private VoxelCustomRenderer surfaceRenderer;

        private float voxelSize { get { return BuilderConfiguration.VoxelSize; } }

        private int placementGridSize = 4;
        public BuilderUserInterface(IDeveloperConsole devConsole, FiniteWorld world)
        {
            this.devConsole = devConsole;
            this.world = world;

            

            //TODO: add commands!

            //PlaceInWorld(createUnitBox(), new Point3(0, 20, 0));
       
        }

        public void processUserInput()
        {
            var ray = TW.Data.Get<CameraInfo>().GetCenterScreenRay();

            if (TW.Graphics.Mouse.RelativeScrollWheel > 0) { placementGridSize *= 2; devConsole.WriteLine("Tool size: " + placementGridSize); }
            if (TW.Graphics.Mouse.RelativeScrollWheel < 0) { placementGridSize /= 2; devConsole.WriteLine("Tool size: " + placementGridSize); }


            placementGridSize = (int)MathHelper.Clamp(placementGridSize, 1, 32);

            var raycaster = new Raycaster<Chunk>();
            world.Chunks.ForEach((c, p) => c.Raycast(raycaster, ray) );
         
            if (raycaster.GetClosest().IsHit)
            {
                Vector3 hitpoint = ray.GetPoint(raycaster.GetClosest().Distance);
                Vector3 normal = -raycaster.GetClosest().HitNormal; //TODO: invert hack

                TW.Graphics.LineManager3D.AddCenteredBox(hitpoint, 0.02f, Color.Yellow);
                TW.Graphics.LineManager3D.AddLine(hitpoint, hitpoint + normal * 0.06f, Color.CadetBlue);


                var targetCube = CalculatePlacementCube(hitpoint - normal * voxelSize * 0.1f, placementGridSize);

                var halfVoxel = new Vector3(voxelSize / 2);
                var placementWorldSize = placementGridSize * voxelSize;
                var targetBoundingBox = new BoundingBox((Vector3)targetCube.ToVector3() * placementWorldSize + halfVoxel,
                    (targetCube + new Vector3(1, 1, 1)) * placementWorldSize + halfVoxel);

                TW.Graphics.LineManager3D.AddBox(targetBoundingBox, Color.GreenYellow);



                if (TW.Graphics.Mouse.LeftMouseJustPressed)
                {
                    var addCube = CalculatePlacementCube(hitpoint + normal * 0.06f, placementGridSize);
                    Point3 placeOffset = (addCube.ToVector3() * placementWorldSize / voxelSize).ToPoint3Rounded();

                    var placer = new BasicShapeBuilder().CreateCube(placementGridSize);
                    PlaceInWorld(placer, placeOffset);


                }
                if (TW.Graphics.Mouse.RightMouseJustPressed)
                {
                    Point3 placeOffset = (targetCube.ToVector3() * placementWorldSize / voxelSize).ToPoint3Rounded();

                    var placer = new BasicShapeBuilder().CreateCube(placementGridSize);
                    RemoveFromWorld(placer, placeOffset);

                }
            }

        }

        public Point3 CalculatePlacementCube(Vector3 hitpoint, int cubeSize)
        {
            var halfVoxel = new Vector3(voxelSize / 2);
            var placementWorldSize = placementGridSize * voxelSize;
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



    }
}