using MHGameWork.TheWizards.VSIntegration;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Various
{
    [TestFixture]
    public class TestVSIntegration
    {
        [Test]
        public void TestAttach()
        {
            var attacher = new VSDebugAttacher();

            attacher.AttachToVisualStudio();

        }

        [Test]
        public void TestOpenFile()
        {
            var attacher = new VSDebugAttacher();
            attacher.GotoLine(@"C:\_MHData\1 - Projecten\The Wizards\_Source\Gameplay\BugTests.cs",12);


        }

    }


}
