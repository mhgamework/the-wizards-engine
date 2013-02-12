using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.RTS.Commands
{
    [ModelObjectChanged]
    public class ResourceType :EngineModelObject
    {
        public ITexture Texture { get; set; }
    }
}
