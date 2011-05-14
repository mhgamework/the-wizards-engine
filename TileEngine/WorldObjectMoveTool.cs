using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Editor.Transform;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.TileEngine.SnapEngine;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.TileEngine
{
    public class WorldObjectMoveTool : IXNAObject
    {
        public XNAGame game;

        public bool Enabled { get; set; }

        /// <summary>
        /// if set to false, rotationmode = enabled.
        /// </summary>
        public bool TranslationEnabled
        {
            get { return translationEnabled; }
            set { translationEnabled = value; }
        }

        public World World;

        public EditorGizmoTranslation translationGizmo = new EditorGizmoTranslation();
        public EditorGizmoRotation rotationGizmo = new EditorGizmoRotation();

        private bool translationEnabled = false;

        private SimpleMeshRenderElement ghost;
        private bool isGhostActive;

        private Snapper snapper = new Snapper();
        private TileSnapInformationBuilder builder;
        private SimpleMeshRenderer renderer;

        private List<WorldObject> worldObjectList;
        private List<Transformation> transformations = new List<Transformation>();

        public WorldObjectMoveTool(XNAGame _game, World world, TileSnapInformationBuilder _builder, SimpleMeshRenderer _renderer)
        {
            game = _game;
            World = world;
            translationGizmo.Position = new Vector3(0, 0, 0);
            translationGizmo.Enabled = true;

            rotationGizmo.Position = new Vector3(0, 0, 0);
            rotationGizmo.Enabled = true;



            worldObjectList = world.WorldObjectList;
            snapper.AddSnapper(new SnapperPointPoint());
            builder = _builder;
            worldTileSnapper = new WorldTileSnapper(builder);
            renderer = _renderer;
        }

        WorldObject selectedWorldObject = null;
        float oldYaw, oldPitch, oldRoll;
        float newYaw, newPitch, newRoll;

        private WorldTileSnapper worldTileSnapper;



        public void Initialize(IXNAGame _game)
        {
            translationGizmo.Load(game);
            rotationGizmo.Load(game);
        }

        public void Render(IXNAGame _game)
        {
            if (!Enabled) return;
            game.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;


            if (isObjectSelected())
            {
                translationGizmo.Render(game);
                rotationGizmo.Render(game);
                game.LineManager3D.AddAABB(selectedWorldObject.ObjectType.BoundingBox, selectedWorldObject.WorldMatrix, Color.White);
            }


        }

        private bool isObjectSelected()
        {
            return selectedWorldObject != null;
        }

        public void Update(IXNAGame _game)
        {
            if (!Enabled) return;
            translationGizmo.Update(game);
            rotationGizmo.Update(game);
            processStates();

            //Cloning
            if (isObjectSelected() && game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) && game.Mouse.LeftMouseJustPressed)
            {
                WorldObject clone = World.CreateNewWorldObject(game, selectedWorldObject.ObjectType, renderer);
                clone.Rotation = selectedWorldObject.Rotation;
                clone.Position = selectedWorldObject.Position;

                selectedWorldObject = clone;
            }

            //Deleting
            if (isObjectSelected() && game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Delete))
            {
                World.DeleteWorldObject(selectedWorldObject);
                selectedWorldObject = null;
            }

            /*
            //Full Reset
            if (game.Mouse.RightMouseJustPressed && game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.R) && selectedWorldObject != null)
            {
                selectedWorldObject.Reset();
                translationGizmo.Position = selectedWorldObject.Position;
                rotationGizmo.RotationQuat = selectedWorldObject.Rotation;
            }
            //Local Reset
            if (game.Mouse.RightMouseJustPressed && selectedWorldObject != null)
            {
                Vector3 target = new Vector3(0, 1, 0);
                Vector3 source = Vector3.Transform(target, selectedWorldObject.Rotation);
                Matrix rotation;
                if (Vector3.Dot(source, target) > 0.999f)
                    rotation = Matrix.Identity;
                else
                    rotation = XnaMathExtensions.CreateRotationMatrixMapDirection(source, target);
                selectedWorldObject.Rotation = Quaternion.CreateFromRotationMatrix(rotation) * selectedWorldObject.Rotation;
                translationGizmo.Position = selectedWorldObject.Position;
                rotationGizmo.RotationQuat = selectedWorldObject.Rotation;
            }
                    
           */
        }

        private void processStates()
        {
            if (!isObjectSelected())
            {
                deleteGhost();
                disableGizmos();
                trySelect();
            }
            else if (isObjectSelected() && !gizmoActive())
            {
                if (ghost != null)
                {
                    Vector3 scale, translation;
                    Quaternion rotation;
                    ghost.WorldMatrix.Decompose(out scale, out rotation, out translation);
                    selectedWorldObject.Position = translation;
                    selectedWorldObject.Rotation = rotation;
                }

                deleteGhost();
                if (trySelect()) return;
                if (!isObjectSelected()) return;
                updateGizmoPosition();
                updateGizmoType();
            }
            else if (isObjectSelected() && gizmoActive())
            {
                updateSelectedObjectPosition();

                if (ghost == null)
                    ghost = renderer.AddMesh(selectedWorldObject.ObjectType.Mesh);

                updateGhostPosition();
            }
            else
            {
                throw new InvalidOperationException("Invalid state!");
            }
        }

        private void deleteGhost()
        {
            if (ghost == null) return;
            ghost.WorldMatrix = new Matrix();
            ghost = null;
        }

        private void disableGizmos()
        {
            translationGizmo.Enabled = false;
            rotationGizmo.Enabled = false;
        }

        private void updateGizmoType()
        {
            if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.R))
            {
                TranslationEnabled = !TranslationEnabled;
            }

            if (translationEnabled)
            {
                translationGizmo.Enabled = true;
                rotationGizmo.Enabled = false;
            }
            else
            {
                translationGizmo.Enabled = false;
                rotationGizmo.Enabled = true;
            }
        }

        private void updateSelectedObjectPosition()
        {
            selectedWorldObject.Position = translationGizmo.Position;

            Quaternion rotation = rotationGizmo.RotationQuat;

            float roundingValue = MathHelper.ToRadians(15);
            float yaw, pitch, roll;
            convertQuaternionToEuler(rotation, out yaw, out pitch, out roll);

            yaw = roundEulerAngleToXDegrees(yaw, roundingValue);
            pitch = roundEulerAngleToXDegrees(pitch, roundingValue);
            roll = roundEulerAngleToXDegrees(roll, roundingValue);
            Quaternion roundedQuat = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);

            selectedWorldObject.Rotation = roundedQuat;
        }

        private void updateGizmoPosition()
        {
            translationGizmo.Position = selectedWorldObject.Position;
            rotationGizmo.Position = selectedWorldObject.Position;
        }

        private bool trySelect()
        {
            if (!game.Mouse.CursorEnabled) return false;
            if (game.Mouse.LeftMouseJustPressed)
            {
                Ray ray = game.GetWereldViewRay(new Vector2(game.Mouse.CursorPosition.X, game.Mouse.CursorPosition.Y));
                WorldObject result = World.Raycast(ray, World.WorldObjectList);
                selectedWorldObject = result;
                return result != null;
            }
            return false;
        }

        private bool gizmoActive()
        {

            return !((translationGizmo.ActiveMoveMode == EditorGizmoTranslation.GizmoPart.None || translationGizmo.Enabled == false) &&
                    (rotationGizmo.ActiveMoveMode == EditorGizmoRotation.GizmoPart.None || rotationGizmo.Enabled == false));
        }

        private void updateGhostPosition()
        {
            worldObjectList.Remove(selectedWorldObject);
            var transformation = worldTileSnapper.CalculateSnap(selectedWorldObject.ObjectType.TileData, selectedWorldObject.Transformation, worldObjectList);
            worldObjectList.Add(selectedWorldObject);

            ghost.WorldMatrix = transformation.CreateMatrix();

        }

        private void convertQuaternionToEuler(Quaternion quat, out float yaw, out float pitch, out float roll)
        {
            Quaternion q1 = quat;
            double sqw = q1.W * q1.W;
            double sqx = q1.X * q1.X;
            double sqy = q1.Y * q1.Y;
            double sqz = q1.Z * q1.Z;
            double unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            double test = q1.X * q1.Y + q1.Z * q1.W;
            if (test > 0.499 * unit)
            { // singularity at north pole
                yaw = (float)(2 * Math.Atan2(q1.X, q1.W));
                pitch = (float)Math.PI / 2;
                roll = 0;
                return;
            }
            if (test < -0.499 * unit)
            { // singularity at south pole
                yaw = (float)(-2 * Math.Atan2(q1.X, q1.W));
                pitch = (float)-Math.PI / 2;
                roll = 0;
                return;
            }
            yaw = (float)Math.Atan2(2 * q1.Y * q1.W - 2 * q1.X * q1.Z, sqx - sqy - sqz + sqw);
            pitch = (float)Math.Asin(2 * test / unit);
            roll = (float)Math.Atan2(2 * q1.X * q1.W - 2 * q1.Y * q1.Z, -sqx + sqy - sqz + sqw);

            float swap = pitch;
            pitch = roll;
            roll = swap;
#if DEBUG

            Quaternion verify = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
            Vector3 up1 = Vector3.Transform(Vector3.Up, quat);
            Vector3 up2 = Vector3.Transform(Vector3.Up, verify);
            Vector3 right1 = Vector3.Transform(Vector3.Right, quat);
            Vector3 right2 = Vector3.Transform(Vector3.Right, verify);

            //Very  large rounding errors seem to occur
            /*if (Vector3.Dot(up1, up2) < 0.99f)
            {
                throw new InvalidOperationException();
            }
            if (Vector3.Dot(right1, right2) < 0.99f)
            {
                throw new InvalidOperationException();
            }*/
#endif

        }

        private float roundEulerAngleToXDegrees(float angle, float roundingValue)
        {
            angle += roundingValue * 0.5f;
            var rounded = (int)(angle / roundingValue) * roundingValue;
            return rounded;
        }


    }

}
