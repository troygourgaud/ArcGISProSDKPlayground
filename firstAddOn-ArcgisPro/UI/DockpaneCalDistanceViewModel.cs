using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    internal class DockpaneCalDistanceViewModel : DockPane
    {
        private const string _dockPaneID = "firstAddOn_ArcgisPro_UI_DockpaneCalDistance";
        private bool TrackDrawToolActive { get; set; }

        protected DockpaneCalDistanceViewModel()
        {
            this.TrackDrawToolActive = false;
        }

        private void TrackingActiveMapToolPropertyChange(object sender, PropertyChangedEventArgs e)
        {
            if (this.TrackDrawToolActive)
            {
                Console.WriteLine("Active draw tool change");
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
        private string _heading = "Compare Calculate Distance formula";
        public string Heading
        {
            get { return _heading; }
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }



        private string _vincencyResult;
        private string _karneyResult;

        public string NativeResult
        {
            get => _vincencyResult;
            set
            {
                SetProperty(ref _vincencyResult, value, () => NativeResult);
            }
        }
        public string KarneyResult
        {
            get => _karneyResult;
            set
            {
                SetProperty(ref _karneyResult, value, () => KarneyResult);
            }
        }



        private ICommand _cmdDrawDistance;
        public ICommand CmdDrawDistance
        {
            get
            {
                return _cmdDrawDistance ?? (_cmdDrawDistance = new RelayCommand(() =>
                {
                    this.btnDrawDistanceClick();
                }));
            }
        }

        private async void btnDrawDistanceClick()
        {
            MessageBox.Show("Draw distance");
            await FrameworkApplication.SetCurrentToolAsync("firstAddOn_ArcgisPro_DrawLineMapTool");
            ((DrawLineMapTool)FrameworkApplication.ActiveTool).OnDrawLineSketchComplete += DockpaneCalDistanceViewModel_OnDrawLineSketchComplete;
        }

        private void DockpaneCalDistanceViewModel_OnDrawLineSketchComplete(Geometry geometry)
        {
            Console.WriteLine("Got geometry from dock pane as well");

            string inGeometryUnit = geometry.SpatialReference.Unit.Name;
            double value = GeometryEngine.Instance.GeodesicLength(geometry);
            this.NativeResult = $"{value}: {inGeometryUnit}";

        }
    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class DockpaneCalDistance_ShowButton : Button
    {
        protected override void OnClick()
        {
            DockpaneCalDistanceViewModel.Show();
        }
    }
}
