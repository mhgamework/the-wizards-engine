using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Synchronization;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// Contains data + states for WorldRendering in the model (singleton)
    /// </summary>
    [NoSync]
    public class RenderingModel : EngineModelObject
    {
        public RenderingModel()
        {
            MeshFactory = new SimpleMeshFactory();
            AssetFactory = new SimpleAssetFactory();
        }
        public SimpleMeshFactory MeshFactory { get; private set; }
        public SimpleAssetFactory AssetFactory { get; private set; }
    }
}
