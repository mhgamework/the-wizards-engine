﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Rendering
{
    public class DefaultModelMaterialTextured : IDefaultModelMaterial
    {
        public Texture2D DiffuseTexture;

        public void SetMaterialToShader(DefaultModelShader shader)
        {
            shader.DiffuseTexture = DiffuseTexture;
            shader.Technique = DefaultModelShader.TechniqueType.Textured;
        }

    }
}
