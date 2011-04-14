using System;
using System.Collections.Generic;
using System.Text;
using DevComponents.DotNetBar;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class DockTab : DevComponents.DotNetBar.DockContainerItem
    {
        /// <summary>
        /// This automatically puts a PanelDockContainer in this.Control
        /// </summary>
        public DockTab()
        {
            Control = new DevComponents.DotNetBar.PanelDockContainer();
        }

        public event EventHandler<EventArgs> TabActivated;


        /// <summary>
        /// This function is called by the WizardsEditorForm
        /// </summary>
        public void OnTabActivated()
        {
            if ( TabActivated != null ) TabActivated( this, null );
        }

        public event EventHandler<EventArgs> TabDeactivated;

        /// <summary>
        /// This function is called by the WizardsEditorForm
        /// </summary>
        public void OnTabDeactivated()
        {
            if ( TabDeactivated != null ) TabDeactivated( this, null );
        }

        public event EventHandler<DevComponents.DotNetBar.DockTabClosingEventArgs> TabClosed;

        /// <summary>
        /// This function is called by the WizardsEditorForm
        /// </summary>
        public void OnTabClosing( object sender, DockTabClosingEventArgs e )
        {
            if ( TabClosed != null ) TabClosed( sender, e );
        }

       

        
    }
}
