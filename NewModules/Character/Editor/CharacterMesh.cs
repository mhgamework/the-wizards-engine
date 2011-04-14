using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.ServerClient.Graphics;
using MHGameWork.TheWizards.Common.Core.Collada;
using MHGameWork.TheWizards.ServerClient.Entity.Rendering;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Common.Core.Graphics;

namespace MHGameWork.TheWizards.Character.Editor
{
    public class CharacterMesh
    {

        private SkinnedShader shader;
        public SkinnedShader Shader { get { return shader; } }
        
        public Primitives Primitives;

        public void SetWorldMatrix( Matrix worldMatrix )
        {

            //shader.World = worldMatrix;
        }

        public CharacterMesh( Character model )
        {
            shader = model.BaseShader.Clone();

        }

        public void Render()
        {
            shader.RenderPrimitiveSinglePass( Primitives, SaveStateMode.None );
        }

    }
}
