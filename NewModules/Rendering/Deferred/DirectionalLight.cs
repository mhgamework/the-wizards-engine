using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    public class DirectionalLight
    {
        public Vector3 LightDirection { get; set; }
        public Vector3 Color { get; set; }

        public bool ShadowsEnabled { get; set; }

        public FrustumCullerView[] ShadowViews { get; set; }

        public DirectionalLight(FrustumCuller culler)
        {

            LightDirection = Vector3.Normalize(new Vector3(1, 2, 1));
            Color = new Vector3(1, 1, 0.9f);

            ShadowViews = new FrustumCullerView[4];
            for (int i = 0; i < ShadowViews.Length; i++)
            {
                ShadowViews[i] = culler.CreateView();
            }

        }
    }
}
