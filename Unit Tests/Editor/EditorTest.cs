using MHGameWork.TheWizards.Editor;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Editor
{
    public class EditorTest
    {
        [Test]
        public void TestWizardsEditor()
        {
            WizardsEditor wizardsEditor = new WizardsEditor();
            wizardsEditor.RunEditor();
        }
    }
}
