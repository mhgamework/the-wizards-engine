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
        private readonly WallPlacer wallPlacer;
        private readonly FloorPlacer floorPlacer;
        public DynamicPlaceMode PlaceMode;

        private int wallTypeIndex = 0;
        private int floorTypeIndex = 0;
        public float planeHeight = -0.5f;

        private Vector3 ptpStart = new Vector3(1000,0,0);
        private Vector3 ptpEnd = new Vector3(1000,0,0);

        public DynamicPlaceTool(DX11Game game, DynamicBlockFactory blockFactory, DynamicTypeFactory typeFactory, WallPlacer wallPlacer,FloorPlacer floorPlacer)
        {
            this.game = game;
            this.blockFactory = blockFactory;
            this.typeFactory = typeFactory;
            this.wallPlacer = wallPlacer;
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
            if (PlaceMode == DynamicPlaceMode.PTPStraightMode)
                executePTPStraightMode();
            if (PlaceMode == DynamicPlaceMode.PTPSkewMode)
                executePTPSkewMode();
        }

        private void executeStraightWallMode()
        {
            DynamicBlock selectedBlock;
            Point3 selectedPos;
            CalculatePlacePos(out selectedPos, out selectedBlock);



            if (game.Keyboard.IsKeyPressed(Key.E))
                wallPlacer.PlaceStraightWall(selectedPos, typeFactory.WallTypes[wallTypeIndex]);
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
                wallPlacer.PlaceResolvedSkewWall(selectedPos, typeFactory.WallTypes[wallTypeIndex]);
            
            if (selectedBlock != null)
            {
                game.LineManager3D.AddCenteredBox(selectedBlock.Position, 1, System.Drawing.Color.White);

                if (game.Keyboard.IsKeyPressed(Key.R))
                    removeWall(selectedBlock.Position);
                if (game.Keyboard.IsKeyPressed(Key.T))
                    wallPlacer.RotateSkewWall(selectedBlock.Position);
            }
            else
                game.LineManager3D.AddCenteredBox(selectedPos, 1, System.Drawing.Color.Blue);

        }

        private void removeWall(Point3 pos)
        {
            wallPlacer.RemoveSkewWall(pos);
            wallPlacer.RemoveStraightWall(pos);
        }

        private void executeFloorMode()
        {
            if (game.Keyboard.IsKeyPressed(Key.UpArrow))
                planeHeight++;
            if (game.Keyboard.IsKeyPressed(Key.DownArrow))
            {
                planeHeight--;
                if (planeHeight < -0.5f)
                    planeHeight = -0.5f;
            }

            drawMovingGrid(planeHeight);
            Vector3 placePos = getPlaneIntersection(planeHeight);

            game.LineManager3D.AddLine(placePos + new Vector3(-0.5f, 0, 0), placePos + new Vector3(0.5f, 0, 0), System.Drawing.Color.Blue);
            game.LineManager3D.AddLine(placePos + new Vector3(0, 0, -0.5f), placePos + new Vector3(0, 0, 0.5f), System.Drawing.Color.Blue);

            if (game.Keyboard.IsKeyDown(Key.E))
                floorPlacer.PlaceFloor(placePos, typeFactory.FloorTypes[floorTypeIndex]);
            if (game.Keyboard.IsKeyDown(Key.R))
                floorPlacer.RemoveFloor(placePos);
        }

        private void executePTPStraightMode()
        {
            drawGrid(planeHeight);

            var pos = snapToFaceMids(getPlaneIntersection(planeHeight));

            if (ptpStart != new Vector3(1000, 0, 0) && Math.Abs(Vector3.Distance(ptpStart, pos)) > 1.01f)
                pos = new Vector3(1000, 0, 0);
            
                game.LineManager3D.AddCenteredBox(pos, 0.2f, System.Drawing.Color.LightBlue);

            if (game.Keyboard.IsKeyPressed(Key.R))
            {
                ptpStart = new Vector3(1000,0,0); //represents the 'NaN-vector3'
                ptpEnd = new Vector3(1000, 0, 0);
            }

            if(ptpEnd != new Vector3(1000,0,0))
            {
                wallPlacer.PlaceStraightWallsPointToPoint(ptpStart, ptpEnd, typeFactory.WallTypes[wallTypeIndex]);
                ptpStart = ptpEnd;
                ptpEnd = new Vector3(1000, 0, 0);
            }

            else if(ptpStart != new Vector3(1000,0,0))
            {
                game.LineManager3D.AddCenteredBox(ptpStart, 0.2f, System.Drawing.Color.Red);

                if (game.Keyboard.IsKeyPressed(Key.E))
                    ptpEnd = pos;
            }

            else if (game.Keyboard.IsKeyPressed(Key.E))
                ptpStart = pos;


        }

        private void executePTPSkewMode()
        {
            drawGrid(planeHeight);

            var pos = snapToFaceMids(getPlaneIntersection(planeHeight));
            
            if (ptpStart != new Vector3(1000, 0, 0) && Math.Abs(Vector3.Distance(ptpStart, pos)) > 1.01f)
                pos = new Vector3(1000, 0, 0);

            game.LineManager3D.AddCenteredBox(pos, 0.2f, System.Drawing.Color.LightBlue);

            if (game.Keyboard.IsKeyPressed(Key.R))
            {
                ptpStart = new Vector3(1000, 0, 0); //represents the 'NaN-vector3'
                ptpEnd = new Vector3(1000, 0, 0);
            }

            if (ptpEnd != new Vector3(1000, 0, 0))
            {
                wallPlacer.PlaceSkewWallsPointToPoint(ptpStart, ptpEnd, typeFactory.WallTypes[wallTypeIndex]);
                ptpStart = ptpEnd;
                ptpEnd = new Vector3(1000, 0, 0);
            }

            else if (ptpStart != new Vector3(1000, 0, 0))
            {
                game.LineManager3D.AddCenteredBox(ptpStart, 0.2f, System.Drawing.Color.Red);

                if (game.Keyboard.IsKeyPressed(Key.E))
                    ptpEnd = pos;
            }

            else if (game.Keyboard.IsKeyPressed(Key.E))
                ptpStart = pos;


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
                rawPlacePos = getPlaneIntersection(0f);
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

        private Vector3 getPlaneIntersection(float height)
        {
            Ray ray = new Ray(game.SpectaterCamera.CameraPosition, game.SpectaterCamera.CameraDirection);
            Plane p = new Plane(new Vector3(0, 1, 0), -height);
            var pos = new Vector3();

            var intersects = ray.xna().Intersects(p.xna());
            if (intersects.HasValue)
                pos = ray.Position + intersects.Value * ray.Direction;

            pos.Y = height;

            return pos;
        }

        private void drawMovingGrid(float height)
        {
            Vector3 rawPoint = getPlaneIntersection(height);
            Vector3 middlePoint = new Vector3((int)Math.Floor(rawPoint.X), height, (int)Math.Floor(rawPoint.Z));
            int gridSize = 3;

            for (int i = 0; i <= gridSize; i++)
            {
                for (int j = 0; j <= gridSize; j++)
                {
                    game.LineManager3D.AddLine(middlePoint + new Vector3(-i - 0.5f, 0, j + 0.5f), middlePoint + new Vector3(-i - 0.5f, 0, -j - 0.5f),
                                               System.Drawing.Color.Blue);
                    game.LineManager3D.AddLine(middlePoint + new Vector3(i + 0.5f, 0, j + 0.5f), middlePoint + new Vector3(i + 0.5f, 0, -j - 0.5f),
                                               System.Drawing.Color.Blue);
                    game.LineManager3D.AddLine(middlePoint + new Vector3(-i - 0.5f, 0, -j - 0.5f), middlePoint + new Vector3(i + 0.5f, 0, -j - 0.5f),
                                               System.Drawing.Color.Blue);
                    game.LineManager3D.AddLine(middlePoint + new Vector3(-i - 0.5f, 0, j + 0.5f), middlePoint + new Vector3(i + 0.5f, 0, j + 0.5f),
                                               System.Drawing.Color.Blue);

                }
            }
        }

        private void drawGrid(float height)
        {
            Vector3 middlePoint = new Vector3(0, height, 0);
            int gridSize = 50;

            for (int i = 0; i <= gridSize; i++)
            {
                for (int j = 0; j <= gridSize; j++)
                {
                    game.LineManager3D.AddLine(middlePoint + new Vector3(-i - 0.5f, 0, j + 0.5f), middlePoint + new Vector3(-i - 0.5f, 0, -j - 0.5f),
                                               System.Drawing.Color.Gray);
                    game.LineManager3D.AddLine(middlePoint + new Vector3(i + 0.5f, 0, j + 0.5f), middlePoint + new Vector3(i + 0.5f, 0, -j - 0.5f),
                                               System.Drawing.Color.Gray);
                    game.LineManager3D.AddLine(middlePoint + new Vector3(-i - 0.5f, 0, -j - 0.5f), middlePoint + new Vector3(i + 0.5f, 0, -j - 0.5f),
                                               System.Drawing.Color.Gray);
                    game.LineManager3D.AddLine(middlePoint + new Vector3(-i - 0.5f, 0, j + 0.5f), middlePoint + new Vector3(i + 0.5f, 0, j + 0.5f),
                                               System.Drawing.Color.Gray);

                }
            }
        }

        public Vector3 snapToFaceMids(Vector3 pos)
        {
            var ret1 = new Vector3((float)Math.Floor(pos.X) + 0.5f, planeHeight, (float)Math.Floor(pos.Z));
            var ret2 = new Vector3((float)Math.Floor(pos.X), planeHeight, (float)Math.Floor(pos.Z) + 0.5f);

            if (Math.Abs(Vector3.Distance(pos, ret1)) < Math.Abs(Vector3.Distance(pos, ret2)))
                return ret1;
            else
                return ret2;

        }
    }
}
