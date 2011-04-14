using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient
{
    public class DatabaseMesh : IUnique
    {
        private int id = -1;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public void SaveToXml( TWXmlNode node )
        {
        }

        public void LoadFromXml( TWXmlNode node )
        {
        }


    }
}
