using MHGameWork.TheWizards.Editor.Scene;
using MHGameWork.TheWizards.Graphics;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Editor.Scene
{
    [TestFixture]
    public class SceneEditorTest
    {
        [Test]
        public void TestSceneEditor()
        {
            SceneEditor editor = new SceneEditor(new EditorScene());

            RunSceneEditorTestEnvironment(editor);

        }

        public static void RunSceneEditorTestEnvironment(SceneEditor editor)
        {
            XNAGame game = new XNAGame();
            game.AddXNAObject(editor);
            game.IsMouseVisible = true;

            game.InitializeEvent += delegate
            {
                // cheat: double call
                editor.Initialize(game);
                editor.ActivateEditorCamera();
            };

            game.Run();
        }
    }
}
