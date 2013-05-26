using System;
using System.Text.RegularExpressions;
using MHGameWork.TheWizards.VSIntegration;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Various
{
    [TestFixture]
    public class VSIntegrationTest
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

        [Test]
        public void TestFindSourceFile()
        {
            var attacher = new VSDebugAttacher();
            var file = attacher.FindSourceFile(typeof(VSIntegrationTest));
            Console.WriteLine(file);
        }

        [Test]
        public void TestSelectExceptionLine()
        {
            Exception exc;
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                exc = ex;
            }

            

            var attacher = new VSDebugAttacher();
            attacher.SelectExceptionLine(exc);
            

        }
    }


}
