using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.TileEngine.SnapEngine;

namespace MHGameWork.TheWizards.TileEngine
{
    [TestFixture]
    public class TileEngineTest
    {
        [Test]
        public void TestSnapTo()
        {
            XNAGame game = new XNAGame();
            var snapper = new Snapper();
            snapper.addSnapper(new SnapperPointPoint());
            var builder = new TileSnapInformationBuilder();

            var tileData1 = new TileData();
            tileData1.Dimensions = new Vector3(1, 2, 3);
            var tileData2 = new TileData();
            tileData2.Dimensions = new Vector3(2, 2, 5);

            var faceSnapType1 = new TileFaceType() { Name = "type1" };
            var faceSnapType2 = new TileFaceType() { Name = "type2" };

            tileData1.SetFaceType(TileFace.Front, faceSnapType1);
            tileData1.SetFaceType(TileFace.Right, faceSnapType2);

            tileData2.SetFaceType(TileFace.Front, faceSnapType1);
            tileData2.SetFaceType(TileFace.Back, faceSnapType1);

            var list = new List<ISnappableWorldTarget>();

            var worldTarget1 = new SimpleSnappableWorldTarget()
            {
                TileData = tileData1,
                SnapInformation = builder.CreateFromTile(tileData1),
                Transformation = new Transformation(Vector3.One, Quaternion.Identity, new Vector3(2, 1, 3))
            };
            var worldTarget2 = new SimpleSnappableWorldTarget()
            {
                TileData = tileData2,
                SnapInformation = builder.CreateFromTile(tileData2),
                Transformation = new Transformation(Vector3.One, Quaternion.Identity, new Vector3(-2, 1, 1))
            };

            list.Add(worldTarget1);
            list.Add(worldTarget2);

            var tileData1SnapInformation = builder.CreateFromTile(tileData1);

            var transformations = snapper.SnapTo(builder.CreateFromTile(tileData1), list);

            var colorTypeMap = new Dictionary<SnapType, Color>();
            colorTypeMap.Add(worldTarget1.SnapInformation.SnapList[0].SnapType, Color.Red);
            colorTypeMap.Add(worldTarget1.SnapInformation.SnapList[1].SnapType, Color.Green);

            game.DrawEvent += delegate
                                  {
                                      for (int i = 0; i < list.Count; i++)
                                      {
                                          var iTarget = (SimpleSnappableWorldTarget)list[i];
                                          renderTile(game, iTarget.Transformation.CreateMatrix(), iTarget.TileData, Color.White);

                                          renderSnapInformation(game, iTarget.Transformation.CreateMatrix(), iTarget.SnapInformation, colorTypeMap);
                                      }

                                      for (int i = 0; i < transformations.Count; i++)
                                      {
                                          var iTransformation = transformations[i];
                                          renderTile(game, iTransformation.CreateMatrix(), tileData1, Color.Yellow);
                                          renderSnapInformation(game, iTransformation.CreateMatrix(), tileData1SnapInformation, colorTypeMap);
                                      }
                                      
                                  };

            game.Run();

        }

        private void renderTile(XNAGame game, Matrix transformation, TileData tileData, Color color)
        {
            var box = new BoundingBox(-tileData.Dimensions * 0.5f, tileData.Dimensions * 0.5f);
            game.LineManager3D.AddAABB(box, transformation, color);
        }

        private void renderSnapInformation(XNAGame game, Matrix transformation, SnapInformation snapInformation, Dictionary<SnapType, Color> colorTypeMap)
        {
            for (int j = 0; j < snapInformation.SnapList.Count; j++)
            {
                SnapPoint point = snapInformation.SnapList[j];
                Vector3 startPos = point.Position;
                var color = colorTypeMap[point.SnapType];
                game.LineManager3D.AddLine(Vector3.Transform(startPos, transformation), Vector3.Transform(startPos + point.Normal, transformation), color);
                game.LineManager3D.AddLine(Vector3.Transform(startPos, transformation), Vector3.Transform(startPos + point.Up, transformation), color);
            }
        }
    }
}
