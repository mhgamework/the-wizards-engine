using System;
using System.Collections.Generic;
using System.Text;

namespace TreeGenerator.LodEngine
{
    public interface ITreeLodLayer
    {
        bool AddEntity(TreeLodEntity ent);
        void RemoveEntity(TreeLodEntity ent);
        float MinDistance { get; set; }
        bool IsFull();
    }
}
