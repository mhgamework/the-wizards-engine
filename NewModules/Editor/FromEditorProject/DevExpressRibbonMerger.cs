using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Docking;
namespace MHGameWork.TheWizards.Editor
{
    public static class DevExpressRibbonMerger
    {

        /// <summary>
        /// WARNING: Dock panels are copied, not merged.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void MergeRibbonForm( RibbonForm source, RibbonForm target )
        {
            // Move the dock panels
            DockManager sourceManager = null;
            DockManager targetManager = null;
            // Following is deprecated
            if ( source is IDevexpressHasDockmanager && target is IDevexpressHasDockmanager )
            {
                sourceManager = ( (IDevexpressHasDockmanager)source ).GetDockManager();
                targetManager = ( (IDevexpressHasDockmanager)target ).GetDockManager();
            }
            //TODO: merge panels, see online documentation (copy for the moment)
            if ( sourceManager != null && targetManager != null )
            {
                for ( int i = 0; i < sourceManager.RootPanels.Count; i++ )
                {
                    DockPanel sourcePanel = sourceManager.RootPanels[ i ];
                    DockPanel targetPanel = targetManager.AddPanel( sourcePanel.Dock );
                    //TODO: Copy some(all) settings
                    targetPanel.Text = sourcePanel.Text;
                    while ( sourcePanel.ControlContainer.Controls.Count > 0 )
                    {

                        targetPanel.ControlContainer.Controls.Add( sourcePanel.ControlContainer.Controls[ 0 ] );
                    }
                    //TODO: test
                    if ( sourcePanel.ParentPanel != null )
                    {
                        DockPanel targetParentPanel = FindDockPanel( targetManager, sourcePanel.ParentPanel.Name );
                        if ( targetParentPanel == null ) throw new Exception();

                        //TODO: preserve order
                        targetPanel.DockAsTab( targetParentPanel );
                    }
                }
            }


            // Move all repository items
            MergeRepositoryItems( source, target );

            // Move all bar items and restore the itemlinks in the groups

            MergeBarItems( source, target );


            LoopThroughAllCategories( source, target );

        }
        private static DockPanel FindDockPanel( DockManager manager, string name )
        {
            for ( int i = 0; i < manager.Panels.Count; i++ )
            {
                if ( manager.Panels[ i ].Name == name ) return manager.Panels[ i ];
            }
            return null;
        }

        public static void MergeRepositoryItems( RibbonForm source, RibbonForm target )
        {
            // Edit this seems not to move but to add to both
            for ( int i = 0; i < source.Ribbon.RepositoryItems.Count; i++ )
            {
                target.Ribbon.RepositoryItems.Add( source.Ribbon.RepositoryItems[ i ] );

            }
        }

        public static void MergeBarItems( RibbonForm source, RibbonForm target )
        {
            for ( int i = 0; i < source.Ribbon.PageCategories.TotalCategory.Pages.Count; i++ )
            {
                RibbonPage page = source.Ribbon.PageCategories.TotalCategory.Pages[ i ];
                List<BarItem> items = new List<BarItem>();
                for ( int iGroup = 0; iGroup < page.Groups.Count; iGroup++ )
                {
                    RibbonPageGroup group = page.Groups[ iGroup ];
                    items.Clear();

                    for ( int iItem = 0; iItem < group.ItemLinks.Count; iItem++ )
                    {
                        items.Add( group.ItemLinks[ iItem ].Item );
                    }

                    target.Ribbon.Items.AddRange( items.ToArray() );


                    group.ItemLinks.AddRange( items.ToArray() );
                }

            }
        }

        private static void LoopThroughAllCategories( RibbonForm source, RibbonForm target )
        {
            // Loop through all categories

            MergeCategory( source.Ribbon.DefaultPageCategory, target.Ribbon.DefaultPageCategory );

            while ( source.Ribbon.PageCategories.Count > 0 )
            {
                RibbonPageCategory cat = source.Ribbon.PageCategories[ 0 ];
                // Check if category with the same caption exists in the target
                bool merged = false;
                if ( cat.Name != "" ) //This means its the defaultcategory, that is the pages without category
                    for ( int i = 0; i < target.Ribbon.PageCategories.Count; i++ )
                    {
                        if ( target.Ribbon.PageCategories[ i ].Text == cat.Text )
                        {
                            RibbonPageCategory targetCat = target.Ribbon.PageCategories[ i ];
                            MergeCategory( cat, targetCat );






                            source.Ribbon.PageCategories.Remove( cat );
                            merged = true;
                            break;
                        }
                    }
                if ( !merged )
                {
                    // Add this category
                    target.Ribbon.PageCategories.Add( cat );

                }

            }
        }
        private static void MergeCategory( RibbonPageCategory sourceCat, RibbonPageCategory targetCat )
        {
            // Loop through all pages

            while ( sourceCat.Pages.Count > 0 )
            {
                RibbonPage sourcePage = sourceCat.Pages[ 0 ];
                bool pageMerged = false;
                for ( int iPage = 0; iPage < targetCat.Pages.Count; iPage++ )
                {

                    RibbonPage targetPage = targetCat.Pages[ iPage ];
                    if ( targetPage.Text == sourcePage.Text )
                    {
                        // Merge this page
                        MergePage( sourcePage, targetPage );


                        sourceCat.Pages.Remove( sourcePage );
                        pageMerged = true;
                        break;
                    }

                }

                if ( !pageMerged )
                {
                    targetCat.Pages.Add( sourcePage );
                }

            }
        }

        public static void MergePage( RibbonPage sourcePage, RibbonPage targetPage )
        {
            //TODO: For now just add the groups, maybe a merge should occur in some cases but that is todo!
            for ( int i = 0; i < sourcePage.Groups.Count; i++ )
            {
                targetPage.Groups.Add( sourcePage.Groups[ i ] );
            }
        }
    }
}
