using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FtpLib;
using MHGameWork.TheWizards;

namespace DocumentationHelper
{
    /// <summary>
    /// Collects namespace documentation and uploads it to wiki.thewizards.be!!
    /// </summary>
    public class DocUploader
    {

        public void UploadDocs()
        {
            using (var ftp = new FtpConnection("ftp.thewizards.be", "thewizards.be", "eroU0Ioti"))
            {

                ftp.Open(); /* Open the FTP connection */
                ftp.Login(); /* Login using previously provided credentials */

                var docFiles = getDocFilesEnumerable("Gameplay")
                    .Union(getDocFilesEnumerable("NewModules"))
                    .Union(getDocFilesEnumerable("ApplicationLogic"))
                    .Union(getDocFilesEnumerable("DirectX11"))
                    .Union(getDocFilesEnumerable("Common.Core"))
                    .Union(getDocFilesEnumerable("Networking"))
                    .Union(getDocFilesEnumerable("DocumentationHelper"))
                    ;

                try
                {
                    string uploadRoot = "/wiki/data/pages/source";

                    foreach (var doc in docFiles)
                    {
                        var dir = FileHelper.GetDirectory(doc.DocPath);
                        string targetDir = uploadRoot + "/" + dir.Replace('\\', '/');
                        targetDir = targetDir.ToLower();
                        if (!ftp.DirectoryExists(targetDir))
                            ftp.CreateDirectory(targetDir);
                        ftp.SetCurrentDirectory(targetDir); /* change current directory */

                        string tempStore = "build";


                        ftp.PutFile(buildDocFile(doc), FileHelper.ExtractFilename(doc.DocPath.ToLower(), false)); /* upload c:\localfile.txt to the current ftp directory as file.txt */
                        Console.WriteLine(doc);
                    }
                }
                catch (FtpException e)
                {
                    Console.WriteLine(String.Format("FTP Error: {0} {1}", e.ErrorCode, e.Message));
                }

            }
        }

        private IEnumerable<Docfile> getDocFilesEnumerable(string projectName)
        {
            string prefix = "../../../" + projectName;
            foreach (var file in Directory.EnumerateFiles(prefix, "_doc.txt", SearchOption.AllDirectories))
            {
                var docfile = new Docfile();
                docfile.DocPath = createDocPathFromRelative(projectName + "\\" + file.Substring(prefix.Length + 1));



                var originalPath = new FileInfo(file).FullName;
                docfile.Localpath = originalPath;



                yield return docfile;
            }
        }

        private string buildDocFile(Docfile doc)
        {
            var text = "";
            text += "====== " + FileHelper.ExtractFilename(doc.DocPath, true) + " ======\n";
            text += File.ReadAllText(doc.Localpath);

            string buildFile = "build\\" + doc.DocPath;
            Directory.CreateDirectory(FileHelper.GetDirectory(buildFile));
            File.WriteAllText(buildFile, text);

            return buildFile;
        }

        private string createDocPathFromRelative(string relative)
        {
            var ret = relative;
            ret = ret.Substring(0, ret.Length - "\\_doc.txt".Length);
            ret = ret + ".txt";
            return ret;
        }

        private struct Docfile
        {
            public string DocPath { get; set; }
            public string Localpath { get; set; }

            public override string ToString()
            {
                return string.Format("DocPath: {0}, Localpath: {1}", DocPath, Localpath);
            }
        }
    }
}
