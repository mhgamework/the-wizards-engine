using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DocumentationHelper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var uploader = new DocUploader();
            uploader.UploadDocs();
        }
    }
}
