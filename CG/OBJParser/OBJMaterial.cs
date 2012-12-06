using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.OBJParser
{
    public class OBJMaterial
    {
        public string Name { get; set;  }

        public Color4 AmbientColor;
        public Color4 DiffuseColor;
        public Color4 SpecularColor;

        public string AmbientMap;
        public string DiffuseMap;

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
