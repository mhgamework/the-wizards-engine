using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Versioning
{
    public interface IStreamProvider
    {
        Stream OpenFileRead(string path);
    }
}
