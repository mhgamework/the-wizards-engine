﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;


namespace MHGameWork.TheWizards.OBJParser
{
    public class OBJGroup
    {
        public List<SubObject> SubObjects { get; set; }
        public SubObject DefaultSubObject { get; set; }
        public string Name { get; set; }

        public OBJGroup()
        {
            SubObjects = new List<SubObject>();

        }

        public OBJGroup(string name)
        {
            SubObjects = new List<SubObject>();
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

            public List<Vector3> GetPositions(ObjImporter importer)
            {
                var positions = new List<Vector3>();
                foreach ( var f in Faces )
                {
                    positions.Add(importer.Vertices[f.V1.Position]);
                    positions.Add(importer.Vertices[f.V2.Position]);
                    positions.Add(importer.Vertices[f.V3.Position]);
                }
                return positions;
            }
        }

        public override string ToString()
        {
            return "Group: Name:" + Name + " SubObjects:" + SubObjects.Count.ToString();
        }

    }
}
