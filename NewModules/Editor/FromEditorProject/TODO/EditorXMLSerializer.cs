using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.ServerClient.Entity;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    /// <summary>
    /// All of the functions could be written in the classes being serialized( EditorObject, EditorModel, WorldEditor )
    /// But i would like to keep the programmatical functions seperated (Data, Editing, Serializing, ...?)
    /// </summary>
    public class EditorXMLSerializer
    {
        private WizardsEditor editor;

        public WizardsEditor Editor
        {
            get { return editor; }
            set { editor = value; }
        }


        //private List<EditorModel> models;

        public EditorXMLSerializer( WizardsEditor nEditor )
        {
            editor = nEditor;
        }


        public void SaveToXMLFile( System.IO.DirectoryInfo dir )
        {
            //objects = new List<EditorObject>();
            //entities = new List<EditorEntity>();
            //objectIDs = new Dictionary<EditorObject, int>();
            //entityIDs = new Dictionary<EditorEntity, int>();

            //string dirObjectsRelative = "Objects";
            //System.IO.DirectoryInfo objectsDir = dir.CreateSubdirectory( dirObjectsRelative );
            //string dirEntitiesRelative = "Entities";
            //System.IO.DirectoryInfo entitiesDir = dir.CreateSubdirectory( dirEntitiesRelative );

            //objects = editor.EditorObjects;
            //entities = editor.EditorEntities;

            //TWXmlNode rootNode = new TWXmlNode( TWXmlNode.CreateXmlDocument(), "WizardsEditor" );
            //TWXmlNode serializerNode = rootNode.CreateChildNode( "Serializer" );
            //serializerNode.AddAttribute( "Version", "00.01.0001" );
            //serializerNode.AddAttribute( "SVNRevision", "137" );

            //TWXmlNode objectsNode = rootNode.CreateChildNode( "Objects" );
            //objectsNode.AddAttribute( "Directory", dirObjectsRelative );
            //for ( int i = 0; i < objects.Count; i++ )
            //{
            //    EditorObject obj = objects[ i ];
            //    TWXmlNode objectNode = objectsNode.CreateChildNode( "Object" );

            //    string filename = "Object" + i.ToString( "000" ) + ".xml";



            //    SaveEditorObjectToXMLFile( obj, objectsDir.FullName + "\\" + filename );
            //    objectNode.AddAttributeInt( "ID", i );
            //    objectNode.AddAttribute( "Filename", filename );
            //    objectIDs.Add( obj, i );
            //}

            //TWXmlNode entitiesNode = rootNode.CreateChildNode( "Entities" );
            //entitiesNode.AddAttribute( "Directory", dirEntitiesRelative );
            //for ( int i = 0; i < entities.Count; i++ )
            //{
            //    EditorEntity ent = entities[ i ];
            //    TWXmlNode entityNode = entitiesNode.CreateChildNode( "Entity" );

            //    string filename = "Entity" + i.ToString( "000" ) + ".xml";



            //    SaveEditorEntityToXMLFile( ent, entitiesDir.FullName + "\\" + filename );
            //    entityNode.AddAttributeInt( "ID", i );
            //    entityNode.AddAttribute( "Filename", filename );
            //}



            //rootNode.Document.Save( dir.FullName + "\\WorldEditor.xml" );
        }



        public void LoadFromXml( System.IO.DirectoryInfo dir )
        {
            //objects = new List<EditorObject>();
            //entities = new List<EditorEntity>();
            //objectIDs = new Dictionary<EditorObject, int>();
            //entityIDs = new Dictionary<EditorEntity, int>();
            //objectsDictionary = new Dictionary<int, EditorObject>();

            //string dirObjectsRelative = "Objects";
            //System.IO.DirectoryInfo objectsDir = dir.CreateSubdirectory( dirObjectsRelative );
            //string dirEntitiesRelative = "Entities";
            //System.IO.DirectoryInfo entitiesDir = dir.CreateSubdirectory( dirEntitiesRelative );






            //TWXmlNode rootNode = TWXmlNode.GetRootNodeFromFile( dir.FullName + "\\WorldEditor.xml" );
            //if ( rootNode.Name != "WizardsEditor" ) throw new Exception( "Invalid file format!" );

            //TWXmlNode serializerNode = rootNode.FindChildNode( "Serializer" );
            //if ( serializerNode.GetAttribute( "Version" ) != "00.01.0001" ) throw new Exception( "Invalid file Version!" );

            //TWXmlNode objectsNode = rootNode.FindChildNode( "Objects" );

            //// Ignore: objectsNode.AddAttribute( "Directory", dirObjectsRelative );
            //TWXmlNode[] objectNodes = objectsNode.GetChildNodes();
            //for ( int i = 0; i < objectNodes.Length; i++ )
            //{
            //    TWXmlNode objectNode = objectNodes[ i ];
            //    if ( objectNode.Name != "Object" ) continue;


            //    int ID = objectNode.GetAttributeInt( "ID" );
            //    string filename = objectNode.GetAttribute( "Filename" );

            //    EditorObject obj = LoadEditorObjectFromXMLFile( objectsDir.FullName + "\\" + filename );

            //    editor.AddEditorObject( obj );
            //    objectsDictionary.Add( ID, obj );

            //}

            //TWXmlNode entitiesNode = rootNode.FindChildNode( "Entities" );
            //// Ignore: entitiesNode.AddAttribute( "Directory", dirEntitiesRelative );
            //TWXmlNode[] entityNodes = entitiesNode.GetChildNodes();
            //for ( int i = 0; i < entityNodes.Length; i++ )
            //{
            //    TWXmlNode entityNode = entityNodes[ i ];
            //    if ( entityNode.Name != "Entity" ) continue;

            //    // Ignore: entityNode.AddAttributeInt( "ID", i );
            //    string filename = entityNode.GetAttribute( "Filename" );


            //    EditorEntity ent = LoadEditorEntityFromXMLFile( entitiesDir.FullName + "\\" + filename );

            //    editor.AddEditorEntity( ent );

            //}




        }


        //private void SaveEditorObjectToXMLFile( EditorObject obj, string filename )
        //{
        //    System.Xml.XmlDocument doc = TWXmlNode.CreateXmlDocument();
        //    TWXmlNode objectNode = new TWXmlNode( doc, "EditorObject" );
        //    SaveEditorObject( obj, objectNode );
        //    doc.Save( filename );

        //}
        //private EditorObject LoadEditorObjectFromXMLFile( string filename )
        //{
        //    TWXmlNode objectNode = TWXmlNode.GetRootNodeFromFile( filename );
        //    if ( objectNode.Name != "EditorObject" ) throw new Exception( "Invalid file format!" );

        //    return LoadEditorObject( objectNode );
        //}

        //private void SaveEditorObject( EditorObject obj, TWXmlNode node )
        //{
        //    throw new NotImplementedException();
        //    TWXmlNode fullDataNode = node.CreateChildNode( "EditorObjectFullData" );
        //    //SaveEditorObjectFullData( obj.FullData, fullDataNode );

        //}
        //private EditorObject LoadEditorObject( TWXmlNode node )
        //{
        //    throw new NotImplementedException();
        //    EditorObject obj = new EditorObject(  );
        //    TWXmlNode fullDataNode = node.FindChildNode( "EditorObjectFullData" );
        //    //LoadEditorObjectFullData( obj, fullDataNode );

        //    for ( int i = 0; i < obj.FullData.Models.Count; i++ )
        //    {
        //        ModelFullData data = obj.FullData.Models[ i ];
        //        EditorModel model = new EditorModel();
        //        model.FullData = data;
        //        obj.Models.Add( model );
        //    }

        //    obj.LoadFromFullData();

        //    return obj;
        //}

       


        //private void SaveEditorEntityToXMLFile( EditorEntity ent, string filename )
        //{
        //    TWXmlNode entityNode = new TWXmlNode( TWXmlNode.CreateXmlDocument(), "EditorEntity" );
        //    SaveEditorEntity( ent, entityNode );
        //    entityNode.XmlDocument.Save( filename );

        //}
        //private EditorEntity LoadEditorEntityFromXMLFile( string filename )
        //{
        //    TWXmlNode entityNode = TWXmlNode.GetRootNodeFromFile( filename );
        //    if ( entityNode.Name != "EditorEntity" ) throw new Exception( "Invalid file format!" );

        //    return LoadEditorEntity( entityNode );
        //}

        //private void SaveEditorEntity( EditorEntity ent, TWXmlNode node )
        //{
        //    throw new NotImplementedException();
        //    int objectID = objectIDs[ ent.EditorObject ];
        //    node.AddChildNode( "EditorObjectID", objectID.ToString() );

        //    TWXmlNode fullDataNode = node.CreateChildNode( "EditorEntityFullData" );
        //    //SaveEditorEntityFullData( ent.FullData, fullDataNode );
        //}
        //private EditorEntity LoadEditorEntity( TWXmlNode node )
        //{
        //    throw new NotImplementedException();

        //    int objectID = node.ReadChildNodeValueInt( "EditorObjectID" );
        //    //EditorEntity ent = new EditorEntity( objectsDictionary[ objectID ] );

        //    TWXmlNode fullDataNode = node.FindChildNode( "EditorEntityFullData" );
        //    //ent.FullData = LoadEditorEntityFullData( fullDataNode );
        //    //ent.FullData.ObjectFullData = ent.EditorObject.FullData;

        //    //return ent;
        //}




        /*private List<EditorObject> ListObjects()
        {
            return editor.EditorObjects;
        }

        private List<EditorModel> ListModels()
        {
            List<EditorModel> ret = new List<EditorModel>();
            for ( int i = 0; i < objects.Count; i++ )
            {
                ret.AddRange( objects[ i ].Models );
            }
            return ret;
        }*/


        /*public static void TestSerializeWorld()
        {
            XNAGameFiles engineFiles = new XNAGameFiles();
            engineFiles.LoadDefaults( System.Windows.Forms.Application.StartupPath );

            WizardsEditorForm editor = new WizardsEditorForm();


            EditorObject obj = new EditorObject( editor );
            editor.EditorObjects.Add( obj );
            obj.AddModel( System.Windows.Forms.Application.StartupPath + @"\Content\Wall001.dae" );


            Editor.EditorXMLSerializer serializer = new EditorXMLSerializer( editor );

            serializer.SaveToXMLFile( new System.IO.DirectoryInfo( engineFiles.DirUnitTests + "\\EditorXMLSerializer\\" ) );
        }*/
    }
}
