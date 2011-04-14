using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.WorldDatabase;

namespace MHGameWork.TheWizards.Entity
{
    /// <summary>
    /// TODO: document
    /// </summary>
    public class ObjectCoreData : IDataElement
    {
        public List<IMesh> Meshes = new List<IMesh>();
    }
}
