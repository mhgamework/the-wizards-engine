using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Responsible for storing information about the engine in the TW model
    /// </summary>
    [ModelObjectChanged]
    public class EngineData : EngineModelObject
    {
        public bool PreviousStateLoaded { get; set; }
    }
}
