using System.Collections.Generic;
using MHGameWork.TheWizards.Graphics;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering.Vegetation.Trees
{
    public class EngineTreeRenderData
    {
      //  private bool initialized = false;
      //  public EngineTreeRenderDataPart TreeBody;
      //  public List<EngineTreeRenderDataPart> Leaves;
      //  private List<bool> leaves;
      //  public Vector3 Position;
      //  IXNAGame game;
      //  public Vector3[] BoundingBoxData = new Vector3[8];// I shall later have to change this probably don't know if it's really necesary

      //  public bool RenderDataGone = false;
      //  public EngineTreeRenderData(IXNAGame _game)
      //  {
      //      game = _game;
      //      Leaves = new List<EngineTreeRenderDataPart>();
      //      leaves = new List<bool>();
      //  }


      //  public void Initialize()
      //  {
      //      if (initialized) return;
      //      initialized = true;
      //      TreeBody.Initialize(game);
      //      for (int i = 0; i < Leaves.Count; i++)
      //      {
      //          Leaves[i].Initialize(game);
      //      }
      //      if (Leaves.Count>0)
      //      {
      //          mergeBoundingBoxses();
      //      }
      //      else
      //      {
      //          //BoundingBoxData = TreeBody.BoundingBox;
      //      }

      //  }

      ///*  public  void draw(Matrix viewProjection,int lodIndex)
      //  {
      //      game.GraphicsDevice .RenderState.CullMode = CullMode.None;
      //      game.GraphicsDevice.RenderState.AlphaTestEnable = true;
      //      game.GraphicsDevice.RenderState.ReferenceAlpha = 200;
      //      game.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.GreaterEqual;
      //      game.GraphicsDevice.RenderState.AlphaBlendEnable = false;
      //      game.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            


          
            


      //      int index=0;
      //      if (lodIndex>TreeBody.Count)
      //      {
      //          index =TreeBody.Count-1;
      //      }
      //      TreeBody[index].RenderTree(viewProjection);
      //      if (leaves[index])
      //      {
      //          //Leaves[index].RenderTree();
      //          Leaves[index].RenderTree(viewProjection);
      //      }


      //      game.GraphicsDevice.RenderState.CullMode = CullMode.None;
      //      game.GraphicsDevice.RenderState.AlphaTestEnable = true;
      //      game.GraphicsDevice.RenderState.ReferenceAlpha = 200;
      //      game.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Less;
      //      game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
      //      game.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;

      //      index = 0;
      //      if (lodIndex > TreeBody.Count)
      //      {
      //          index = TreeBody.Count - 1;
      //      }
      //      TreeBody[index].RenderTree(viewProjection);
      //      if (leaves[index])
      //      {
      //          //Leaves[index].RenderTree();
      //          Leaves[index].RenderTree(viewProjection);
      //      }

      //  }*/

      //  public void draw()
      //  {
            
      //      TreeBody.RenderTree(game.Camera.ViewProjection,game.Camera.ViewInverse);
      //      for (int i = 0; i < Leaves.Count; i++)
      //      {

      //          Leaves[i].RenderTree(game.Camera.ViewProjection, game.Camera.ViewInverse);

      //      }
           
      //  }
      //  public void draw(Matrix viewProjection,Matrix viewInverse)
      //  {
      //      TreeBody.RenderTree(viewProjection,viewInverse);
      //      for (int i = 0; i < Leaves.Count; i++)
      //      {

      //          Leaves[i].RenderTree(viewProjection,viewInverse);

      //      }
      //  }

      //  public void RenderPrimitives()
      //  {
      //      TreeBody.RenderPrimitives();
      //  }
      //  public void RenderLinearDepth()
      // {
      //     TreeBody.RenderLinearDepth(game.Camera);
      //     //for (int i = 0; i < Leaves.Count; i++)
      //     //{
      //     //    Leaves[i].RenderLinearDepth(game.Camera.ViewProjection, game.Camera.View, game.Camera.FarClip, game.Camera.Projection);
      //     //}
      // }
      //  private void mergeBoundingBoxses()
      //  {

      //      Vector3 min = TreeBody.BoundingBox[0];
      //      Vector3 max = TreeBody.BoundingBox[0];
      //      for (int i = 0; i < TreeBody.BoundingBox.Length; i++)
      //      {
      //          Vector3 pos = TreeBody.BoundingBox[i];
      //          Vector3.Min(ref min, ref pos, out min);
      //          Vector3.Max(ref max, ref pos, out max);

      //      }
      //      for (int i = 0; i < Leaves[0].BoundingBox.Length; i++)
      //      {
      //          Vector3 pos = Leaves[0].BoundingBox[i];
      //          Vector3.Min(ref min, ref pos, out min);
      //          Vector3.Max(ref max, ref pos, out max);

      //      }
      //      BoundingBoxData[0] = new Vector3(min.X, min.Y, min.Z);
      //      BoundingBoxData[1] = new Vector3(max.X, min.Y, min.Z);
      //      BoundingBoxData[2] = new Vector3(max.X, min.Y, max.Z);
      //      BoundingBoxData[3] = new Vector3(min.X, min.Y, max.Z);

      //      BoundingBoxData[4] = new Vector3(min.X, max.Y, max.Z);
      //      BoundingBoxData[5] = new Vector3(max.X, max.Y, max.Z);
      //      BoundingBoxData[6] = new Vector3(max.X, max.Y, min.Z);
      //      BoundingBoxData[7] = new Vector3(min.X, max.Y, min.Z);



      //  }

      //  //public void DiscartRenderData()
      //  //{
      //  //    TreeBody.decl = null;
      //  //    TreeBody.Vertices1 = null;
      //  //    RenderDataGone = true;
      //  //}
      //  //public void LoadRenderData(EngineTreeType Tree, IXNAGame game)
      //  //{
      //  //    EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(20);
      //  //    gen.GetRenderData(Tree, game);
      //  //    TreeBody = gen.TreeRenderData.TreeBody;
      //  //    Leaves = gen.TreeRenderData.Leaves;
      //  //    TreeBody.Initialize(game);
      //  //    Leaves.Initialize(game);
      //  //    RenderDataGone = false;
      //  //}


      //  // new design
      //  public void Transform(Matrix world)
      //  {
           
      //          TreeBody.SetWorldMatrix(world);

      //          for (int i = 0; i < Leaves.Count; i++)
      //          {
      //              Leaves[i].SetWorldMatrix(world);
      //          }
 
      //      }
            

        
      //  private void transform(Matrix tree)
      //  {
      //      //for (int i = 0; i < TreeBody.Count; i++)
      //      //{
      //      //    TreeBody[i].SetWorldMatrix(tree);
      //      //    if (leaves[i])
      //      //    {
      //      //        Leaves[i].SetWorldMatrix(tree);
      //      //    }
      //      //}


      //  }
      //  public void RenderNew(Matrix viewProjection,int index)
      //  {
      //      //int lodindex = 0;
      //      //if (lodIndex > TreeBody.Count)
      //      //{
      //      //    index = TreeBody.Count - 1;
      //      //}
      //      //transform(WorldMatrices[index]);
      //      ////TreeBody.RenderTree();
      //      //TreeBody[lodIndex].RenderTree(viewProjection);
      //      //if (leaves[lodindex])
      //      //{
      //      //    Leaves[lodindex].RenderTree();
      //      //    Leaves[lodindex].RenderTree(viewProjection);
      //      //}
      //  }

      //  public Vector3[] TransFormBoundingBox(int index)
      //  {
      //       Vector3[] box = new Vector3[8];
      //  //    Matrix mat = WorldMatrices[index];
      //  //     Vector3.Transform(boundingBox, ref mat , box);
      //  return box;
      //  }


      //  public static void TestEngineRenderdata()
      //  {
      //      XNAGame game;
      //      game = new XNAGame();

           
      //      EngineTreeRenderDataGenerater gen = new EngineTreeRenderDataGenerater(20);
      //      EngineTreeRenderData treeRenderData = new EngineTreeRenderData(game);
      //      TreeGenerator.TreeStructure treeStruct = TreeGenerator.TreeStructure.GetTestTreeStructure(game);
            
            
          
      //      game.InitializeEvent +=
      //         delegate
      //             {
                     
                      
      //                 treeRenderData = gen.GetRenderData(treeStruct, game,0);
      //                 treeRenderData.Initialize();
      //                 //treeRenderData.TreeBody.SetWorldMatrix(Matrix.CreateScale(20));
      //             };
           
      //      game.DrawEvent +=
      //          delegate
      //          {


      //              treeRenderData.draw();
      //          };
      //      game.Run();
      //  }

      //  public static void TestForest()
      //  {
      //      XNAGame game;
      //      game = new XNAGame();
      //      //game.DrawFps = true;

      //      TreeGenerator.TreeTypeData treeTypeData;
      //      TreeGenerator.TreeStructure treeStruct;
      //      TreeGenerator.TreeStructureGenerater genStruct = new TreeGenerator.TreeStructureGenerater();
      //      EngineTreeRenderData renderData = new EngineTreeRenderData(game);
      //      EngineTreeRenderDataGenerater genData = new EngineTreeRenderDataGenerater(10);
      //      TreeGenerator.Seeder seeder = new TreeGenerator.Seeder(47856);
      //      List<Matrix> Matrices = new List<Matrix>();
      //      game.InitializeEvent +=
      //         delegate
      //             {
      //                 treeTypeData = TreeGenerator.TreeTypeData.GetTestTreeType();
      //                 treeStruct=genStruct.GenerateTree(treeTypeData, 123);
      //                 genData.GetRenderData(treeStruct, game, 0);

      //                 renderData = genData.TreeRenderData;
      //                 renderData.Initialize();

      //                 for (int i = 0; i < 1000; i++)
      //                 {
      //                     Matrix mat=new Matrix();
      //                     mat = Matrix.CreateScale(seeder.NextFloat(1, 1.4f));
      //                     mat *= Matrix.CreateRotationY(seeder.NextFloat(0, MathHelper.TwoPi));
      //                     mat *=Matrix.CreateTranslation(seeder.NextVector3(new Vector3(300, 0, 300),new Vector3(0, 0, 0)));
      //                     Matrices.Add(mat);
      //                 }
      //             };

      //      game.DrawEvent +=
      //          delegate
      //              {
      //                  for (int i = 0; i < 1000; i++)
      //                  {
      //                      renderData.Transform(Matrices[i]);
      //                  renderData.draw();
      //                  }
                       

      //              };
      //      game.Run();
      //  }

      //  #region IRenderable Members

      //  public void Render(IXNAGame game)
      //  {
      //      draw();
      //  }

      //  public void SetWorldMatrix(Matrix world)
      //  {
      //      Transform(world);
      //  }

      //  #endregion
    }
}

