using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TreeGenerator.LodEngine
{
   public class TreeLodEntity
   {
       public TreeStructure TreeStructure;

       public TreeLodEntity(TreeStructure treeStructure)
       {
           TreeStructure = treeStructure;
       }

       public Matrix WorldMatrix { get; set; }
       public ITreeLodLayer LodLayer { get; set; }
       public ITreeLodLayer NextLodLayer { get; set; }
    }
}
