using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeGenerator.EngineSynchronisation
{
    public class RAMTreeType : ITreeType
    {
        public RAMTreeType()
        {
            Guid = Guid.NewGuid();
        }
        public Guid Guid { get; private set; }

        public TreeTypeData Data { get; set; }
        TreeTypeData ITreeType.GetData()
        {
            return Data;
        }
    }
}
