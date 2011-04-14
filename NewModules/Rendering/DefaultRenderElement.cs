using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// Represents a single Renderable element, that is used an manipulated as a whole. 
    /// It represents something that can be deleted as a whole.
    /// </summary>
    public class DefaultRenderElement
    {
        public IDefaultRenderable Renderable { get; private set; }

        public Matrix WorldMatrix{get; set; }

        public DefaultRenderElement(IDefaultRenderable renderable)
        {
            Renderable = renderable;
            WorldMatrix = Matrix.Identity;
        }
    }
}
