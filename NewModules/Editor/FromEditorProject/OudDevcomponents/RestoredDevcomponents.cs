using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    [Obsolete( "This class is to be replaced by a DevExpress form" )]
    public partial class RestoredDevcomponents : Office2007RibbonForm
    {
        private WizardsEditor editor;

        public WizardsEditor Editor
        {
            get { return editor; }
            set { editor = value; }
        }





        /*private Editor.EditorObject activeEditorObject;
        public Editor.EditorObject ActiveEditorObject
        {
            get { return activeEditorObject; }
            //set { activeEditorObject = value; }
        }*/





        /*private DevComponents.AdvTree.Node nodeObjectsAll;
        public DevComponents.AdvTree.Node NodeObjectsAll
        {
            get { return nodeObjectsAll; }
            //set { nodeObjectsAll = value; }
        }*/


        public RestoredDevcomponents( WizardsEditor nEditor )
        {
            editor = nEditor;
            InitializeComponent();


            //InitObjectsList();
            //SetActiveEditorObject( null );
        }






        /**
         * Allowed code
         * 
         **/

        private void barMain_DockTabChange( object sender, DockTabChangeEventArgs e )
        {
            if ( e.OldTab != null && e.OldTab is Editor.DockTab )
            {
                ( (Editor.DockTab)e.OldTab ).OnTabDeactivated();
            }
            if ( e.NewTab is Editor.DockTab )
            {
                ( (Editor.DockTab)e.NewTab ).OnTabActivated();
            }
        }

        private void barMain_DockTabClosing( object sender, DockTabClosingEventArgs e )
        {

            if ( !( e.DockContainerItem is Editor.DockTab ) ) return;

            ( (Editor.DockTab)e.DockContainerItem ).OnTabDeactivated();
            ( (Editor.DockTab)e.DockContainerItem ).OnTabClosing( sender, e );

        }


        /**
         * End Allowed code
         * 
         **/


        private void ribbonTabItem1_Click( object sender, EventArgs e )
        {

        }



        private void btnMainExit_Click( object sender, EventArgs e )
        {
            ExitWorldEditor();
        }

        public void ExitWorldEditor()
        {
            Close();
            Application.ExitThread();
        }





        public void SetMainBarActiveTab( DockContainerItem item )
        {

        }






        /*public void SetActiveEditorObject( Editor.EditorObject obj )
        {
            activeEditorObject = obj;
            if ( activeEditorObject != null )
            {

                btnObjectAddModel.Enabled = true;
                txtObjectName.Enabled = true;
            }
            else
            {
                btnObjectAddModel.Enabled = false;
                txtObjectName.Enabled = false;
            }
        }*/


        /*private void InitObjectsList()
        {
            treeObjects.Nodes.Clear();
            DevComponents.AdvTree.Node rootNode;

            rootNode = new DevComponents.AdvTree.Node();
            rootNode.Text = "Objects";

            treeObjects.Nodes.Add( rootNode );

            nodeObjectsAll = new DevComponents.AdvTree.Node();
            nodeObjectsAll.Text = "All";

            rootNode.Nodes.Add( nodeObjectsAll );

        }*/


        private void buttonItem13_Click( object sender, EventArgs e )
        {
            ExitWorldEditor();
        }

        [STAThread]
        public static void RunWorldEditor()
        {
            WizardsEditor editor = new WizardsEditor();
            editor.RunEditorDevcomponents();


        }


        private void ribbonTabItem2_Click( object sender, EventArgs e )
        {

        }

        private void buttonItem1_Click( object sender, EventArgs e )
        {

        }

        private void buttonItem7_Click( object sender, EventArgs e )
        {

        }


        private void barRightIGuess_ItemClick( object sender, EventArgs e )
        {

        }

        private void ribbonTabWorldTerrain_Click( object sender, EventArgs e )
        {

        }


    }
}