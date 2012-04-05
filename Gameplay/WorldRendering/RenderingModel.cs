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
    public class RenderingModel : BaseModelObject
    {
        public RenderingModel()
        {
            MeshFactory = new SimpleMeshFactory();
        }
        public SimpleMeshFactory MeshFactory { get; private set; }
    }
}
