using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using DevExpress.Utils.Zip;
using ICSharpCode.SharpZipLib.Zip;

namespace MHGameWork.TheWizards.Versioning
{
    public class VersioningSystem : IStreamProvider
    {
        private SHA1 sha;
        public DirectoryInfo RootDirectory { get; private set; }
        public Revision CheckoutRevision { get; private set; }

        public VersioningSystem(DirectoryInfo rootDirectory)
        {
            RootDirectory = rootDirectory;
            Directory.CreateDirectory(getWorkingDir());
            sha = SHA1.Create();
        }

        public void SwitchTo(Revision rev)
        {
            CheckoutRevision = rev;
        }
        /// <summary>
        /// Commits the working copy to a new revision and switches to that revision
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Revision Commit(string msg)
        {
            // This line also cleans the working copy, which is mandatory
            if (IsWorkingCopyEmpty()) throw new InvalidOperationException("The working copy is empty!!");

            var newRev = new Revision();
            newRev.Guid = Guid.NewGuid();
            newRev.Parent = CheckoutRevision;
            newRev.Message = msg;

            Directory.Move(getWorkingDir(), getRevisionRoot(newRev));
            Directory.CreateDirectory(getWorkingDir());

            SwitchTo(newRev);

            return newRev;
        }

        /// <summary>
        /// This cleans up the working copy and counts the number of files.
        /// Cleans up means removes the unchanged files
        /// </summary>
        /// <returns></returns>
        public bool IsWorkingCopyEmpty()
        {
            CleanUpWorkingCopy();
            return getWorkingCopyFilesCount() == 0;

        }

        /// <summary>
        /// Removes all unchanged files from the working copy (these are redundant files)
        /// </summary>
        public void CleanUpWorkingCopy()
        {
            var di = new DirectoryInfo(getWorkingDir());
            var e = di.EnumerateFiles("*", SearchOption.AllDirectories);
            foreach (var fi in e)
            {
                var path = fi.FullName.Replace(getWorkingDir() + "\\", "");
                path = path.Substring(0, path.Length - 4); // Remove .zip
                byte[] hash1;
                byte[] hash2;

                using (var strm = OpenFileRead(path))
                {
                    if (strm == null) continue;

                    hash1 = sha.ComputeHash(strm);
                }

                using (var s = new ZipInputStream(fi.Open(FileMode.Open)))
                {
                    s.GetNextEntry();

                    hash2 = sha.ComputeHash(s);
                }

                if (!hash1.SequenceEqual(hash2)) continue;

                fi.Delete();
            }


        }

        internal string getPathFromFileInfo(Revision rev, FileInfo fi)
        {
            var ret = fi.FullName.Replace(getRevisionRoot(rev), "");
            return ret.Substring(0, ret.Length - 4);
        }

        private int getWorkingCopyFilesCount()
        {
            var di = new DirectoryInfo(getWorkingDir());
            var e = di.EnumerateFiles("*", SearchOption.AllDirectories);
            return e.Count();


        }

        public Stream OpenFileRead(string path)
        {
            Revision rev = CheckoutRevision;

            Stream ret = null;
            while (ret == null && rev != null)
            {
                ret = tryOpenFileRead(rev, path);
                rev = rev.Parent;
            }

            //if (ret == null) throw new InvalidOperationException("File not found in revision");

            return ret;

        }
        private Stream tryOpenFileRead(Revision rev, string path)
        {

            var filePath = getFilePath(rev, path);
            if (!File.Exists(filePath)) return null;
            var fs = File.Open(filePath, FileMode.Open);
            var strm = new ZipInputStream(fs);
            var entry = strm.GetNextEntry();
            return strm;

        }

        internal string getFilePath(Revision rev, string path)
        {
            return getRevisionRoot(rev) + "\\" + path + ".zip";
        }

        public Stream OpenWrite(string path)
        {
            var filePath = getWorkingDir() + "\\" + path + ".zip";
            var fi = new FileInfo(filePath);
            fi.Directory.Create();
            var fs = File.Open(filePath, FileMode.Create);
            var strm = new ZipOutputStream(fs);
            strm.PutNextEntry(new ZipEntry(getZipEntryName(path)));
            return strm;
        }

        private string getZipEntryName(string path)
        {
            return (new FileInfo(path)).Name;
        }


        internal string getRevisionRoot(Revision rev)
        {
            return RootDirectory.FullName + "\\" + rev.Guid;
        }
        private string getWorkingDir()
        {
            return RootDirectory.FullName + "\\_Working";
        }


        public Revision FindRevision(Guid masterGuid)
        {
            throw new NotImplementedException();
        }
    }
}
