using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.Raycast;
using MHGameWork.TheWizards.TileEngine;
using MHGameWork.TheWizards;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Tests.TileEngine
{
    public class SnapLearnTool : IXNAObject
    {
        private readonly TheWizards.TileEngine.World world;
        private SimpleRaycaster<WorldObject> raycaster;
        private SimpleRaycaster<TileFace> raycasterFace;
        private BoxMesh ghostMesh;


        public SnapLearnTool(TheWizards.TileEngine.World world)
        {
            this.world = world;
            raycaster = new SimpleRaycaster<WorldObject>();
            raycasterFace = new SimpleRaycaster<TileFace>();

        }

        public void Initialize(IXNAGame _game)
        {
            ghostMesh = new BoxMesh();
            _game.AddXNAObject(ghostMesh);
        }

        public void Render(IXNAGame _game)
        {

        }

        public void Update(IXNAGame _game)
        {
            if (!_game.Mouse.CursorEnabled)
                return;

            WorldObject target = RaycastWorldTile(_game);

            if (target == null)
                return;

            TileFace resultFace = RaycastTileFace(_game, target);

            SetGhostAtFace(target, resultFace);
        }

        private void SetGhostAtFace(WorldObject target, TileFace resultFace)
        {
            var bb = target.ObjectType.TileData.GetBoundingBox();
            var range = (bb.Max - bb.Min) * 0.5f;


            Vector3 right;
            Vector3 normal;
            Vector3 up;
            normal = TileSnapInformationBuilder.getFaceNormal(resultFace);
            up = TileSnapInformationBuilder.getFaceUp(resultFace);
            right = Vector3.Cross(up, normal);
            range *= 2f;
            Vector3 dim = new Vector3();
            var ghostWidth = 0.01f;


            dim += normal * ghostWidth;
            dim += up * Vector3.Dot(up, range);
            dim += right * Vector3.Dot(right, range);

            dim.X = Math.Abs(dim.X);
            dim.Y = Math.Abs(dim.Y);
            dim.Z = Math.Abs(dim.Z);


            ghostMesh.Dimensions = dim;
            ghostMesh.PivotPoint = Vector3.One * 0.5f;
            ghostMesh.WorldMatrix = Matrix.CreateTranslation(normal * (Math.Abs(Vector3.Dot(normal, range * 0.5f)) + 0.01f)) * target.WorldMatrix;
        }

        private TileFace RaycastTileFace(IXNAGame _game, WorldObject target)
        {
            var bb = target.ObjectType.TileData.GetBoundingBox();
            var range = (bb.Max - bb.Min) * 0.5f;

            Ray ray;
            var targetMatrix = Matrix.Invert(target.WorldMatrix);
            ray = _game.GetWereldViewRay(_game.Mouse.CursorPositionVector);
            ray.Position = Vector3.Transform(ray.Position, targetMatrix);
            ray.Direction = Vector3.TransformNormal(ray.Direction, targetMatrix);

            raycasterFace.Reset();
            Vector3 normal;
            Vector3 up;
            Vector3 right;


            for (int i = 0; i < 6; i++)
            {
                var face = (TileFace)(i + 1);
                normal = TileSnapInformationBuilder.getFaceNormal(face);
                up = TileSnapInformationBuilder.getFaceUp(face);
                right = Vector3.Cross(up, normal);
                var scaledNormal = normal * Math.Abs(Vector3.Dot(normal, range));
                var scaledUp = up * Math.Abs(Vector3.Dot(up, range));
                var scaledRight = right * Math.Abs(Vector3.Dot(right, range));
                var v1 = scaledNormal + scaledUp +
                         scaledRight;
                var v2 = scaledNormal + scaledUp -
                         scaledRight;
                var v3 = scaledNormal - scaledUp +
                         scaledRight;
                var v4 = scaledNormal - scaledUp -
                         scaledRight;

                float? result;
                Functions.RayIntersectsSquare(ref ray, ref v1, ref v2, ref v3, ref v4, out result);
                raycasterFace.AddResult(result, face);
            }

            return raycasterFace.ClosestObject;
        }

        private WorldObject RaycastWorldTile(IXNAGame _game)
        {
            raycaster.Reset();

            Ray ray;

            for (int i = 0; i < world.SnapTargetList.Count(); i++)
            {
                var obj = (WorldObject)world.SnapTargetList[i];
                var objectMatrix = Matrix.Invert(obj.WorldMatrix);
                var objBB = obj.ObjectType.TileData.GetBoundingBox();
                ray = _game.GetWereldViewRay(_game.Mouse.CursorPositionVector);
                ray.Position = Vector3.Transform(ray.Position, objectMatrix);
                ray.Direction = Vector3.TransformNormal(ray.Direction, objectMatrix);

                raycaster.AddResult(ray.Intersects(objBB), (WorldObject)world.SnapTargetList[i]);
            }

            return raycaster.ClosestObject;
        }
    }
}
