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
            Grid = new NavigableGrid2D();
            Grid.Create(0.1f,100,100);
        }
        /// <summary>
        /// Dont persist?!!
        /// </summary>
        public NavigableGrid2D Grid { get; set; }
    }
}
