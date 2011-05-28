using System;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.ServerClient.Terrain;
using MHGameWork.TheWizards.Terrain;
using MHGameWork.TheWizards.Terrain.Editor;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Terrain
{
    /// <summary>
    /// TODO: fix
    /// </summary>
    [TestFixture]
    public class TerrainEditorTest
    {
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestTerrainEditor()
        {
            throw new NotImplementedException();
            WizardsEditor editor = new WizardsEditor();

            editor.OpenWorldEditor();
            TerrainWorldEditorGUI gui = new TerrainWorldEditorGUI(editor.WorldEditor);

            editor.WorldEditor.AddTool(new TerrainCreateTool(editor.WorldEditor, gui.Form));
            editor.WorldEditor.AddTool(new TerrainRaiseTool(editor.WorldEditor, gui.Form));


            editor.RunEditor();
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestLoadTerrainEditorGUI()
        {
            throw new NotImplementedException();
            WizardsEditor editor = new WizardsEditor();

            editor.OpenWorldEditor();
            TerrainWorldEditorGUI gui = new TerrainWorldEditorGUI(editor.WorldEditor);



            editor.RunEditor();
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestTerrainCreateTool()
        {
            throw new NotImplementedException();
            WizardsEditor editor = new WizardsEditor();

            editor.OpenWorldEditor();
            TerrainWorldEditorGUI gui = new TerrainWorldEditorGUI(editor.WorldEditor);
            editor.WorldEditor.AddTool(new TerrainCreateTool(editor.WorldEditor, gui.Form));


            editor.RunEditor();


        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestTerrainRaiseTool()
        {
            throw new NotImplementedException();
            WizardsEditor editor = new WizardsEditor();

            editor.OpenWorldEditor();
            TerrainWorldEditorGUI gui = new TerrainWorldEditorGUI(editor.WorldEditor);
            editor.WorldEditor.AddTool(new TerrainRaiseTool(editor.WorldEditor, gui.Form));

            EditorTerrain t = editor.CreateTerrain();
            t.FullData.Position = Vector3.Zero;
            t.FullData.SizeX = 256;
            t.FullData.SizeZ = 256;
            t.FullData.BlockSize = 16;
            t.FullData.NumBlocksX = t.FullData.SizeX / t.FullData.BlockSize;
            t.FullData.NumBlocksZ = t.FullData.SizeZ / t.FullData.BlockSize;
            t.FullData.HeightMap = new HeightMap(t.FullData.SizeX + 1, t.FullData.SizeZ + 1);

            editor.RunEditor();


        }
    }
}
