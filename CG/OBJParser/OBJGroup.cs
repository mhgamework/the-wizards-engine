using System;
using System.Collections.Generic;
using System.Text;


namespace MHGameWork.TheWizards.CG.OBJParser
{
    public class OBJGroup
    {
        public List<SubObject> SubObjects { get; set; }
        public SubObject DefaultSubObject { get; set; }
        public string Name { get; set; }

        public OBJGroup()
        {
            SubObjects = new List<SubObject>();
            DefaultSubObject = new SubObject();

        }

        public OBJGroup(string name) :this()
        {
            Name = name;
        }

        /// <summary>
        /// When null is given, the default SubObject
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public SubObject GetOrCreateSubObject(OBJMaterial mat)
        {
            if (mat == null) return DefaultSubObject;
            for (int i = 0; i < SubObjects.Count; i++)
            {
                if (SubObjects[i].Material == mat)
                    return SubObjects[i];
            }

            SubObject obj = new SubObject();
            SubObjects.Add(obj);

            obj.Material = mat;

            return obj;
        }



        public class SubObject
        {
            public OBJMaterial Material { get; set; }

            public List<Face> Faces { get; set; }

            public SubObject()
            {
                Faces = new List<Face>();
            }

        }

        public override string ToString()
        {
            return "Group: Name:" + Name + " SubObjects:" + SubObjects.Count.ToString();
        }

    }
}
