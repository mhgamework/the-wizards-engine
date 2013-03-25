using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.OBJParser
{
    public class OBJMaterial
    {
        public string Name { get; set;  }

        public Color AmbientColor;
        public Color DiffuseColor;
        public Color SpecularColor;

        public string AmbientMap;
        public string DiffuseMap;
        public string BumpMap;

        /// <summary>
        /// The obj Ns value
        /// </summary>
        public float SpecularExponent;


        public override string ToString()
        {
            var str = "Material: ";

            if (DiffuseMap != null) str += " Tex: " + DiffuseMap;

            return str;
        }



    }
}
