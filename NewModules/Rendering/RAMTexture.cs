using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Rendering
{
    public class RAMTexture : ITexture
    {
        private TextureCoreData coreData = new TextureCoreData();

        public Guid Guid { get; private set; }

        public RAMTexture()
        {
            Guid = Guid.NewGuid();
        }

        public TextureCoreData GetCoreData()
        {
            return coreData;
        }

        public override string ToString()
        {
            return coreData.ToString();
        }
    }
}
