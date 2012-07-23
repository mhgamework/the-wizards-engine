using System;
using Microsoft.Xna.Framework;
using TreeGenerator.Clouds;
using TreeGenerator.help;
using Microsoft.Xna.Framework.Graphics;
using TreeGenerator.Editor;
using TreeGenerator.TreeEngine;
using TreeGenerator.AtlasTool;
using MHGameWork.TheWizards.Utilities;
using System.Reflection;

namespace TreeGenerator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread()]
        static void Main(string[] args)
        {
            /*TestRunnerGUI runnerGui = new TestRunnerGUI();
            runnerGui.TestsAssembly = Assembly.GetExecutingAssembly();
            runnerGui.Run();*/

            //TreestructureGenerator.TestGenerator();

            //Tree.TestTreeStructure();
            //Tree.TestRenderTree();
            //Leaf.Testleaves();
           
            //Editor.TreeTypeEditor.TestEditor();
            //TreeLeafType.TestLeaf();
            //TreeStructureGenerater.TestGenerater();
            //Directions.TestDirectionsFromAngles();

            //MHGameWork.TheWizards.ServerClient.LineManager3D.TestRenderLines();
            //EditorTreeRenderDataGenerater.TestCreateVertices();
            //EditorTreeRenderDataGenerater.TestCreateAllVertices();

            //EditorTreeRenderDataGenerater.TestCreateForest();// using the imposters
           
            //EngineTreeRenderDataImp.TestEngineRenderdata();

            //EngineTreeRenderData.TestEngineRenderdata();
            //TreeEngine.TreeEngine.TestEngine();
            //TreeEngine.TreeEngine.TestEnginePlusGrass();
            //Imposter.ImposterEngine.TestImposterEngine();
            //TreeEngine.EngineTreeRenderDataGenerater.TestEngineRenderdataGenerater();
            //TreeStructureGenerater.TestStructureGenerater();


            //////////////cleanup of treegenenrator tests//////////////////
            //TreeStructure.TestTreeStructure();
           // EngineTreeRenderData.TestEngineRenderdata();
            //TreeTypeEditor.TestEditor();
            //EngineTreeRenderDataPart.TestPointSpriteLeaves();
            //EngineTreeRenderData.TestForest();


            
            /////////////////////////////////Clouds/////////////////////////////////
         //Clouds.BaseCloudElement.TestCreateVertices();


            

            ///////////////////////////////////objhImporter/////////////////////////////////
           
          


            //ImposterRing.ImposterRing.TestImposterRing();
            //ImposterRing.ImposterRing.UberDuberTestImposterRing();


           // ObjExporter.TestEditorMeshPartRenderDataSimple();

            //TestVolumetricLeafShader.TestLeaf();

            ////////////to get the billboard shader working/////////////////////
            //BillBoards.TestCreateVertices();





            
            return;
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
}

