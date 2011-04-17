using System;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Editor
{
    /// <summary>
    /// TODO: make this so it can increase in size: idea to make different grids with each a greater interval, and than moving the grids so the center of them
    /// is beneath the camera.
    /// </summary>
    public class EditorGrid : IXNAObject
    {
        private IXNAGame game;

        public IXNAGame Game
        {
            get { return game; }
            set { game = value; }
        }

        private float interval;

        public float Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        private float majorInterval;

        public float MajorInterval
        {
            get { return majorInterval; }
            set { majorInterval = value; }
        }

        private Color majorColor;

        public Color MajorColor
        {
            get { return majorColor; }
            set { majorColor = value; }
        }



        private Color color;

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        private Color borderColor;

        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; }
        }


        private Vector2 size;

        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }

        private Vector3 position;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }


        public EditorGrid()
        {

            interval = 1;
            MajorInterval = 10;
            color = Color.White;
            majorColor = Color.Gray;
            borderColor = Color.Black;
            size = new Vector2( 100, 100 );
            position = Vector3.Zero;
        }


        public void Initialize(IXNAGame _game)
        {
            game = _game;

        }

        public void Render(IXNAGame _game)
        {
            //TODO: optimize this using its own vertexbuffer and indexbuffer instead of the linemanager

            Vector2 halfSize = size * 0.5f;

            //Draw the border around the grid.

            game.LineManager3D.AddRectangle(position, size, Vector3.Forward, Vector3.Right, borderColor);

            // do the x-axis
            for (float i = 0; i < halfSize.X; i += interval)
            {
                if (i % majorInterval == 0)
                {

                    game.LineManager3D.AddLine(new Vector3(position.X + i, position.Y, position.Z - halfSize.Y),
                        new Vector3(position.X + i, position.Y, position.Z + halfSize.Y), majorColor);
                    game.LineManager3D.AddLine(new Vector3(position.X - i, position.Y, position.Z - halfSize.Y),
                        new Vector3(position.X - i, position.Y, position.Z + halfSize.Y), majorColor);
                    continue;
                }



                game.LineManager3D.AddLine(new Vector3(position.X + i, position.Y, position.Z - halfSize.Y),
                    new Vector3(position.X + i, position.Y, position.Z + halfSize.Y), color);
                game.LineManager3D.AddLine(new Vector3(position.X - i, position.Y, position.Z - halfSize.Y),
                    new Vector3(position.X - i, position.Y, position.Z + halfSize.Y), color);
            }

            for (float i = 0; i < halfSize.Y; i += interval)
            {
                if (i % majorInterval == 0)
                {

                    game.LineManager3D.AddLine(new Vector3(position.X - halfSize.Y, position.Y, position.Z + i),
                        new Vector3(position.X + halfSize.Y, position.Y, position.Z + i), majorColor);
                    game.LineManager3D.AddLine(new Vector3(position.X - halfSize.Y, position.Y, position.Z - i),
                        new Vector3(position.X + halfSize.Y, position.Y, position.Z - i), majorColor);
                    continue;
                }



                game.LineManager3D.AddLine(new Vector3(position.X - halfSize.Y, position.Y, position.Z + i),
                    new Vector3(position.X + halfSize.Y, position.Y, position.Z + i), color);
                game.LineManager3D.AddLine(new Vector3(position.X - halfSize.Y, position.Y, position.Z - i),
                    new Vector3(position.X + halfSize.Y, position.Y, position.Z - i), color); ;
            }
        }

        public void Update(IXNAGame _game)
        {
        }
    }
}
