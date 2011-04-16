using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Graphics;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Editor
{
    [TestFixture]
    public class EditorTest
    {
        [Test]
        public void TestEditorGrid()
        {
            var game = new XNAGame();

            var grid = new EditorGrid();

            game.AddXNAObject(grid);

            game.Run();
        }
        [Test]
        public void TestEditorCamera()
        {
            var game = new XNAGame();

            var grid = new EditorGrid();
            game.AddXNAObject(grid);

            var cam = new EditorCamera();
            cam.Enabled = true;
            game.SetCamera(cam);

            game.AddXNAObject(cam);

            game.UpdateEvent += delegate
                                    {
                                        cam.UpdateCameraMoveModeDefaultControls();
                                    };


            game.Run();
        }

    }
}
