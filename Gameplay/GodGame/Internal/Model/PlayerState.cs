using SlimDX;

namespace MHGameWork.TheWizards.GodGame
{
    public class PlayerState
    {
        public PlayerState()
        {
            
        }
        public string Name = "NOT SET";
        public IPlayerTool ActiveTool { get; set; }
        public BoundingBox SelectionBox { get; set; }

        public int HeightToolSize { get; set; }
        public ChangeHeightToolPerPlayer.HeightToolState HeightToolState { get; set; }

    }
}