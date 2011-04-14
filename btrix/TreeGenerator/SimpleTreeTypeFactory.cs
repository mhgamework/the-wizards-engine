using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeGenerator
{
    public class SimpleTreeTypeFactory:ITreeTypeFactory
    {
        private Dictionary<Guid, ITreeType> treeTypes = new Dictionary<Guid, ITreeType>();
        public void AddTreeType(Guid guid, ITreeType treeType)
        {
            treeTypes.Add(guid, treeType);
        }
        public ITreeType GetTreeType(Guid guid)
        {
            return treeTypes[guid];
        }
    }
}
