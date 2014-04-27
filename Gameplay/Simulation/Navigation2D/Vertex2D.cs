using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    public class Vertex2D
    {
        public Vertex2D()
        {
            MinDistance = -1;
        }

        public Vertex2D(Vector2 position)
            : this()
        {
            Position = position;

        }

        public Vector2 Position { get; set; }
        public int MinDistance { get; set; }
    }
}