using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ModelContainer;

namespace MHGameWork.TheWizards.Assetbrowser
{
    public class AssetbrowserData : BaseModelObject
    {
        public Boolean BrowserEnabled { get; set; }
        public AssetBrowserCamera Camera { get; set; }
    }
}
