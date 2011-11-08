using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Building
{
    public class DynamicPlaceTool
    {
        private readonly DX11Game game;
        private readonly DynamicBlockFactory blockFactory;
        private readonly DynamicTypeFactory typeFactory;
        private readonly StraightWallPlacer straightWallPlacer;
        private readonly SkewWallPlacer skewWallPlacer;
        private readonly FloorPlacer floorPlacer;
        public DynamicPlaceMode PlaceMode;

        private int wallTypeIndex = 0;
        private int floorTypeIndex = 0;
        private int planeHeight = 0;

        public DynamicPlaceTool(DX11Game game, DynamicBlockFactory blockFactory, DynamicTypeFactory typeFactory, StraightWallPlacer straightWallPlacer, SkewWallPlacer skewWallPlacer, FloorPlacer floorPlacer)
        {
            this.game = game;
            this.blockFactory = blockFactory;
            this.typeFactory = typeFactory;
            this.straightWallPlacer = straightWallPlacer;
            this.skewWallPlacer = skewWallPlacer;
            this.floorPlacer = floorPlacer;
        }

        public void Update()
        {
            if (PlaceMode == DynamicPlaceMode.StraightWallMode)
                executeStraightWallMode();
            if (PlaceMode == DynamicPlaceMode.SkewWallMode)
                executeSkewWallMode();
            if (PlaceMode == DynamicPlaceMode.FloorMode)
                executeFloorMode();
        }

        private void executeStraightWallMode()
        {
            DynamicBlock selectedBlock;
            Point3 selectedPos;
            CalculatePlacePos(out selectedPos, out selectedBlock);

            

            if (game.Keyboard.IsKeyPressed(Key.E))
                straightWallPlacer.PlaceStraightWall(selectedPos, typeFactory.WallTypes[wallTypeIndex]);
            if (selectedBlock != null)
            {
                game.LineManager3D.AddCenteredBox(selectedBlock.Position, 1, System.Drawing.Color.White);

                if (game.Keyboard.IsKeyPressed(Key.R))
                    removeWall(selectedBlock.Position);
            }
            else
                game.LineManager3D.AddCenteredBox(selectedPos, 1, System.Drawing.Color.Blue);

        }

        private void executeSkewWallMode()
        {
            DynamicBlock selectedBlock;
            Point3 selectedPos;
            CalculatePlacePos(out selectedPos, out selectedBlock);

            if (game.Keyboard.IsKeyPressed(Key.E))
                skewWallPlacer.PlaceSkewWall(selectedPos, typeFactory.WallTypes[wallTypeIndex]);
            if (selectedBlock != null)
            {
                game.LineManager3D.AddCenteredBox(selectedBlock.Position, 1, System.Drawing.Color.White);

                if (game.Keyboard.IsKeyPressed(Key.R))
                    removeWall(selectedBlock.Position);
            }
            else
                game.LineManager3D.AddCenteredBox(selectedPos, 1, System.Drawing.Color.Blue);

        }

        private void removeWall(Point3 pos)
        {
            skewWallPlacer.RemoveSkewWall(pos);
            straightWallPlacer.RemoveStraightWall(pos);
        }

        private void executeFloorMode()
        {
            if (game.Keyboard.IsKeyPressed(Key.UpArrow))
                planeHeight++;
            if (game.Keyboard.IsKeyPressed(Key.DownArrow))
            {
                planeHeight--;
                if (planeHeight < 0)
                    planeHeight = 0;
            }

            drawGrid(planeHeight);
            Vector3 placePos = getPlaneIntersection(planeHeight);

            game.LineManager3D.AddCenteredBox(placePos, 0.1f, System.Drawing.Color.Blue);

            if (game.Keyboard.IsKeyPressed(Key.E))
                floorPlacer.PlaceFloor(placePos, typeFactory.FloorTypes[floorTypeIndex]);
            if (game.Keyboard.IsKeyPressed(Key.R))
                floorPlacer.RemoveFloor(placePos);
        }

        private void CalculatePlacePos(out Point3 placePos, out DynamicBlock selectedBlock)
        {
            Vector3 rawPlacePos = new Vector3(0, 0, 0);
            var ret = new Point3();
            DynamicBlock rayBlock;
            Vector3 intersectPoint;

            CalculateClosestBlockIntersection(out rayBlock, out intersectPoint);

            if (rayBlock == null)
            {
                rawPlacePos = getPlaneIntersection(0);
            }
            else
            {
                var placePoint = intersectPoint - rayBlock.Position;
                game.LineManager3D.AddCenteredBox(intersectPoint, 0.1f, System.Drawing.Color.Blue);
                var placeDir = getMainAxis(Vector3.Normalize(placePoint));
                rawPlacePos = rayBlock.Position + placeDir;
            }

            placePos = new Point3((int)Math.Floor(rawPlacePos.X), (int)Math.Floor(rawPlacePos.Y),
                                  (int)Math.Floor(rawPlacePos.Z));
            selectedBlock = rayBlock;
        }

        private void CalculateClosestBlockIntersection(out DynamicBlock block, out Vector3 intersectPoint)
        {
            Ray ray;
            ray.Position = game.SpectaterCamera.CameraPosition;
            ray.Direction = game.SpectaterCamera.CameraDirection;

            DynamicBlock closestBlock = null;
            float closestDist = 100;


            for (int i = 0; i < blockFactory.BlockList.Count(); i++)
            {
                var currentBB = new BoundingBox(blockFactory.BlockList[i].Position - new Vector3(0.5f, 0.5f, 0.5f),
                                                blockFactory.BlockList[i].Position + new Vector3(0.5f, 0.5f, 0.5f));
                var intersects = ray.xna().Intersects(currentBB.xna());

                if (intersects.HasValue && intersects < closestDist)
                {
                    closestDist = intersects.Value;
                    closestBlock = blockFactory.BlockList[i];
                }
            }

            block = closestBlock;
            intersectPoint = ray.Position + closestDist * ray.Direction; ;
        }

        private Vector3 getMainAxis(Vector3 dir)
        {
            var viewDir = dir;
            var maxViewComp = Math.Max(Math.Max(Math.Abs(viewDir.X), Math.Abs(viewDir.Y)), Math.Abs(viewDir.Z));

            var intersectDir = new Vector3(viewDir.X / maxViewComp, viewDir.Y / maxViewComp, viewDir.Z / maxViewComp);
            if (Math.Abs(intersectDir.X) != 1)
                intersectDir.X = 0;
            if (Math.Abs(intersectDir.Y) != 1)
                intersectDir.Y = 0;
            if (Math.Abs(intersectDir.Z) != 1)
                intersectDir.Z = 0;

            return intersectDir;
        }

        private Vector3 getPlaneIntersection(int height)
        {
            Ray ray = new Ray(game.SpectaterCamera.CameraPosition, game.SpectaterCamera.CameraDirection);
            Plane p = new Plane(new Vector3(0, 1, 0), height);
            var pos = new Vector3();

            var intersects = ray.xna().Intersects(p.xna());
            if (intersects.HasValue)
                pos = ray.Position + intersects.Value * ray.Direction;

            pos.Y = height;

            return pos;
        }
    
        private void drawGrid(int height)
        {
            Point3 middlePoint = new Point3((int)Math.Floor(getPlaneIntersection(height).X), (int)Math.Floor(getPlaneIntersection(height).Y), (int)Math.Floor(getPlaneIntersection(height).Z));
            int gridSize = 3;

            for (int i = -gridSize; i <= gridSize; i++)
            {
                for (int j = -gridSize; j <= gridSize; j++)
                {
                    game.LineManager3D.AddLine(middlePoint + new Vector3(i, 0, j), middlePoint + new Vector3(i, 0, -j),
                                               System.Drawing.Color.Blue);
                    game.LineManager3D.AddLine(middlePoint + new Vector3(-i, 0, j), middlePoint + new Vector3(i, 0, j),
                                               System.Drawing.Color.Blue);

                }
            }
        }
    }
}
