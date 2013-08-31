using System.Collections.Generic;
using System.Drawing;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Graphics;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering.Vegetation.Trees
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
        private List<Color4> color = new List<Color4>();
        public void Visualize(DX11Game game, Vector3 position)
        {
            color.Add(new Color4(Color.Black));
            color.Add(new Color4(Color.Green));
            color.Add(new Color4(Color.Red));
            game.LineManager3D.AddLine(position, position + Base.Direction.Heading * Base.Length, new Color4(Color.Black));
            
            visualizeChilderen(game, Base, position + Base.Direction.Heading * Base.Length);
        }
        private void visualizeChilderen(DX11Game game,TreeStructureSegment seg,Vector3 pos)
        {
           
            for (int i = 0; i < seg.Children.Count; i++)
            {
                game.LineManager3D.AddLine(pos, pos + seg.Children[i].Direction.Heading * seg.Children[i].Length,color[seg.LevelTextureData.Level]);
                visualizeChilderen(game, seg.Children[i], pos + seg.Children[i].Direction.Heading * seg.Children[i].Length);
            }
        }
        

        public static TreeStructure GetTestTreeStructure()
        {
            TreeTypeData treeTypeData = TreeTypeData.GetTestTreeType();
            TreeStructureGenerater gen = new TreeStructureGenerater();
            return gen.GenerateTree(treeTypeData, 468);
        }
        
    }
}
