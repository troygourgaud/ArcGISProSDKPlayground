using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;


namespace firstAddOn_ArcgisPro.UI
{
    internal class DockpaneSearchLayerFileViewModel : DockPane
    {
        private const string _dockPaneID = "firstAddOn_ArcgisPro_UI_DockpaneSearchLayerFile";

        protected DockpaneSearchLayerFileViewModel()
        {
            this.InfoMsg = "";
            this.SearchFileCommand = new RelayCommand(this.PerformSearchFileCommand, this.CanPerformSearchAction);
            this.FocusItemCommand = new RelayCommand(this.PerformFocusItemCommand, this.CanPerformFocus);
        }

        private bool CanPerformFocus()
        {
            if (this.SelectedFile!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async void PerformFocusItemCommand()
        {
           
            await (QueuedTask.Run(async() =>
            {
                string DirectoryPath = this.SelectedFile.Directory.FullName;
                string SelectedFolderConnectionPath = this.SelectedFolderConnection.Path;
                ArcGIS.Desktop.Core.IProjectWindow projectWindow = Project.GetCatalogPane(true);
                var folderContainer = Project.Current.ProjectItemContainers.First(c => c.Path == "FolderConnection");

                Item subFolderItem = this.SelectedFolderConnection;

                if (DirectoryPath.Equals(SelectedFolderConnectionPath, StringComparison.OrdinalIgnoreCase))
                {
                    await projectWindow.SelectItemAsync(this.SelectedFolderConnection, true, true, folderContainer);
                }
                else
                {
                    List<string> PathListToFind = new List<string>();
                    string HierarchyPath = DirectoryPath;
                    while (!HierarchyPath.Equals(SelectedFolderConnectionPath, StringComparison.OrdinalIgnoreCase))
                    {
                        PathListToFind.Add(HierarchyPath);
                        HierarchyPath = (new DirectoryInfo(HierarchyPath)).Parent.FullName;
                    }

                    Item CurrentSearchFolder = this.SelectedFolderConnection;
                    for (int idx = PathListToFind.Count - 1; idx >= 0; idx--)
                    {
                        string folderPath = PathListToFind[idx];
                        //Cannot be null, if null something wrong
                        if (CurrentSearchFolder != null)
                        {

                            CurrentSearchFolder = CurrentSearchFolder.GetItems().FirstOrDefault(x => x.Path.Equals(folderPath, StringComparison.OrdinalIgnoreCase));
                        }
                    }
                    //Cannot be null, if null something wrong but double check
                    if (CurrentSearchFolder != null)
                    {
                        await projectWindow.SelectItemAsync(CurrentSearchFolder, true, true, folderContainer);
                    }
                }
                
            }));
        }

        private bool CanPerformSearchAction()
        {
            ArcGIS.Desktop.Core.IProjectWindow projectWindow = Project.GetCatalogPane(true);
            var CheckedSelectedFolderConnection = projectWindow.SelectedItems.FirstOrDefault(x => x.GetType() == typeof(FolderConnectionProjectItem));
            if (CheckedSelectedFolderConnection != null && (!string.IsNullOrEmpty(this.SearchInput)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void PerformSearchFileCommand()
        {
            ArcGIS.Desktop.Core.IProjectWindow projectWindow = Project.GetCatalogPane(true);
            this.SelectedFolderConnection = projectWindow.SelectedItems.FirstOrDefault(x => x.GetType() == typeof(FolderConnectionProjectItem));
            string selectedFolderPath = ((FolderConnectionProjectItem)this.SelectedFolderConnection).Path;
            this.InfoMsg = $"Perform search in {selectedFolderPath} with {this.SearchInput} criteria";
            if (!Directory.Exists(selectedFolderPath))
            {
                MessageBox.Show("Please select a valid folder connection from catalog and search again");
                return;
            }
            DirectoryInfo dirInfo = new DirectoryInfo(selectedFolderPath);
            this.SearchResult = new List<FileInfo>();
            FileInfo[] resultFileList = dirInfo.GetFiles(this.SearchInput, SearchOption.AllDirectories);
            if(resultFileList!= null && resultFileList.Length > 0)
            {
                this.SearchResult = resultFileList.ToList();
            }
        }

        /// <summary>
        /// Show the DockPane.
        /// </summary>
        internal static void Show()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane == null)
                return;

            pane.Activate();
        }

        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "Search Layer file from selected folder connection";
        public string Heading
        {
            get { return _heading; }
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }

        private string _searchInput;
        public string SearchInput
        {
            get { return _searchInput; }
            set
            {
                SetProperty(ref _searchInput, value, () => SearchInput);
            }
        }
        private string _infoMsg;
        public string InfoMsg
        {
            get { return _infoMsg; }
            set
            {
                SetProperty(ref _infoMsg, value, () => InfoMsg);
            }
        }
        
        private List<FileInfo> _searchResult;
        public List<FileInfo> SearchResult
        {
            get { return _searchResult; }
            set
            {
                SetProperty(ref _searchResult, value, () => SearchResult);
            }
        }
        private FileInfo _selectedFile;
        /// <summary>
        /// ArcGIS Pro project file is selected and will be opened.
        /// </summary>
        public FileInfo SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                SetProperty(ref _selectedFile, value, () => SelectedFile);


            }
        }
        public ICommand SearchFileCommand { get; set; }
        public ICommand FocusItemCommand { get; set; }
        public Item SelectedFolderConnection { get; private set; }
    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class DockpaneSearchLayerFile_ShowButton : Button
    {
        protected override void OnClick()
        {
            DockpaneSearchLayerFileViewModel.Show();
        }
    }
}
