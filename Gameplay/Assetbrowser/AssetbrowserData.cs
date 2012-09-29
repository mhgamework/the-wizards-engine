using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ModelContainer;
using SlimDX;

namespace MHGameWork.TheWizards.Assetbrowser
{
    public class AssetbrowserData : EngineModelObject
    {
        public Boolean BrowserEnabled { get; set; }
        public Vector3 CameraPosition { get; set; }
        public Vector3 CameraDirection { get; set; }
    }
}
