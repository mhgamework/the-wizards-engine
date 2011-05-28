using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Versioning
{
    public class Revision
    {
        public Guid Guid { get; set; }
        public Revision Parent { get; set; }
        public Revision MergedParent { get; set; }
        public string Message { get; set; }


    }
}
