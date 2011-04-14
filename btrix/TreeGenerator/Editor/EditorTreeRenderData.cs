using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Graphics;

namespace TreeGenerator.Editor
{
    public class EditorTreeRenderData
    {
        public List<EditorTreeRenderDataPart> TreeRenderDataParts= new List<EditorTreeRenderDataPart>();

        public List<string> Texture=new List<string>();
        public List<string> BumpTexture=new List<string>();


        public void Initialize(IXNAGame game)
        {
           
			 for (int j = 0; j < TreeRenderDataParts.Count; j++)
                       {
                            if (BumpTexture[j] == "null")
                            {
                                TreeRenderDataParts[j].Initialize(game, Texture[j]);
                            }
                            else
                            {
                                TreeRenderDataParts[j].InitializeBump(game, Texture[j], BumpTexture[j]);
                            }
                            TreeRenderDataParts[j].SetWorldMatrix(Matrix.Identity);
                        }
	    }

      

        public void render()
        {
            for (int i = 0; i < TreeRenderDataParts.Count; i++)
            {
                TreeRenderDataParts[i].Render();
               
            }
        }
    }
}
