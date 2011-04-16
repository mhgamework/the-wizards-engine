using MHGameWork.TheWizards.Editor;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Editor
{
    [TestFixture]
    public class EditorTest
    {
        [Test]
        public void TestWizardsEditor()
        {
            var wizardsEditor = new WizardsEditor();
            wizardsEditor.RunEditor();
        }
    }
}
