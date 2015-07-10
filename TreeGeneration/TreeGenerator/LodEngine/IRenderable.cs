using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using Microsoft.Xna.Framework;

namespace TreeGenerator.LodEngine
{
    public interface IRenderable
    {
        void Render(IXNAGame game);
        void SetWorldMatrix(Matrix world);

    }
}
