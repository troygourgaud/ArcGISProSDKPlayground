using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Events;
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
using ArcGIS.Desktop.Mapping.Events;

namespace firstAddOn_ArcgisPro.UI
{
    internal class SeismicLinesReportViewModel : DockPane
    {
        private const string _dockPaneID = "firstAddOn_ArcgisPro_UI_SeismicLinesReport";

        protected SeismicLinesReportViewModel() { }

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
        private string _heading = "Seismic Lines Report";
        public string Heading
        {
            get { return _heading; }
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }
        private SubscriptionToken _eventToken = null;
        private SubscriptionToken _toceventToken = null;
        // Called when the visibility of the DockPane changes.
        protected override void OnShow(bool isVisible)
        {
            if (isVisible && _eventToken == null) //Subscribe to event when dockpane is visible
            {
                _eventToken = MapSelectionChangedEvent.Subscribe(OnMapSelectionChangedEvent);
                _toceventToken= TOCSelectionChangedEvent.Subscribe(OnTOCSelectionChangeEvent);
            }

            if (!isVisible && _eventToken != null) //Unsubscribe as the dockpane closes.
            {
                MapSelectionChangedEvent.Unsubscribe(_eventToken);
                TOCSelectionChangedEvent.Unsubscribe(_toceventToken);
                _eventToken = null;
            }
        }

        private void OnTOCSelectionChangeEvent(MapViewEventArgs obj)
        {
            System.Diagnostics.Debug.WriteLine("toc event change");
        }

        //Event handler when the MapSelection event is triggered.
        private void OnMapSelectionChangedEvent(MapSelectionChangedEventArgs obj)
        {
            System.Diagnostics.Debug.WriteLine("Map selection change");
        }

    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class SeismicLinesReport_ShowButton : Button
    {
        protected override void OnClick()
        {
            SeismicLinesReportViewModel.Show();
        }
    }
}
