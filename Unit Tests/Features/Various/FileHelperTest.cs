using System.IO;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Various
{
    [TestFixture]
    public class FileHelperTest
    {
        private DirectoryInfo rootDir;
        private DirectoryInfo subDir;
        private FileInfo rootFile1;
        private FileInfo subFile1;

        [SetUp]
        public void SetUp()
        {
            rootDir = TWDir.Test.CreateSubdirectory("FileHelperTest");
            subDir = rootDir.CreateSubdirectory("Sub1");
            rootFile1 =new FileInfo( rootDir + "\\file1.txt");
            subFile1 = new FileInfo(subDir + "\\subfile1.txt");

            File.WriteAllText(rootFile1.FullName, "Hello!");
            File.WriteAllText(subFile1.FullName, "Hello!");
        }

        [Test]
        public void TestIsFileInDirectory()
        {
            Assert.True(FileHelper.IsFileInDirectory(rootFile1, rootDir));
            Assert.False(FileHelper.IsFileInDirectory(rootFile1, subDir));

            Assert.True(FileHelper.IsFileInDirectory(subFile1, rootDir));
            Assert.True(FileHelper.IsFileInDirectory(subFile1, subDir));
        }

        [Test]
        public void TestIsChildFolder()
        {
            Assert.True(FileHelper.IsChildFolder(subDir, rootDir));
            Assert.False(FileHelper.IsChildFolder(rootDir, subDir));

            Assert.False(FileHelper.IsChildFolder(rootDir, rootDir));
            Assert.False(FileHelper.IsChildFolder(subDir, subDir));

        }
    }
}
