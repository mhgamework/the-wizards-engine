using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Synchronization;
using SlimDX;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// Represents a WireframeBox in the world
    /// Adds a wireframe boundingbox to the renderer. The box with identity worldmatrix is origin centered from -0.5 to 0.5
    /// </summary>
    [NoSync]
    public class WireframeBox : EngineModelObject
    {

        public WireframeBox()
        {
            WorldMatrix = Matrix.Identity;
            Visible = true;
        }

        public void FromBoundingBox(BoundingBox box)
        {
            float xScale = box.Maximum.X - box.Minimum.X;
            float yScale = box.Maximum.Y - box.Minimum.Y;
            float zScale = box.Maximum.Z - box.Minimum.Z;

            Vector3 translation = new Vector3(box.Minimum.X + xScale*0.5f, box.Minimum.Y + yScale*0.5f,
                                              box.Minimum.Z + zScale*0.5f);

            WorldMatrix = Matrix.Scaling(new Vector3(xScale, yScale, zScale))*Matrix.Translation(translation);
        }


        public Matrix WorldMatrix { get; set; }
        public bool Visible { get; set; }
        public Color4 Color { get; set; }

    }
}
