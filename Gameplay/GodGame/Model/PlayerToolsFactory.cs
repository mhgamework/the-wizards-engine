using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Types;

namespace MHGameWork.TheWizards.GodGame.Model
{
    /// <summary>
    /// Responsible for creating all player tools
    /// </summary>
    public class PlayerToolsFactory
    {
        private IEnumerable<IPlayerTool> tools;
        public IEnumerable<IPlayerTool> Tools { get { return tools; } }

        public PlayerToolsFactory(IEnumerable<IPlayerTool> tools)
        {
            this.tools = tools;
        }
    }


}