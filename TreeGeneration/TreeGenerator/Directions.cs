using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework.Graphics;

namespace TreeGenerator.help
{
    public struct Directions
    {
        public Vector3 Heading;
        public Vector3 Right;
        public Directions(Vector3 heading, Vector3 right)
        {
            Heading = heading;
            Right = right;
        }
        public static Directions DirectionsFromAngles(Directions _dirs, float _dropAngle, float _axialSplit)
        {
            Directions d;
            Vector3 right;
            Vector3 heading;
            right = Vector3.Transform(_dirs.Right, Matrix.CreateFromAxisAngle(_dirs.Heading, _axialSplit));// soùething id wrong poistion????
            heading = Vector3.Transform(_dirs.Heading, Matrix.CreateFromAxisAngle(right, _dropAngle));
            d.Heading = heading;
            d.Right = right;

            return d;

        }

        public static Directions DirectionsFromAngles(Directions _dirs, float _dropAngle, float _axialSplit, float _wobbelaxialsplit, float _wobbelDropAngle)
        {
            return DirectionsFromAngles(_dirs, _dropAngle + _wobbelDropAngle, _axialSplit + _wobbelaxialsplit);
        }

        public static Directions DirectionsFromAngleDown(Directions _dirs, float _dropAngle)
        {
            Directions d;
            Vector3 vec1=_dirs.Heading;
            d.Right = _dirs.Right;
            if (vec1 == Vector3.Up)
            {
                d.Heading = vec1;
                d=DirectionsFromAngles(d, _dropAngle, 0);
            }
            else
            {
                vec1.Y = 0;
                vec1.Normalize();
                Vector3 vec2 = Vector3.Cross(vec1, Vector3.Down);
                d.Heading = Vector3.Transform(_dirs.Heading, Matrix.CreateFromAxisAngle(vec2, _dropAngle));
                
            }

            return d;
        }
        /// <summary>
        /// Test render lines
        /// </summary>
        public static void TestDirectionsFromAngles()
        {
            TestXNAGame game = null;
            Directions dirs1 = new Directions();
            Directions dirs2 = new Directions();
            Directions dirs3 = new Directions();
            float dropAngle2 = MathHelper.PiOver4;
            float axisSplit2 = 0;
            float dropAngle3 = 0;
            float axisSplit3 = 0;
            Vector3 position1 = new Vector3(2, 1, 2);
            Vector3 position2 = new Vector3();
            Vector3 position3 = new Vector3();
            float length = 3;
            TestXNAGame.Start("TestDirectionsFromAngles",
                delegate
                {
                    game = TestXNAGame.Instance;
                    //game.RenderAxis = false;
                    //dirs1 = new Directions(Vector3.Up, Vector3.UnitZ);
                    dirs1 = new Directions(Vector3.Normalize(new Vector3(1,1,0)), Vector3.UnitZ);

                    
                },
                delegate // 3d render code
                {
                    if (game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                    {
                        axisSplit2 -= game.Elapsed * 0.3f * MathHelper.TwoPi;
                    }
                    if (game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                    {
                        axisSplit2 += game.Elapsed * 0.3f * MathHelper.TwoPi;
                    }
                    if (game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                    {
                        dropAngle2 -= game.Elapsed * 0.3f * MathHelper.TwoPi;
                    }
                    if (game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                    {
                        dropAngle2 += game.Elapsed * 0.3f * MathHelper.TwoPi;
                    }
                    if (dropAngle2 < 0) dropAngle2 = 0;
                    if (dropAngle2 > MathHelper.Pi) dropAngle2 = MathHelper.Pi;

                    if (game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad4))
                    {
                        axisSplit3 -= game.Elapsed * 0.3f * MathHelper.TwoPi;
                    }
                    if (game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad6))
                    {
                        axisSplit3 += game.Elapsed * 0.3f * MathHelper.TwoPi;
                    }
                    if (game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad8))
                    {
                        dropAngle3 -= game.Elapsed * 0.3f * MathHelper.TwoPi;
                    }
                    if (game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad2))
                    {
                        dropAngle3 += game.Elapsed * 0.3f * MathHelper.TwoPi;
                    }
                    if (dropAngle3 < 0) dropAngle3 = 0;
                    if (dropAngle3 > MathHelper.Pi) dropAngle3 = MathHelper.Pi;

                    dirs2 = Directions.DirectionsFromAngleDown(dirs1, dropAngle2);//, axisSplit2);
                    position2 = position1 + length * dirs1.Heading;

                    dirs3 = Directions.DirectionsFromAngleDown(dirs2, dropAngle3);//, axisSplit3);
                    position3 = position2 + length * dirs2.Heading;

                    game.LineManager3D.AddLine(position1, position1 + dirs1.Heading, Color.Red);
                    game.LineManager3D.AddLine(position1, position1 + dirs1.Right, Color.Green);

                    game.LineManager3D.AddLine(position2, position2 + dirs2.Heading, Color.Red);
                    game.LineManager3D.AddLine(position2, position2 + dirs2.Right, Color.Green);

                    game.LineManager3D.AddLine(position3, position3 + dirs3.Heading, Color.Red);
                    game.LineManager3D.AddLine(position3, position3 + dirs3.Right, Color.Green);


                });
        }// TestRenderLines()

    }
}
