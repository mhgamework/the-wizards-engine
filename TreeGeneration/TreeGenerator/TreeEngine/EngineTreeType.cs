using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TreeGenerator.TreeEngine
{
     public class EngineTreeType// I don't know perhaps I should make a struct from this, it is supposed to be stored in the wizards database
    {
         public int seed=658;
         public string TreeType = "TreeLeaves";
         public Vector3 Position=Vector3.Zero;
    }
}
