using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.TileEngine.SnapEngine;
using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;


namespace MHGameWork.TheWizards.TileEngine
{
    [TestFixture]
    public class SnapEngineTest
    {
        [Test]
        public void TestSnapperPointPoint()
        {
            SnapPoint PointA = new SnapPoint();
            SnapPoint PointB = new SnapPoint();

            PointA.Position = new Vector3(1, 0, 2);
            PointA.Normal = Vector3.Normalize(new Vector3(1, 0, 1));
            PointA.Up = Vector3.Normalize(new Vector3(0, 1, 0));

            PointB.Position = new Vector3(2, 0, 3);
            PointB.Normal = Vector3.Normalize(new Vector3(3, 0, 4));
            PointB.Up = Vector3.Normalize(new Vector3(0, 1, 0));

            SnapperPointPoint pointSnapper = new SnapperPointPoint();
            List<Transformation> transformations = new List<Transformation>();

            pointSnapper.SnapAToB(PointA, PointB, Transformation.Identity, transformations);

            XNAGame game = new XNAGame();

            Matrix transformation = transformations[0].CreateMatrix();
            Vector3 APosTrans = PointA.Position;
            Vector3 ANormTrans = PointA.Normal;
            Vector3 AUpTrans = PointA.Up;




            game.DrawEvent += delegate
            {

                game.LineManager3D.AddLine(PointA.Position, PointA.Position + PointA.Normal, Color.Red);
                game.LineManager3D.AddLine(PointA.Position, PointA.Position + PointA.Up, Color.Yellow);

                game.LineManager3D.AddLine(PointB.Position, PointB.Position + PointB.Normal, Color.Red);
                game.LineManager3D.AddLine(PointB.Position,PointB.Position + PointB.Up, Color.Red);

                Vector3 offset = new Vector3(0.01f, 0.01f, 0.01f);
                game.LineManager3D.AddLine(Vector3.Transform(APosTrans, transformation) + offset, Vector3.Transform(APosTrans + ANormTrans , transformation) + offset, Color.Blue);
                game.LineManager3D.AddLine(Vector3.Transform(APosTrans, transformation) + offset, Vector3.Transform(APosTrans + AUpTrans , transformation) + offset, Color.Orange);

            };

            game.Run();


        }
    }
}
