using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.ServerClient.Database;
using System.IO;
using MHGameWork.TheWizards.ServerClient.Editor;

namespace MHGameWork.TheWizards.ServerClient.Database
{
    /// <summary>
    /// I think this class should load the services' states from disk
    /// This class is incorrect. 
    /// </summary>
    public class DiskLoaderService : IGameService
    {
        private TheWizards.Database.Database database;
        public TheWizards.Database.Database Database
        {
            get { return database; }
            //set { database = value; }
        }

        private List<IDiskSerializer> serializers;
        private DiskSerializerService diskSerializerService;



        /*public DirectoryInfo TargetDirectory
        {
            get { return targetDirectory; }
            //set { outputDirectory = value; }
        }*/

        public DiskLoaderService( TheWizards.Database.Database _database )
        {
            database = _database;
            serializers = new List<IDiskSerializer>();
            diskSerializerService = database.FindService<DiskSerializerService>();
        }

        public void AddDiskSerializer( IDiskSerializer item )
        {
            //Debug safety: check if unique name
            for ( int i = 0; i < serializers.Count; i++ )
            {
                IDiskSerializer serializer = serializers[ i ];
                if ( serializer.UniqueName == item.UniqueName ) throw new InvalidOperationException( "This serializer's name is not unique!!" );
            }
            serializers.Add( item );


        }

        public void SaveToDisk()
        {
            //if ( targetDirectory == null ) throw new InvalidOperationException( "Serialization directory not set!!" );
            IXMLFile file = diskSerializerService.OpenXMLFile( "DiskSerializerService.txt", "DiskLoaderService" );
            TWXmlNode rootnode = file.RootNode;
            rootnode.Clear();

            for ( int i = 0; i < serializers.Count; i++ )
            {
                TWXmlNode subNode = rootnode.CreateChildNode( "Serializer" );
                subNode.AddAttribute( "UniqueName", serializers[ i ].UniqueName );
                serializers[ i ].SaveToDisk( this, subNode );
            }

            file.SaveToDisk();

            //rootnode.Document.Save( targetDirectory.FullName + @"\DiskSerializerService.txt" );

        }

        public void LoadFromDisk()
        {
            //if ( targetDirectory == null ) throw new InvalidOperationException( "Serialization directory not set!!" );
            //string filename = targetDirectory.FullName + @"\DiskSerializerService.txt";
            //if ( File.Exists( filename ) == false ) return;
            IXMLFile file = diskSerializerService.OpenXMLFile( "DiskSerializerService.txt", "DiskLoaderService" );
            TWXmlNode rootnode = file.RootNode; // TWXmlNode.GetRootNodeFromFile( filename );

            TWXmlNode[] childNodes = rootnode.GetChildNodes();

            for ( int i = 0; i < childNodes.Length; i++ )
            {
                TWXmlNode node = childNodes[ i ];

                if ( node.Name != "Serializer" ) continue;
                string serializerName = node.GetAttribute( "UniqueName" );

                for ( int iS = 0; iS < serializers.Count; iS++ )
                {
                    // Find the correct serializer.
                    if ( serializers[ iS ].UniqueName == serializerName )
                    {
                        // This is the one
                        serializers[ iS ].LoadFromDisk( this, node );

                    }
                }
            }

        }
        /// <summary>
        /// DEPRECATED: use DiskSerializerService instead
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="relativeFilename"></param>
        /// <returns></returns>
        public IXMLFile OpenXMLFile( IDiskSerializer serializer, string relativeFilename )
        {
            return diskSerializerService.OpenXMLFile( relativeFilename, serializer.UniqueName ); 
        }
        /// <summary>
        /// DEPRECATED: use DiskSerializerService instead
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="relativeFilename"></param>
        /// <returns></returns>
        public IBinaryFile OpenBinaryFile( IDiskSerializer serializer, string relativeFilename )
        {
            return diskSerializerService.OpenBinaryFile( relativeFilename, serializer.UniqueName );
        }

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion

    }

    internal class XMLFile : IXMLFile
    {
        /// <summary>
        /// This root node can only be used by DiskSerializerService
        /// </summary>
        public TWXmlNode RealRootNode;
        public string FullFilename;
        private string relativeFilename;
        private TWXmlNode rootNode;

        public XMLFile()
        {
        }


        public TWXmlNode RootNode
        {
            get { return rootNode; }
            set { rootNode = value; }
        }


        public string RelativeFilename
        {
            get { return relativeFilename; }
            set { relativeFilename = value; }
        }

        public void SaveToDisk()
        {
            RealRootNode.Document.Save( FullFilename );
        }

    }

    /// <summary>
    /// This is a pattern i invented and would like to test. It ensures the user cant instantiate a File instance, but still allows him to use it.
    /// </summary>
    public interface IXMLFile
    {
        TWXmlNode RootNode { get;}
        string RelativeFilename { get;}

        void SaveToDisk();
    }

    /// <summary>
    /// This is a pattern i invented and would like to test. It ensures the user cant instantiate a File instance, but still allows him to use it.
    /// </summary>
    public interface IBinaryFile : IDisposable
    {
        ByteWriter Writer { get;}
        ByteReader Reader { get;}
        string RelativeFilename { get;}
        /// <summary>
        /// Keep in mind that this is the position started counting at the start of the header,
        /// not the start of the data!
        /// </summary>
        long Position { get;}

        /// <summary>
        /// This seeks in the whole file, also the header!
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        void Seek( long offset, SeekOrigin origin );

        void SaveAndClose();
    }

    /// <summary>
    /// Keep in mind that this file has a header!!
    /// </summary>
    internal class BinaryFile : IBinaryFile
    {
        /// <summary>
        /// This root node can only be used by DiskSerializerService
        /// </summary>
        public ByteWriter Writer;
        public ByteReader Reader;
        public System.IO.FileStream FileStream;
        public string FullFilename;
        public long HeaderLength;
        private string relativeFilename;

        public BinaryFile()
        {
        }

        public void CreateBinaryReaderWriter()
        {
            Writer = new ByteWriter( FileStream );
            Reader = new ByteReader( FileStream );
        }


        public string RelativeFilename
        {
            get { return relativeFilename; }
            set { relativeFilename = value; }
        }

        public void SaveAndClose()
        {
            FileStream.Close();
            FileStream = null;
            Writer = null;
            Reader = null;
        }



        public void Seek( long offset, SeekOrigin origin )
        {
            FileStream.Seek( offset, origin );
        }

        ByteWriter IBinaryFile.Writer
        {
            get { return Writer; }
        }

        ByteReader IBinaryFile.Reader
        {
            get { return Reader; }
        }

        public long Position
        {
            get { return FileStream.Position; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if ( FileStream == null ) return;
            SaveAndClose();
        }

        #endregion
    }
}