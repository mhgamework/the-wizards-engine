using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Versioning;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Versioning
{
    [TestFixture]
    public class VersioningTest
    {
        [Test]
        public void TestWriteWorkingCopy()
        {
            var sys = new VersioningSystem(TWDir.Test.CreateSubdirectory("Versioning\\TestWorking"));
            using (var wr = new StreamWriter(sys.OpenWrite("TestFile.txt")))
            {
                wr.WriteLine("Test Line!");

            }
        }
        [Test]
        public void TestCommit()
        {
            var sys = new VersioningSystem(TWDir.Test.CreateSubdirectory("Versioning\\TestCommit"));
            using (var wr = new StreamWriter(sys.OpenWrite("TestFile.txt")))
            {
                wr.WriteLine("Test Line!");

            }

            sys.Commit("First commit");

            using (var wr = new StreamWriter(sys.OpenWrite("TestFile.txt")))
            {
                wr.WriteLine("Test Line! Changed!!");

            }
            sys.Commit("Change the one file that exists!");
        }
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCommitEmpty()
        {
            var sys = new VersioningSystem(TWDir.Test.CreateSubdirectory("Versioning\\TestCommitEmpty"));
            sys.Commit("Should Crash!");
        }
        [Test]
        public void TestCommitRedundant()
        {
            var sys = new VersioningSystem(TWDir.Test.CreateSubdirectory("Versioning\\TestCommitRedundant"));
            using (var wr = new StreamWriter(sys.OpenWrite("TestFile.txt")))
            {
                wr.WriteLine("Test Line!");

            }

            sys.Commit("First commit");

            using (var wr = new StreamWriter(sys.OpenWrite("TestFile.txt")))
            {
                wr.WriteLine("Test Line!");

            }
            using (var wr = new StreamWriter(sys.OpenWrite("TestFile2.txt")))
            {
                wr.WriteLine("Test Line2!");

            }
            sys.Commit("Test!");
        }
        [Test]
        public void TestRead()
        {
            var sys = new VersioningSystem(TWDir.Test.CreateSubdirectory("Versioning\\TestRead"));
            using (var wr = new StreamWriter(sys.OpenWrite("TestFile.txt")))
            {
                wr.WriteLine("Test Line!");

            }

            sys.Commit("First commit");

            using (var wr = new StreamWriter(sys.OpenWrite("TestFile.txt")))
            {
                wr.WriteLine("Test Line!");

            }
            using (var wr = new StreamWriter(sys.OpenWrite("TestFile2.txt")))
            {
                wr.WriteLine("Test Line2!");

            }
            sys.Commit("Test!");


            using (var s = new StreamReader(sys.OpenFileRead("TestFile2.txt")))
            {
                Assert.AreEqual(s.ReadLine(), "Test Line2!");
            }

            using (var s = new StreamReader(sys.OpenFileRead("TestFile.txt")))
            {
                Assert.AreEqual(s.ReadLine(), "Test Line!");
            }
        }

    }
}
