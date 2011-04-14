using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TreeGenerator.help;
using TreeGenerator.TreeEngine;
using TreeGenerator.Editor;

namespace TreeGenerator
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestGenerator()
        {
            TreestructureGenerator.TestGenerator();
        }
           [Test]
        public void TestTreeStructureOld()
        {
        Tree.TestTreeStructure();
        }
         [Test]
        public void TestRenderTree()
        {
        Tree.TestRenderTree();
         }
         [Test]
         public void Testleaves()
         {
             Leaf.Testleaves();
         }
           [Test]
         public void TestEditorOld()
         {
        Editor.TreeTypeEditor.TestEditor();
           }
   [Test]
           public void TestLeaf()
         {
       TreeLeafType.TestLeaf();
   }
   
   //public void TestGenerater()
   //      {
   //     TreeStructureGenerater.TestGenerater();
   //}
   //[Test]
   //public void TestDirectionsFromAngles()
   //      {
   //     Directions.TestDirectionsFromAngles();
   //}
   //[Test]
   //public void TestRenderLines()
   //      {
   //     MHGameWork.TheWizards.ServerClient.LineManager3D.TestRenderLines();
   //}
   //             [Test]
   //public void TestCreateVertices()
   //      {
   //     EditorTreeRenderDataGenerater.TestCreateVertices();
   //             }
   //             [Test]
   //             public void TestCreateAllVertices()
   //      {
   //     EditorTreeRenderDataGenerater.TestCreateAllVertices();
   //             }
   //[Test]
   //             public void TestCreateForest()
   //      {
   //     EditorTreeRenderDataGenerater.TestCreateForest();// using the imposters
   //}
   [Test]
   public void TestEngineRenderdataOld()
         {
        EngineTreeRenderDataImp.TestEngineRenderdata();
   }
  
   
        //TreeEngine.TreeEngine.TestEngine();
        //TreeEngine.TreeEngine.TestEnginePlusGrass();
        //Imposter.ImposterEngine.TestImposterEngine();
        //TreeEngine.EngineTreeRenderDataGenerater.TestEngineRenderdataGenerater();
        //TreeStructureGenerater.TestStructureGenerater();


        //////////////cleanup of treegenenrator tests//////////////////
        [Test]
   public void TestTreeStructure()
   {
       TreeStructure.TestTreeStructure();
   }
        [Test]
        public void TestEngineRenderdata()
        {
            EngineTreeRenderData.TestEngineRenderdata();
        }
        [Test]
        public void TestEditor()
        {
            TreeTypeEditor.TestEditor();
        }
        [Test]
        public void TestPointSpriteLeaves()
        {
            EngineTreeRenderDataPart.TestPointSpriteLeaves();
        }
        [Test]
        public void TestForest()
        {
            EngineTreeRenderData.TestForest();
        }
    }
}
