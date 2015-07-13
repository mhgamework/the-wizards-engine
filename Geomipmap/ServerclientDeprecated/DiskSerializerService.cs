using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MHGameWork.TheWizards.Graphics.Xna.XML;

namespace MHGameWork.TheWizards.ServerClient.Database
{
    public class DiskSerializerService : IGameService
    {
        private DirectoryInfo targetDirectory;

        public DirectoryInfo TargetDirectory
        {
            get { return targetDirectory; }
        }
        private TheWizards.Database.Database database;
        public TheWizards.Database.Database Database
        {
            get { return database; }
            //set { database = value; }
        }

        public DiskSerializerService( TheWizards.Database.Database _database, string serializationDir )
        {
            database = _database;
            targetDirectory = System.IO.Directory.CreateDirectory( serializationDir );

        }

        /// <summary>
        /// WARNING: the TWXmlNode this returns contains the data that is in the file when it exists!
        /// This file can only be reopend when called with the same fileCreaterName argument
        /// </summary>
        /// <param name="fileCreaterName"></param>
        /// <param name="relativeFilename"></param>
        /// <returns></returns>
        public IXMLFile OpenXMLFile( string relativeFilename, string fileCreaterName )
        {
            //if ( targetDirectory == null ) throw new InvalidOperationException( "Serialization directory not set!!" );
            XMLFile xmlFile = new XMLFile();

            string fullFilename = targetDirectory.FullName + "\\" + relativeFilename;
            TWXmlNode realRootNode;
            TWXmlNode publicRootNode; // The node for public use (by the services) to put data in
            TWXmlNode serializerNode;
            TWXmlNode fileInfoNode;

            if ( System.IO.File.Exists( fullFilename ) )
            {
                realRootNode = TWXmlNode.GetRootNodeFromFile( fullFilename );

                serializerNode = realRootNode.FindChildNode( "FileCreaterName" );
                if ( serializerNode == null ) throw new InvalidOperationException( "Invalid file type!!(no FileCreaterName tag)" );
                if ( serializerNode.Value.Equals( fileCreaterName ) == false )
                    throw new InvalidOperationException( "SECURITY BREACH: The file in this location is not created by the same creater." );

                fileInfoNode = realRootNode.FindChildNode( "FileInfo" );
                if ( fileInfoNode == null ) throw new InvalidOperationException( "Invalid file type!!(no FileInfo tag)" );
                //TODO: the following check for the paths to be equal can cause problems when using different slashes '\' or  '/'
                if ( fileInfoNode.GetAttribute( "RelativeFilename" ).Equals( relativeFilename ) == false )
                    throw new InvalidOperationException( "The file on the disk is in the wrong location!!" );

                // Valid file, get the publicrootnode
                publicRootNode = realRootNode.FindChildNode( "Data" );
                if ( publicRootNode == null ) throw new InvalidOperationException( "Invalid file type!!(no Data tag)" );

            }
            else
            {
                FileInfo fi = new FileInfo( fullFilename );
                System.IO.Directory.CreateDirectory( fi.DirectoryName );

                realRootNode = new TWXmlNode( TWXmlNode.CreateXmlDocument(), "DiskSerializerService.File" );
                serializerNode = realRootNode.CreateChildNode( "FileCreaterName" );
                serializerNode.Value = fileCreaterName;//( "UniqueName", serializer.UniqueName );

                fileInfoNode = realRootNode.CreateChildNode( "FileInfo" );
                fileInfoNode.AddAttribute( "RelativeFilename", relativeFilename );

                publicRootNode = realRootNode.CreateChildNode( "Data" );
            }

            xmlFile.FullFilename = fullFilename;
            xmlFile.RelativeFilename = relativeFilename;
            xmlFile.RootNode = publicRootNode;
            xmlFile.RealRootNode = realRootNode;


            return xmlFile;

        }

        /// <summary>
        /// WARNING: the TWXmlNode this returns contains the data that is in the file when it exists!
        /// This file can only be reopend when called with the same fileCreaterName argument
        /// </summary>
        /// <param name="fileCreaterName"></param>
        /// <param name="relativeFilename"></param>
        /// <returns></returns>
        public IBinaryFile OpenBinaryFile( string relativeFilename, string fileCreaterName )
        {
            if ( targetDirectory == null ) throw new InvalidOperationException( "Serialization directory not set!!" );
            BinaryFile file = new BinaryFile();
            string fullFilename = targetDirectory.FullName + "\\" + relativeFilename;

            file.FullFilename = fullFilename;
            file.RelativeFilename = relativeFilename;

            FileStream fs = null;

            if ( System.IO.File.Exists( fullFilename ) )
            {
                fs = new FileStream( fullFilename, FileMode.Open, FileAccess.ReadWrite );
                file.FileStream = fs;

                file.CreateBinaryReaderWriter();

                // Check the header

                string headerStart;
                string createrName;
                string relativeFilenameHeader;
                string headerEnd;

                try
                {
                    //TODO: instead of splitting the header in parts, it would be faster to first make the header in one piece and then check its correctness
                    headerStart = file.Reader.ReadString();
                    createrName = file.Reader.ReadString();
                    relativeFilenameHeader = file.Reader.ReadString();
                    headerEnd = file.Reader.ReadString();
                }
                catch ( Exception )
                {
                    throw new InvalidOperationException( "Invalid file format!!(incorrect 'DiskSerializerService' header)" );

                }

                if ( !headerStart.Equals( "DiskSerializerServiceHeaderStart" ) ||
                     !headerEnd.Equals( "HeaderEnd" ) )
                    throw new InvalidOperationException( "Invalid file format!!(incorrect 'DiskSerializerService' header)" );

                if ( !createrName.Equals( fileCreaterName ) )
                    throw new InvalidOperationException( "The file in this location is not created by this creater." );

                //TODO: the following check for the paths to be equal can cause problems when using different slashes '\' or  '/'
                if ( !relativeFilenameHeader.Equals( relativeFilename ) )
                    throw new InvalidOperationException( "The file on the disk is in the wrong location!!" );



            }
            else
            {
                FileInfo fi = new FileInfo( fullFilename );
                System.IO.Directory.CreateDirectory( fi.DirectoryName );

                fs = new FileStream( fullFilename, FileMode.CreateNew, FileAccess.ReadWrite );
                file.FileStream = fs;

                file.CreateBinaryReaderWriter();

                file.Writer.Write( "DiskSerializerServiceHeaderStart" );
                file.Writer.Write( fileCreaterName );
                file.Writer.Write( relativeFilename );
                file.Writer.Write( "HeaderEnd" );

                //NOTE: could be slow!!
                file.FileStream.Flush();

            }


            file.HeaderLength = file.FileStream.Position;


            return file;

        }


        #region IDisposable Members

        void IDisposable.Dispose()
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
}
