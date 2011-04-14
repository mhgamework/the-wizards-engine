using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using MHGameWork.TheWizards.Entities;
using MHGameWork.TheWizards.ServerClient.Database;
using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.ServerClient.Terrain;
using MHGameWork.TheWizards.ServerClient.TWClient;
using MHGameWork.TheWizards.ServerClient.Utilities;

namespace MHGameWork.TheWizards.Editor
{
    public class WizardsEditor : IGameService
    {

        #region Obsolete
        public void EnableObjectEditorButtons()
        {
            form.ribbonTabObjectGeneral.Enabled = true;
            form.ribbonTabObjectGeneral.Visible = true;
            form.ribbonControl.RecalcLayout();
            form.ribbonTabObjectGeneral.Select();
        }
        public void DisableObjectEditorButtons()
        {
            form.ribbonTabObjectGeneral.Enabled = false;
            form.ribbonTabObjectGeneral.Visible = false;
            form.ribbonControl.SelectFirstVisibleRibbonTab();
            form.ribbonControl.RecalcLayout();
        }

        public void EnableWorldEditorButtons()
        {
            EnableRibbonTab( form.ribbonTabWorldGeneral );
            EnableRibbonTab( form.ribbonTabWorldTerrain );

            form.ribbonTabWorldGeneral.Select();

            form.ribbonControl.RecalcLayout();

        }
        public void DisableWorldEditorButtons()
        {
            DisableRibbonTab( form.ribbonTabWorldGeneral );
            DisableRibbonTab( form.ribbonTabWorldTerrain );
            form.ribbonControl.SelectFirstVisibleRibbonTab();
            form.ribbonControl.RecalcLayout();

        }

        private void DisableRibbonTab( RibbonTabItem tab )
        {
            tab.Enabled = false;
            tab.Visible = false;
        }

        private void EnableRibbonTab( RibbonTabItem tab )
        {
            tab.Enabled = true;
            tab.Visible = true;
        }

        //private void btnCreateObject_Click( object sender, EventArgs e )
        //{
        //    Entity.TaggedObject tObj = EntityManagerService.CreateObject();

        //    EditorObject obj = tObj.GetTag<EditorObject>();

        //    AddEditorObject( obj );

        //    obj.MakeActiveObject();

        //    obj.OpenObjectEditor();

        //}
        void btnLoadAll_Click( object sender, EventArgs e )
        {
            /*EditorXMLSerializer serializer = new EditorXMLSerializer( this );
            System.IO.DirectoryInfo dirMain = System.IO.Directory.CreateDirectory( Application.StartupPath + "\\WizardsEditorSave" );
            serializer.LoadFromXml( dirMain );*/

            database.FindService<DiskLoaderService>().LoadFromDisk();

            TerrainManagerService tms = database.FindService<TerrainManagerService>();

            terrains.Clear();

            for ( int i = 0; i < tms.Terrains.Count; i++ )
            {
                EditorTerrain eTerr = new EditorTerrain( tms, tms.Terrains[ i ] );
                terrains.Add( eTerr );

                for ( int iTex = 0; iTex < eTerr.FullData.Textures.Count; iTex++ )
                {
                    TerrainFullData.TerrainTexture tex = eTerr.FullData.Textures[ iTex ];
                    Form.editorWindowTerrainTextures.textureChooser.AddTexture( tex.DiffuseTexture );
                }
            }

            //for ( int i = 0; i < EntityManagerService.Objects.Count; i++ )
            //{
            //    AddEditorObject( EntityManagerService.Objects[ i ].GetTag<EditorObject>() );
            //}
            //for ( int i = 0; i < EntityManagerService.Entities.Count; i++ )
            //{
            //    AddEditorEntity( EntityManagerService.Entities[ i ].GetTag<EditorEntity>() );
            //}
        }

        void btnSaveAll_Click( object sender, EventArgs e )
        {
            /*EditorXMLSerializer serializer = new EditorXMLSerializer( this );
            System.IO.DirectoryInfo dirMain = System.IO.Directory.CreateDirectory( Application.StartupPath + "\\WizardsEditorSave" );
            serializer.SaveToXMLFile( dirMain );*/

            database.FindService<DiskLoaderService>().SaveToDisk();

            for ( int i = 0; i < terrains.Count; i++ )
            {
                terrains[ i ].FullData.SaveFullData( database );
            }

            //for ( int i = 0; i < EditorObjects.Count; i++ )
            //{
            //    EditorObject editorObject = EditorObjects[ i ];
            //    editorObject.SaveToDisk();
            //}
            //for ( int i = 0; i < editorEntities.Count; i++ )
            //{
            //    EditorEntity editorEntity = editorEntities[ i ];
            //    editorEntity.SaveToDisk();
            //}

        }

        void btnOpenWorld_Click( object sender, EventArgs e )
        {
            if ( worldEditor == null )
            {
                worldEditor = new WorldEditor( this );
                worldEditor.TabClosed += new EventHandler<DockTabClosingEventArgs>( worldEditor_TabClosed );

            }
            worldEditor.Show();
        }

        public WizardsEditorFormDevcomponents Form
        {
            get { return form; }
            //set { form = value; }
        }
        private WizardsEditorFormDevcomponents form;
        #endregion
        #region Properties

        private Database.Database database;

        public Database.Database Database
        {
            get { return database; }
            //set { database = value; }
        }

        public readonly EntityManagerService EntityManagerService;
        public WizardsEditorForm FormNew
        {
            get
            {
                return formNew;
            }
            set
            {
                formNew = value;
            }
        }

        private WizardsEditorForm formNew;

        public TerrainQuadtreeNode QuadTree
        {
            get { return quadTree; }
            set { quadTree = value; }
        }

        private TerrainQuadtreeNode quadTree;
        public readonly TerrainManagerService TerrainManagerService;

        public WorldEditor WorldEditor
        {
            get { return worldEditor; }
            set { worldEditor = value; }
        }

        private WorldEditor worldEditor;
        #endregion
        #region Should be in the database class

        //public void AddEditorObject( EditorObject obj )
        //{
        //    editorObjects.Add( obj );

        //    objectsList.AddObject( obj );
        //}

        //public void AddEditorEntity( EditorEntity ent )
        //{
        //    editorEntities.Add( ent );
        //}

        //public EditorEntity CreateEntity( EditorObject obj )
        //{
        //    Entity.TaggedEntity ent = EntityManagerService.CreateEntity();


        //    EditorEntity ret = ent.GetTag<EditorEntity>();


        //    ret.EditorObject = obj;

        //    ret.FullData.TaggedObject = obj.TaggedObject;

        //    AddEditorEntity( ret );



        //    return ret;
        //}

        /*/// <summary>
        /// TODO: This should be a database function
        /// EDIT: TODO: This function is invalid, since the renderdata should only be shared among one game. When
        /// EDIT: TODO:  this renderdata is used in another game this causes an exceptoin
        /// </summary>
        /// <returns></returns>
        public EditorObjectRenderData GetEditorObjectRenderData( IXNAGame game, EditorObject obj )
        {
            if ( obj.RenderData == null )
            {
                obj.RenderData = new EditorObjectRenderData();
                obj.RenderData.Initialize( game, obj.FullData );
            }

            return obj.RenderData;
        }*/


        //public void DeleteEditorEntity( EditorEntity entity )
        //{
        //    editorEntities.Remove( entity );
        //}


        /// <summary>
        /// DEPRECATED: moved to TerrainManagerService, use the service instead
        /// </summary>
        /// <returns></returns>
        public EditorTerrain CreateTerrain()
        {
            TaggedTerrain terr = database.FindService<TerrainManagerService>().CreateTerrain();

            EditorTerrain t = new EditorTerrain( database.FindService<TerrainManagerService>(), terr );
            terrains.Add( t );
            return t;
        }




        //private List<EditorEntity> editorEntities;
        ///// <summary>
        ///// TODO: Should move to database 
        ///// </summary>
        //public List<EditorEntity> EditorEntities
        //{
        //    get { return editorEntities; }
        //    set { editorEntities = value; }
        //}
        //// TODO: Temporary storage! should move to database
        //private List<EditorObject> editorObjects;
        //// TODO: Temporary storage! should move to database
        //public List<EditorObject> EditorObjects
        //{
        //    get { return editorObjects; }
        //    set { editorObjects = value; }
        //}



        //private EditorObjectsList objectsList;

        //public EditorObjectsList ObjectsList
        //{
        //    get { return objectsList; }
        //    set { objectsList = value; }
        //}

        public List<EditorTerrain> Terrains
        {
            get { return terrains; }
            set { terrains = value; }
        }

        private List<EditorTerrain> terrains;
        #endregion

        private WorldDatabase.WorldDatabase worldDatabase;

        public WorldDatabase.WorldDatabase WorldDatabase
        {
            get { return worldDatabase; }
        }

        private List<IWorldEditorExtension> worldEditorExtensions = new List<IWorldEditorExtension>();
        private List<IWizardsEditorDatabaseExtension> databaseExtensions = new List<IWizardsEditorDatabaseExtension>();

        public void AddWorldEditorExtension( IWorldEditorExtension extension )
        {
            worldEditorExtensions.Add( extension );

        }
        public void AddDatabaseExtension(IWizardsEditorDatabaseExtension extension)
        {
            databaseExtensions.Add(extension);
        }
        /// <summary>
        /// This method will probably be removed, since this is unlogical
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T FindWorldEditorExtension<T>() where T : class, IWorldEditorExtension
        {
            return TypeSearcher.FindItem<T, IWorldEditorExtension>( worldEditorExtensions );
        }

        private EditorFiles files;

        public EditorFiles Files
        {
            get { return files; }
        }


        public WizardsEditor()
        {
            files = new EditorFiles( Application.StartupPath );
            //editorObjects = new List<EditorObject>();
            //editorEntities = new List<EditorEntity>();
            terrains = new List<EditorTerrain>();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            form = new WizardsEditorFormDevcomponents( this );
            //form.btnCreateObject.Click += new EventHandler( btnCreateObject_Click );
            form.btnOpenWorld.Click += new EventHandler( btnOpenWorld_Click );
            form.btnSaveAll.Click += new EventHandler( btnSaveAll_Click );
            form.btnLoadAll.Click += new EventHandler( btnLoadAll_Click );
            form.btnStartServerClient.Click += new EventHandler( btnStartServerClient_Click );

            DisableObjectEditorButtons();
            DisableWorldEditorButtons();


            formNew = new WizardsEditorForm( this );

            formNew.btnLoadAll.ItemClick += new ItemClickEventHandler(btnLoadAll_ItemClick);
            formNew.btnSaveAll.ItemClick += new ItemClickEventHandler(btnSaveAll_ItemClick);



            //objectsList = new EditorObjectsList( this );

            quadTree = new TerrainQuadtreeNode(); //TODO


            // Load services

            // Main Services
            database = new Database.Database();
            database.AddService( this );
            database.AddService( new DiskSerializerService( database, Application.StartupPath + "\\WizardsEditorSave" ) );
            database.AddService( new DiskLoaderService( database ) );
            database.AddService( new SettingsService( database, Application.StartupPath + "\\Settings.xml" ) );
            database.AddService( new UniqueIDService( database ) );
            //System.IO.DirectoryInfo dirMain = System.IO.Directory.CreateDirectory( Application.StartupPath + "\\WizardsEditorSave" );

            //database.FindService<DiskLoaderService>().SetSerializationDirectory( dirMain );

            // Load all plugins in assembly
            //List<IPlugin002> plugins = Editor.PluginLoader.GetPlugins<IPlugin002>( System.Reflection.Assembly.GetExecutingAssembly() );
            List<IPlugin002> plugins = PluginLoader.GetPlugins<IPlugin002>( AppDomain.CurrentDomain.GetAssemblies() );
            for ( int i = 0; i < plugins.Count; i++ )
            {
                IPlugin002 plugin = plugins[ i ];
                plugin.LoadPlugin( database );
            }

            //database.FindService<DiskSerializerService>().AddDiskSerializer( terrainManager );
            //database.FindService<DiskSerializerService>().AddDiskSerializer( new TerrainDiskSerializer(database) );




            LoadPlugins();

            EntityManagerService = database.FindService<EntityManagerService>();
            TerrainManagerService = database.FindService<TerrainManagerService>();

            worldDatabase  = new WorldDatabase.WorldDatabase(Application.StartupPath +  "\\WizardsEditorWorld");

        }

        public void SaveToWorkingCopy()
        {
            for (int i = 0; i < databaseExtensions.Count; i++)
            {
                databaseExtensions[i].BeforeSaveWorkingCopy(worldDatabase);
            }
            worldDatabase.SaveWorkingCopy();
            MessageBox.Show("Saved succesfully to the working copy!");
        }

        void btnSaveAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            btnSaveAll_Click(null, null);
        }

        void btnLoadAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            btnLoadAll_Click(null, null);
        }

        void btnStartServerClient_Click( object sender, EventArgs e )
        {
            /*System.Threading.Thread t = new System.Threading.Thread( ServerClient.Program.RunGame );
            t.Start();*/
        }

        private void LoadPlugins()
        {

            List<IPlugin002> plugins = PluginLoader.GetPlugins<IPlugin002>( Files.RootDirectory + @"\Plugins" );
            for ( int i = 0; i < plugins.Count; i++ )
            {
                IPlugin002 plugin = plugins[ i ];

                plugin.LoadPlugin( database );

            }
        }



        public void RunEditor()
        {
            Application.Run( formNew );
        }
        public void RunEditorDevcomponents()
        {
            Application.Run( form );
        }

        [Obsolete( "DevComponents is not supported anymore!" )]
        public DockTab CreateNewTab()
        {
            DockTab item = new DockTab();
            form.barMain.Items.Add( item );

            /*if ( item.Control.Parent != form.barMain )
            {
                if ( item.Control != null ) throw new Exception( "What should happen in this case?" );
                form.barMain.Controls.Add( item.Control );
            }*/

            item.Visible = true;

            if ( !form.barMain.Visible )
                form.barMain.Visible = true;
            else
                form.barMain.RecalcLayout();

            return item;
        }

        /*public void AddTab( DockContainerItem item )
        {
            if ( item.ContainerControl != form.barMain )
            {
                // Not on the bar yet
                form.barMain.Items.Add( item );

            }
            if ( item.Control.Parent != form.barMain )
            {
                if ( item.Control != null ) throw new Exception( "What should happen in this case?" );
                form.barMain.Controls.Add( item.Control );
            }

            item.Visible = true;

            if ( !form.barMain.Visible )
                form.barMain.Visible = true;
            else
                form.barMain.RecalcLayout();
        }*/

        public void AddMDIForm( Form form )
        {
            form.MdiParent = FormNew;
            form.Show();

        }

        public void OpenWorldEditor()
        {
            if ( worldEditor == null )
            {
                worldEditor = new WorldEditor( this );
                // Unmerge the ribbon, so that extensions can add stuff
                FormNew.Ribbon.MdiMergeStyle = RibbonMdiMergeStyle.Never;
                worldEditor.TabClosed += new EventHandler<DockTabClosingEventArgs>( worldEditor_TabClosed );

                for ( int i = 0; i < worldEditorExtensions.Count; i++ )
                {
                    worldEditorExtensions[ i ].Load( worldEditor );
                }
            }
            // Remerge the ribbon
            FormNew.Ribbon.MdiMergeStyle = RibbonMdiMergeStyle.Always;
            worldEditor.Show();
        }
        void worldEditor_TabClosed( object sender, DockTabClosingEventArgs e )
        {
            worldEditor.TabClosed -= new EventHandler<DockTabClosingEventArgs>( worldEditor_TabClosed );
            //worldEditor.Close();

            for ( int i = 0; i < worldEditorExtensions.Count; i++ )
            {
                worldEditorExtensions[ i ].Unload( worldEditor );
            }


            worldEditor = null;

        }


        public static void TestRunEditor()
        {
            WizardsEditor editor = new WizardsEditor();
            editor.RunEditor();
        }

        public static void TestRunEditorDevcomponents()
        {
            WizardsEditorFormDevcomponents.RunWorldEditor();
        }


        #region IDisposable Members

        void IDisposable.Dispose()
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
}