using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Networking
{
    public class CodeGeneraterReader
    {
        public BinaryReader Reader;


        public Guid ReadGuid()
        {
            var bytes = Reader.ReadBytes(16);
            return new Guid(bytes);
        }
    }
}
