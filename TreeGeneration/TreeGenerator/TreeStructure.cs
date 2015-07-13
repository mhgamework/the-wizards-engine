using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;
using TreeGenerator.help;


namespace TreeGenerator
{
    /// <summary>
    /// Data klasse
    /// </summary>
    public class TreeStructure
    {
        public TreeStructureSegment Base;

        // later ITexture
        public List<string> Textures=new List<string>();
        //this represent the width and height of the texture in real life this is linked to the texcoords in RenderData
        public float TextureWidth ;
        public float TextureHeight;

        public int AmountOfLevels;
        //public List<string> Bumps=new List<string>();// I will implement this later

        // public List<Vector2> UVMaps=new List<Vector2>();


        //seeder is just for visualisation
        private List<Color> color = new List<Color>();
        public void Visualize(XNAGame game, Vector3 position)
        {
            color.Add(Color.Black);
            color.Add(Color.Green);
            color.Add(Color.Red);
            game.LineManager3D.AddLine(position, position + Base.Direction.Heading * Base.Length,Color.Black );
            
            visualizeChilderen(game, Base, position + Base.Direction.Heading * Base.Length);
        }
        private void visualizeChilderen(XNAGame game,TreeStructureSegment seg,Vector3 pos)
        {
           
            for (int i = 0; i < seg.Children.Count; i++)
            {
                game.LineManager3D.AddLine(pos, pos + seg.Children[i].Direction.Heading * seg.Children[i].Length,color[seg.LevelTextureData.Level]);
                visualizeChilderen(game, seg.Children[i], pos + seg.Children[i].Direction.Heading * seg.Children[i].Length);
            }
        }


        public static TreeStructure GetTestTreeStructure(XNAGame game)
        {
            TreeTypeData treeTypeData = TreeTypeData.GetTestTreeType();
            TreeStructureGenerater gen = new TreeStructureGenerater();
            return gen.GenerateTree(treeTypeData, 468);
        }
        public static void TestTreeStructure()
        {
            XNAGame game = new XNAGame();

            TreeStructure treeStruct = new TreeStructure();
            TreeTypeData treeTypeData = TreeTypeData.GetTestTreeType();
            TreeStructureGenerater gen = new TreeStructureGenerater();
            game.InitializeEvent +=
                delegate
                    {
                        //treeTypeData = TreeTypeData.LoadFromXML("treeLevel2");
                        treeStruct= gen.GenerateTree(treeTypeData, 1);
                        
                    };
            game.DrawEvent +=
                delegate
                    {
                        treeStruct.Visualize(game,new Vector3(5,0,5));
                    };
            game.Run();
        }
    }
}
