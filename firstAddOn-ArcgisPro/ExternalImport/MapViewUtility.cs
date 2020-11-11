using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoodsideTool.ProDesktop.Controller
{
    public class MapViewUtility
    {
        /// <summary>
        /// Zoom to selected features
        /// </summary>
        /// <param name="SelectedOids"></param>
        /// <param name="TargetMapView"></param>
        /// <returns></returns>
        public async Task PerformZoomToSelectedAction(List<long> SelectedOids, MapView TargetMapView, FeatureLayer SelectedLayer)
        {
            await QueuedTask.Run(() =>
            {
                TargetMapView.Map.SetSelection(new Dictionary<MapMember, List<long>>() { { SelectedLayer, SelectedOids } });
            });
            await TargetMapView.ZoomToSelectedAsync();
        }

        /// <summary>
        /// Add selected row into existing selection into attribute table
        /// </summary>
        /// <param name="selectedOids"></param>
        /// <param name="aMapView"></param>
        /// <returns></returns>
        public async Task PerformAddHighlightedRowAction(List<long> selectedOids, MapView aMapView, FeatureLayer SelectedLayer)
        {
            await QueuedTask.Run(() =>
            {
                var selectedFeatures = aMapView.Map.GetSelection()
                          .Where(kvp => kvp.Key is BasicFeatureLayer && kvp.Key.Equals(SelectedLayer))
                          .ToDictionary(kvp => (BasicFeatureLayer)kvp.Key, kvp => kvp.Value);
                if (selectedFeatures != null && selectedFeatures.Count > 0)
                {
                    List<long> oidList = selectedFeatures[SelectedLayer];
                    selectedOids.AddRange(oidList);
                }

                aMapView.Map.SetSelection(new Dictionary<MapMember, List<long>>() { { SelectedLayer, selectedOids } });
            });
        }
    }
}
