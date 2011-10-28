using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for placement by the user of blocks in the world of given blocktype.
    /// </summary>
    public class PlaceTool
    {
        private readonly DX11Game game;
        private DeferredRenderer renderer;
        private BlockFactory blockFactory;
        private readonly BlockPlaceLogic blockPlaceLogic;

        private BlockType ghost;
        private Point3 currentGhostPos;


        public PlaceTool(DX11Game game, DeferredRenderer renderer, BlockFactory blockFactory, BlockPlaceLogic blockPlaceLogic)
        {
            this.game = game;
            this.renderer = renderer;
            this.blockFactory = blockFactory;
            this.blockPlaceLogic = blockPlaceLogic;
        }


        public void Update()
        {
            //if (ghost == null)
                //return;

            currentGhostPos = CalculatePlacePos();
            
            //ghost.WorldMatrix = Matrix.Translation(currentGhostPos+ new Vector3(0.5f,0,0.5f));


            if (game.Keyboard.IsKeyPressed(Key.E))
                PlaceBlock();

            if (game.Keyboard.IsKeyPressed(Key.R))
                DeleteBlock();

        }

        private Point3 CalculatePlacePos()
        {

            var placePos = new Vector3();
            var ret = new Point3();
            Block rayBlock;
            Vector3 intersectPoint;

            CalculateClosestBlockIntersection(out rayBlock, out intersectPoint);

            if (rayBlock == null)
            {
                Ray ray = new Ray(game.SpectaterCamera.CameraPosition, game.SpectaterCamera.CameraDirection);
                Plane p = new Plane(new Vector3(0, 1, 0), 0);

                var intersects = ray.xna().Intersects(p.xna());

                if (intersects.HasValue)
                {
                    placePos = ray.Position + intersects.Value * ray.Direction;
                    game.LineManager3D.AddLine(placePos, placePos + MathHelper.Up, new Color4(0, 0.5f, 0));
                }

            }
            else
            {
                var placePoint = intersectPoint - (rayBlock.Position + new Vector3(0.5f, 0.5f, 0.5f));
                game.LineManager3D.AddCenteredBox(intersectPoint, 0.2f, new Color4(0, 0.5f, 0));
                var placeDir = getMainAxis( Vector3.Normalize( placePoint));
                placePos = rayBlock.Position + placeDir;
            }
            ret.X = (int)Math.Floor( placePos.X);
            ret.Y = (int)Math.Floor(placePos.Y);
            ret.Z = (int)Math.Floor(placePos.Z);

            return ret;
        }

        private Vector3 getMainAxis(Vector3 dir)
        {
            var viewDir = dir;
            var maxViewComp = Math.Max(Math.Max(Math.Abs( viewDir.X), Math.Abs(viewDir.Y)), Math.Abs(viewDir.Z));

            var intersectDir = new Vector3(viewDir.X / maxViewComp, viewDir.Y / maxViewComp, viewDir.Z / maxViewComp);
            if (Math.Abs(intersectDir.X) != 1)
                intersectDir.X = 0;
            if (Math.Abs(intersectDir.Y) != 1)
                intersectDir.Y = 0;
            if (Math.Abs(intersectDir.Z) != 1)
                intersectDir.Z = 0;

            return intersectDir;
        }

        private void CalculateClosestBlockIntersection(out Block block, out Vector3 intersectPoint)
        {
            Ray ray;
            ray.Position = game.SpectaterCamera.CameraPosition;
            ray.Direction = game.SpectaterCamera.CameraDirection;

            Block closestBlock = null;
            float closestDist = 100;


            for (int i = 0; i < blockFactory.BlockList.Count(); i++)
            {
                var currentBB = new BoundingBox(blockFactory.BlockList[i].Position,
                                                blockFactory.BlockList[i].Position + new Vector3(1, 1, 1));
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

        private void PlaceBlock()
        {
            if (ghost == null) return;
            var block = blockFactory.CreateBlock(ghost, currentGhostPos);
           // blockFactory.AddBlock(block); is done by the blockfactory on creating a block
            blockPlaceLogic.CalulateBlocks();
        }


        private void DeleteBlock()
        {
            Block rayBlock;
            Vector3 intersectPoint;
            CalculateClosestBlockIntersection(out rayBlock, out intersectPoint);

            if (blockFactory.BlockList.Contains(rayBlock))
            {
                rayBlock.DeleteFromRenderer();
                blockFactory.BlockList.Remove(rayBlock);
                blockPlaceLogic.CalulateBlocks();
            }
        }


        public void SetBlockType(BlockType type)
        {
            ghost = type;
        }


    }
}
