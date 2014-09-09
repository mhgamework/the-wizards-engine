﻿using SlimDX;

namespace MHGameWork.TheWizards.GodGame
{
    public class PlayerState
    {
        public string Name = "Not Supported Yet";
        public IPlayerTool ActiveTool { get; set; }
        public BoundingBox SelectionBox { get; set; }
    }
}