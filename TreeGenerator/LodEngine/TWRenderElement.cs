using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TreeGenerator.LodEngine
{
   public class TWRenderElement
   {
       public IRenderable RenderData;
       public TWRenderElement(IRenderable renderData)
       {
           RenderData = renderData;
       }
       public Matrix WorldMatrix { get; set; }
    }
}
