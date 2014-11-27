using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Types;

namespace MHGameWork.TheWizards.GodGame.Model
{
    /// <summary>
    /// Responsible for creating all player tools
    /// </summary>
    public class PlayerToolsFactory
    {
        private IEnumerable<PlayerTool> tools;
        public IEnumerable<PlayerTool> Tools { get { return tools; } }

        public PlayerToolsFactory(IEnumerable<PlayerTool> tools)
        {
            this.tools = tools;
        }
    }


}