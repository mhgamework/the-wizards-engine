using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics.Xna.XML;
using MHGameWork.TheWizards.ServerClient.Database;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public interface IDiskSerializer
    {
        void SaveToDisk( DiskLoaderService service, TWXmlNode node );
        void LoadFromDisk( DiskLoaderService service, TWXmlNode node );

        string UniqueName { get;}
    }
}
