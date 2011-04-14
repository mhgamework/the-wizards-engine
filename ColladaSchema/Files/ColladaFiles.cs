using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Collada.Files
{
    public static class ColladaFiles
    {
        public static System.IO.Stream GetAdvancedScene001DAE()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "MHGameWork.TheWizards.Collada.Files.AdvancedScene001.DAE" );
        }

        public static System.IO.Stream GetPyramid001DAE()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "MHGameWork.TheWizards.Collada.Files.Pyramid001.DAE" );
        }

        public static System.IO.Stream GetSimplePlaneDAE()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "MHGameWork.TheWizards.Collada.Files.SimplePlane.DAE" );
        }

        public static System.IO.Stream GetTeapot001DAE()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "MHGameWork.TheWizards.Collada.Files.Teapot001.DAE" );
        }
    }
}
