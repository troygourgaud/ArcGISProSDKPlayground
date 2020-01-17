using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Catalog;

namespace firstAddOn_ArcgisPro
{
    //ArcGIS.Desktop.Framework.Controls.SearchTextBox" how about trying this?
    public class SearchCatalogEditBox : ArcGIS.Desktop.Framework.Contracts.EditBox
    {
        public SearchCatalogEditBox() => Module1.Current.RibbonSearchCatalogEditBox = this;
        

        protected override void OnEnter() //this fires twice on a single keypress  
        {
            //MessageBox.Show($"enter pressed and the content is {this.Text}");

            DockPane catalogPane = this.OpenAndGetCatalogPane();

            if (catalogPane != null)
            {
                //No wait inside
               var ProcessSearchQueued =  QueuedTask.Run(async() =>
                {
                    IProjectWindow projectWindow = Project.GetCatalogPane(true);
                    //PaneCollection instantiatedPane = FrameworkApplication.Panes; //GDB
                    var dbConnectionContainer = Project.Current.ProjectItemContainers.First(x => x.Path == "GDB");
                    Item dbItems = (Item)Project.Current.GetItems<GDBProjectItem>().Where(x => x.Path.Contains(this.Text.Trim()));

                    //await projectWindow.SelectItemAsync(dbItems, true, true, dbConnectionContainer);
                });
                //Let activate the catalog window
                ProcessSearchQueued.Wait();
            }
        }

        public DockPane OpenAndGetCatalogPane()
        {
            DockPane catalogPane = FrameworkApplication.DockPaneManager.Find("esri_core_projectDockPane");
            var catalogPaneInstantiated = FrameworkApplication.DockPaneManager.IsDockPaneCreated("esri_core_projectDockPane");

            if (catalogPane == null)
            {
                if (!catalogPaneInstantiated)
                {
                    Console.WriteLine("Catalog Pane has been instantiated");
                }
                MessageBox.Show("Please open the catalog window and select an item to start searching");
            }
            else
            {
                if (!catalogPane.IsVisible)
                {
                    catalogPane.Activate(true);
                }
            }
            return catalogPane;
        }
    }
}
