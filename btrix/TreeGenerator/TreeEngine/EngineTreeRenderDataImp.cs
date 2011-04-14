using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Common.Core.Collada;
using MHGameWork.TheWizards.Common.Core.Graphics;
using TreeGenerator.Imposter;
using MHGameWork.TheWizards.Graphics;
namespace TreeGenerator.TreeEngine
{
    public class EngineTreeRenderDataImp
    {
        public EngineTreeRenderDataPart TreeBody;
        public EngineTreeRenderDataPart Leaves;
        private bool leaves;
        public Vector3 Position;
        public Imposter.Imposter TreeImposter;
        IXNAGame game;
        public Vector3[] boundingBox = new Vector3[8];

        public bool RenderDataGone = false;
        public EngineTreeRenderDataImp(IXNAGame _game)
        {
            game = _game;
            //Position = tree.Position;
            TreeImposter = new TreeGenerator.Imposter.Imposter();
        }
        public void Initialize()
        {
            TreeBody.Initialize(game);
            if (Leaves.Vertices1.Count<1)
            {
                leaves = false;
            }
            if (leaves)
            {
                Leaves.Initialize(game);
                mergeBoundingBoxses();
            }
            else
            {
                boundingBox = TreeBody.BoundingBox;
            }
           

            TreeImposter.MinDistance =25000;
            TreeImposter.MaxAngleDifference = 0.5f;
            //TreeImposter.Intialize(500,500,game,draw,Position,boundingBox);
           
            //TreeImposter.MaxTime = 10000000;
        }
        public void update()
        {
            if (RenderDataGone != true)
            {
                TreeImposter.Update();
            }
        }
        private void draw(Matrix viewProjection,Matrix viewInverse)
        {
            //TreeBody.RenderTree();
            TreeBody.RenderTree(viewProjection, viewInverse);
            if (leaves)
            {
                Leaves.RenderTree();
                Leaves.RenderTree(viewProjection, viewInverse);
            }
        }
        public void ImposterDraw()
        {
            TreeImposter.Render();
        }
        private void mergeBoundingBoxses()
        {
       
            Vector3 min = TreeBody.BoundingBox[0];
            Vector3 max =TreeBody.BoundingBox[0];
            for (int i = 0; i < TreeBody.BoundingBox.Length; i++)
			{
                Vector3 pos = TreeBody.BoundingBox[i];
                Vector3.Min(ref min, ref pos, out min);
                Vector3.Max(ref max, ref pos, out max);
			 
			}
            for (int i = 0; i < Leaves.BoundingBox.Length; i++)
			{
                Vector3 pos = Leaves.BoundingBox[i];
                Vector3.Min(ref min, ref pos, out min);
                Vector3.Max(ref max, ref pos, out max);
			 
			}
            boundingBox[0] = new Vector3(min.X, min.Y, min.Z);
            boundingBox[1] = new Vector3(max.X, min.Y, min.Z);
            boundingBox[2] = new Vector3(max.X, min.Y, max.Z);
            boundingBox[3] = new Vector3(min.X, min.Y, max.Z);

            boundingBox[4] = new Vector3(min.X, max.Y, max.Z);
            boundingBox[5] = new Vector3(max.X, max.Y, max.Z);
            boundingBox[6] = new Vector3(max.X, max.Y, min.Z);
            boundingBox[7] = new Vector3(min.X, max.Y, min.Z);

        

        }

        public void DiscartRenderData()
        {
            TreeBody.decl = null;
            TreeBody.Vertices1 = null;
            RenderDataGone = true;
        }
        //public void LoadRenderData(EngineTreeType Tree,IXNAGame game)
        //{
        //    EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(20);
        //    gen.GetRenderData(Tree, game);
        //    TreeBody = gen.TreeRenderData.TreeBody;
        //    Leaves = gen.TreeRenderData.Leaves;
        //    TreeBody.Initialize(game);
        //    Leaves.Initialize(game);
        //    RenderDataGone = false;
        //}

        public static void TestEngineRenderdata()
        {
             XNAGame game;
            game = new XNAGame();
            
            EngineTreeType tree = new EngineTreeType();
            EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(20);
            EngineTreeRenderDataImp treeRenderData = new EngineTreeRenderDataImp(game);
            //treeRenderData = gen.GetRenderData(tree, game);
             game.InitializeEvent +=
                delegate
                {
                
                    treeRenderData.Initialize();

                };
            game.UpdateEvent+=
                delegate{
                    treeRenderData.update();
                };
            game.DrawEvent+=
                delegate
                {
                   
                    //treeRenderData.draw(game.Camera.ViewProjection);
                    treeRenderData.ImposterDraw();
                };
            game.Run();
        }
    }
}

 