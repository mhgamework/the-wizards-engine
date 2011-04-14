using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    /*public class TerrainDiskSerializer : IDiskSerializer
    {
     public string UniqueName { get { return "TerrainDiskSerializer001"; } }

        private const string outputFolder = "TerrainSerializer";

        public void SaveToDisk( DiskSerializerService service, TWXmlNode node )
        {

            int nextID = 1;

            TerrainManagerService manager = service.Database.FindService<TerrainManagerService>();

            for ( int i = 0; i < manager.Terrains.Count; i++ )
            {
                string filename = outputFolder + string.Format( "\\Terrain{0}.txt", nextID.ToString( "000" ) );
                DiskSerializerService.IFile file = service.OpenFile( this, filename );

                TWXmlNode terrNode = node.CreateChildNode( "Terrain" );
                terrNode.AddAttributeInt( "ID", nextID );
                terrNode.AddAttribute( "RelativeFilename", filename );

                file.RootNode.Clear();




                nextID++;


            }


            node.AddChildNode( "Testoutput", "succeeded" );



            DiskSerializerService.IFile testFile = service.OpenFile( this, outputFolder + "/Testfile.txt" );

            testFile.RootNode.Clear();




            testFile.SaveToDisk();
        }

        public void LoadFromDisk( DiskSerializerService service, TWXmlNode node )
        {

        }
       
    }*/
}
