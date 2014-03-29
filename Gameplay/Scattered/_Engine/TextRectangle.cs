﻿using System;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered._Engine
{
    /// <summary>
    /// Part of the engine rendering facade. It allows adding a text rectangle to the scene.
    /// TODO: IDEA: try use something like the changessink here to make this class observable
    /// TODO: remove TW dependencies
    /// TODO: add uniform scene management interfaces here (eg ISceneObject containing visible, delete, transform etc)
    /// </summary>
    public class TextRectangle : IDisposable
    {
        private string _text;
        private int fontSize;
        public int FontSize
        {
            get { return fontSize; }
            set { fontSize = value; updateTexture(); }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                var changed = _text != value;
                _text = value;
                if (changed) updateTexture();
            }
        }



        public Vector3 Position { get; set; }
        public float Radius { get; set; }
        public Vector3 Normal { get; set; }
        public Matrix PreTransformation { get; set; }

        public bool IsBillboard { get; set; }

        /// <summary>
        /// Note: this is currently layer leak, to allow visibility and deletion, but should be removed and added to the scene layer
        /// </summary>
        public Entity Entity { get; private set; }

        public TextRectangle()
        {
            fontSize = 100;
            Entity = new Entity();
            Text = "[The Wizards Engine - Text]";
            PreTransformation = Matrix.Identity;
            Radius = 1;
            Normal = Vector3.UnitZ;

        }


        public void Update()
        {
            var up = Vector3.UnitY;
            if (Math.Abs(Vector3.Dot(up, Normal)) > 0.99) up = -Vector3.UnitZ;

            if (IsBillboard)
            {
                Normal = Vector3.Normalize(TW.Graphics.Camera.ViewInverse.xna().Translation.dx() - Position);
                up = TW.Graphics.Camera.ViewInverse.xna().Up.dx();
            }

            var camDist = Vector3.Distance(Position, TW.Graphics.Camera.ViewInverse.xna().Translation.dx());

            Entity.WorldMatrix = PreTransformation *
                Matrix.Scaling(Radius, Radius, Radius * camDist)
                * Matrix.Invert(Matrix.LookAtRH(Position, Position + Normal, up));
        }

        private void updateTexture()
        {
            Entity.Mesh = UtilityMeshes.CreateBoxWithTexture(UtilityMeshes.CreateTextureAssetFromText(_text, TW.Graphics), new Vector3(1, 1, 0.0002f));
            Update();
        }


        public void Dispose()
        {
            if (Entity != null)
                TW.Data.Objects.Remove(Entity);
            Entity = null;
        }
    }
}