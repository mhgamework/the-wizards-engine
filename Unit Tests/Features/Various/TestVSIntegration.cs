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
       
    }


}
