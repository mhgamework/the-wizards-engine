using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Networking
{
    public class CodeGeneraterWriter
    {
        public BinaryWriter Writer;
     
        public void Write(Guid guid)
        {
            Writer.Write(guid.ToByteArray());
        }
    }
}
