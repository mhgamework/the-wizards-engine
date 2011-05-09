using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.Raycast;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.TileEngine;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.TileEngine.SnapEngine;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Tests.TileEngine
{
    public class SnapLearnTool : IXNAObject
    {
        private readonly TheWizards.TileEngine.World world;
        private SimpleRaycaster<WorldObject> raycaster;
        private SimpleRaycaster<TileFace> raycasterFace;
        private BoxMesh ghostFace;

        private bool PickOperandAState = true;
        private bool PickOperandBState;
        private bool SnapLearnState;


        private WorldObject tileA;
        private WorldObject tileB;

        private SnapperPointPoint snapper = new SnapperPointPoint();
        private TileSnapInformationBuilder builder;
        private SnapPoint PointB;
        private SnapPoint PointA;

        private SimpleMeshRenderElement ghost;
        private SimpleMeshRenderer renderer;
        private bool winding;
        private TileFace tileFaceA;
        private TileFace tileFaceB;

        private Random randomNbGenerator = new Random();

        public bool Enabled { get; set; }


        public SnapLearnTool(TheWizards.TileEngine.World world, SimpleMeshRenderer renderer, TileSnapInformationBuilder builder)
        {
            this.world = world;
            raycaster = new SimpleRaycaster<WorldObject>();
            raycasterFace = new SimpleRaycaster<TileFace>();
            this.renderer = renderer;
            this.builder = builder;

        }

        public void Initialize(IXNAGame _game)
        {
            ghostFace = new BoxMesh();
            _game.AddXNAObject(ghostFace);
        }

        public void Render(IXNAGame _game)
        {
            if (!Enabled) return;
        }

        public void Update(IXNAGame _game)
        {
            if (!Enabled) return;

            if (!_game.Mouse.CursorEnabled)
                return;

            WorldObject target = RaycastWorldTile(_game);

            if (target == null)
                return;

            TileFace resultFace = RaycastTileFace(_game, target);

            if (PickOperandAState)
            {
                if (_game.Mouse.LeftMouseJustPressed)
                {
                    tileA = target;
                    tileFaceA = resultFace;
                  

                    ghost = renderer.AddMesh(tileA.ObjectType.Mesh);

                    
                    PickOperandAState = false;
                    PickOperandBState = true;
                }
            }

            if (PickOperandBState)
            {
                if (tileA.Equals(target))
                    return;

                tileB = target;
                tileFaceB = resultFace;

                PointA = builder.GetPoint(tileA.ObjectType.TileData,tileFaceA,null
                    , tileA.ObjectType.TileData.GetTotalWinding(tileFaceA ) ^ winding);
                PointB = builder.GetPoint(tileB.ObjectType.TileData, tileFaceB, null
                    , tileB.ObjectType.TileData.GetTotalWinding(tileFaceB));


                List<Transformation> transformations = new List<Transformation>();

                snapper.SnapAToB(PointA, PointB, tileB.Transformation, transformations);

                if (transformations.Count != 0)
                    ghost.WorldMatrix = transformations[0].CreateMatrix();

                if (_game.Mouse.RightMouseJustPressed)
                {
                    winding = !winding;
                }
                if (_game.Mouse.LeftMouseJustPressed)
                {
                    PickOperandBState = false;
                    SnapLearnState = true;
                }
            }

            if (SnapLearnState)
            {

                if (tileA.ObjectType.TileData.GetFaceType(tileFaceA) == null)
                {
                    tileA.ObjectType.TileData.SetFaceType(tileFaceA, new TileFaceType());
                    //tileA.ObjectType.TileData.SetLocalWinding(tileFaceA, !tileB.ObjectType.TileData.GetLocalWinding(tileFaceB));
                }
                if (tileB.ObjectType.TileData.GetFaceType(tileFaceB) == null)
                {
                    tileB.ObjectType.TileData.SetFaceType(tileFaceB, new TileFaceType());
                    //tileB.ObjectType.TileData.SetLocalWinding(tileFaceB, !tileA.ObjectType.TileData.GetLocalWinding(tileFaceA));
                }
                if (getTypeA().GetRoot() == getTypeB().GetRoot()) return;

                TileFaceType newRoot = new TileFaceType();
                newRoot.Name = "AutoGen";

                getTypeA().GetRoot().FlipWinding ^= winding;

                getTypeA().GetRoot().SetParent(newRoot);
                getTypeB().GetRoot().SetParent(newRoot);
                

                tileA.ObjectType.SnapInformation = builder.CreateFromTile(tileA.ObjectType.TileData);
                tileB.ObjectType.SnapInformation = builder.CreateFromTile(tileB.ObjectType.TileData);

               

                SnapLearnState = false;
                PickOperandAState = true;

                ghost.WorldMatrix = ghost.WorldMatrix;
                Quaternion rotation;
                Vector3 translation;
                Vector3 scale;
                ghost.WorldMatrix.Decompose(out scale, out rotation, out translation);
                tileA.Rotation = rotation;
                tileA.Position = translation;

                ghost.WorldMatrix = new Matrix();
                ghostFace.WorldMatrix = new Matrix();

                Enabled = false;
                return;
            }

            SetGhostFaceAtFace(target, resultFace);
        }

        private TileFaceType getTypeB()
        {
            return tileB.ObjectType.TileData.GetFaceType(tileFaceB);
        }

        private TileFaceType getTypeA()
        {
            return tileA.ObjectType.TileData.GetFaceType(tileFaceA);
        }

        private void SetGhostFaceAtFace(WorldObject target, TileFace resultFace)
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


            ghostFace.Dimensions = dim;
            ghostFace.PivotPoint = Vector3.One * 0.5f;
            ghostFace.WorldMatrix = Matrix.CreateTranslation(normal * (Math.Abs(Vector3.Dot(normal, range * 0.5f)) + 0.01f)) * target.WorldMatrix;
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

            for (int i = 0; i < world.WorldObjectList.Count(); i++)
            {
                var obj = (WorldObject)world.WorldObjectList[i];
                var objectMatrix = Matrix.Invert(obj.WorldMatrix);
                var objBB = obj.ObjectType.TileData.GetBoundingBox();
                ray = _game.GetWereldViewRay(_game.Mouse.CursorPositionVector);
                ray.Position = Vector3.Transform(ray.Position, objectMatrix);
                ray.Direction = Vector3.TransformNormal(ray.Direction, objectMatrix);

                raycaster.AddResult(ray.Intersects(objBB), (WorldObject)world.WorldObjectList[i]);
            }

            return raycaster.ClosestObject;
        }
    }
}
