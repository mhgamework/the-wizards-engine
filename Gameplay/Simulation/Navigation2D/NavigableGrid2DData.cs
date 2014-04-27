using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.Navigation2D
{
    [ModelObjectChanged]
    public class NavigableGrid2DData : EngineModelObject
    {
        public NavigableGrid2DData()
        {
            Size = 10;
            NodeSize = 0.1f;
        }
        /// <summary>
        /// Dont persist?!!
        /// </summary>
        public NavigableGrid2D Grid { get; set; }
        public int Size { get; set; }
        public float NodeSize { get; set; }
    }
}
