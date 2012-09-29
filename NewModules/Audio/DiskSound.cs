using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Audio
{
    /// <summary>
    /// Represents a sound that is stored on a Drive
    /// </summary>
    public class DiskSound : ISound
    {
        public string Path { get; set; }
    }
}
