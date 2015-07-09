using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MHGameWork.TheWizards.Rendering
{
    public class TextureCoreData
    {
        // This could be temporary, but nevertheless comes in handy
        public enum TextureStorageType
        {
            None = 0,
            Disk,
            Assembly

        }

        public TextureStorageType StorageType;

        //Storage type Disk
        public string DiskFilePath;

        //Storage type Assembly
        public Assembly Assembly;
        public string AssemblyResourceName;

        public override string ToString()
        {
            return StorageType.ToString() + " + " + DiskFilePath + " + " + AssemblyResourceName + " + ";
        }
        


    }
}
