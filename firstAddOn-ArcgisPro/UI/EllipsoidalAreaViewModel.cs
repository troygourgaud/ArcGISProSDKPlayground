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
using ArcGIS.Desktop.Editing.Attributes;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ProDialog = ArcGIS.Desktop.Framework.Dialogs;
using System.Data;
using System.Collections;
using System.Windows;
using System.Collections.ObjectModel;
using CrosscutUtility;
using System.Collections.Generic;
using System;
using WoodsideTool.ProDesktop.Controller;
using ProWoodsideTools;
using System.Windows.Threading;
using ArcGIS.Desktop.Layouts;

namespace firstAddOn_ArcgisPro.UI
{
    internal class EllipsoidalAreaViewModel : DockPane
    {
        private const string _dockPaneID = "firstAddOn_ArcgisPro_UI_EllipsoidalArea";
        private const string _graphicLayerName = "Ellipsoidal Area Graphics";
        public List<IDisposable> CurrentGraphicList { get; set; }
        private GeometryUtility GeometryUtil;

        //CmdClearGraphics
        private ICommand _cmdClearGraphics;

        public ICommand CmdClearGraphics
        {
            get
            {
                return _cmdClearGraphics;
            }
        }

        public ICommand CmdRemoveSelected { get; set; }
        private GraphicsLayer _PolygonGraphicsLayer;
       
        protected EllipsoidalAreaViewModel()
        {
            this.DockEnabled = true;
            this.GeometryUtil = new GeometryUtility();
            this.GeodesicAreaInfo = "Graphic Area:";
            //this._cmdClearGraphics = new RelayCommand(() => this.RemoveDrawnGraphics(), () => this.CanClearGraphics());
            this._cmdClearGraphics = new RelayCommand(() => this.CheckAndRemoveGraphicLayer(), () => this.CanClearGraphics());
            this.CmdRemoveSelected = new RelayCommand(() => this.RemoveSelectedGraphic(), () => this.CanRemoveGraphics());
            this.CmdUnitChanged = new RelayCommand(() => this.HandleUnitChanged(), () => { return true; });
            this.MapViewUtil = new MapViewUtility();
            //From this constructor , it does not reach/notify the binding of properties to UI
            this.BuildUiBindedProperties();
            this.CurrentGraphicList = new List<IDisposable>();
            this._PolygonGraphicsLayer = null;
            this._esriLayoutSelect = FrameworkApplication.GetPlugInWrapper(DAML.Tool.esri_layouts_selectByRectangleTool) as ICommand;
        }

        

        private async void CheckAndCreateGraphicLayer()
        {
            var map = MapView.Active.Map;
            if (map.MapType != MapType.Map)
                return;// not 2D

            if (this._PolygonGraphicsLayer == null)//is that null?
            {
                //find in the current map
                this._PolygonGraphicsLayer = map.GetLayersAsFlattenedList().OfType<GraphicsLayer>().FirstOrDefault(x => x.Name.Equals(_graphicLayerName, StringComparison.OrdinalIgnoreCase));
                if (this._PolygonGraphicsLayer == null) // if it is not existing layer in the map
                {
                    var gl_param = new GraphicsLayerCreationParams { Name = _graphicLayerName };
                    await QueuedTask.Run(() =>
                    {
                        this._PolygonGraphicsLayer = LayerFactory.Instance.CreateLayer<GraphicsLayer>(gl_param, map);
                    });
                    this._PolygonGraphicsLayer.PropertyChanged += _PolygonGraphicsLayer_PropertyChanged;
                }
            }
        }

        private void _PolygonGraphicsLayer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.CalculateTotalAreaInfo();
        }

        private async void CheckAndRemoveGraphicLayer()
        {
            var map = MapView.Active.Map;
            if (map.MapType != MapType.Map)
                return;// not 2D

            if (this._PolygonGraphicsLayer != null)
            {
                
                await QueuedTask.Run(() =>
                {
                    map.RemoveLayer((Layer)this._PolygonGraphicsLayer);
                });
                this._PolygonGraphicsLayer = null;
            }
        }

        private bool CanClearGraphics()
        {
            if (MapView.Active != null && MapView.Active.Map != null)
            {
                var map = MapView.Active.Map;
                if (map.MapType != MapType.Map)
                    return false;// not 2D

                if (this._PolygonGraphicsLayer == null)
                {
                    return false;
                }

                if (this._PolygonGraphicsLayer.GetElementsAsFlattenedList().Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
        private bool CanRemoveGraphics()
        {
            if (MapView.Active != null && MapView.Active.Map != null)
            {
                var map = MapView.Active.Map;
                if (map.MapType != MapType.Map)
                    return false;// not 2D

                if (this._PolygonGraphicsLayer == null)
                {
                    return false;
                }

                if (this._PolygonGraphicsLayer.GetSelectedElements().Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private async void RemoveSelectedGraphic()
        {
            var map = MapView.Active.Map;
            if (map.MapType != MapType.Map)
                return;// not 2D
            IReadOnlyList<Element> graphicElementList = this._PolygonGraphicsLayer.GetSelectedElements();
            await QueuedTask.Run(() =>
            {
                this._PolygonGraphicsLayer.RemoveElements(graphicElementList);
                
            });
            this.CalculateTotalAreaInfo();
        }

      
        private async Task RemoveAllGraphics()
        {
            var ps = new ProgressorSource("Check and remove graphic...");
            if (this.CurrentGraphicList != null && this.CurrentGraphicList.Count > 0)
            {
                int totalGraphic = this.CurrentGraphicList.Count;
                for (int i = totalGraphic - 1; i > -1; i--)
                {
                    this.CurrentGraphicList[i].Dispose();
                }
                this.CurrentGraphicList = new List<IDisposable>();
            }
            this.GeodesicAreaInfo = "Graphic Area:";
            await QueuedTask.Run(() => Task.Delay(1000).Wait(), ps.Progressor); //Lets give garbage collector 1 sec.
        }
        private void BuildUiBindedProperties()
        {
            this.HiddenColumns = new List<string>() { ObjIdCol, FeatureLayerUriCol, DisplayFieldNameCol, GeometryCol };
            this.NotifyPropertyChanged("HiddenColumns");

            this._resultTable = new DataTable();

            this.LinearUnitList = new ObservableCollection<KeyValuePair<int, string>>() {
                new KeyValuePair<int, string>(LinearUnit.Kilometers.FactoryCode, "Kilometres squared"),
                new KeyValuePair<int, string>(LinearUnit.Meters.FactoryCode, "Metres squared")
            };
            this.SelectedUnitFormat = this.LinearUnitList.First();
        }
        private void BuildResultTable()
        {
            this._resultTable = new DataTable();
            this._resultTable.Columns.Add(new DataColumn(ObjIdCol, typeof(long)));
            this._resultTable.Columns.Add(new DataColumn(FeatureLayerUriCol, typeof(string)));
            this._resultTable.Columns.Add(new DataColumn(GeometryCol, typeof(Geometry)));
            this._resultTable.Columns.Add(new DataColumn(DisplayFieldNameCol, typeof(string)));
            DataColumn dc = new DataColumn(DisplayFieldValueCol, typeof(string));

            this._resultTable.Columns.Add(dc);
            this._resultTable.Columns.Add(new DataColumn(AreaCol, typeof(double)));

        }
        private async void ZoomAndUpdateTotal()
        {
            if (MapView.Active != null && this.SelectedFeature.Count > 0)
            {
                List<long> selectedOids = new List<long>();
                string featureLayerUri = "";
                double totalArea = 0;
                foreach (var selectedRow in this.SelectedFeature)
                {
                    object oidValue = ((DataRowView)selectedRow)[ObjIdCol];
                    featureLayerUri = ((DataRowView)selectedRow)[FeatureLayerUriCol] + string.Empty;
                    totalArea += Convert.ToDouble(((DataRowView)selectedRow)[AreaCol]);
                    selectedOids.Add(Convert.ToInt64(oidValue));
                }

                FeatureLayer fl = this.CurrentSelection.Keys.First(x => x.URI.Equals(featureLayerUri));
                await this.MapViewUtil.PerformZoomToSelectedAction(selectedOids, MapView.Active, fl);
                this.TotalSelectedFeatureArea = StringFormatter.FormatDecimal(totalArea, 2) + " " + this.SelectedUnitFormat.Value.ToString();
            }
            else
            {
                this.TotalSelectedFeatureArea = " ";
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

        private bool PropertyBind = false;
        private const string ObjIdCol = "ObjectId";
        private const string AreaCol = "Area";
        private const string FeatureLayerUriCol = "FeatureLayerUri";
        private const string DisplayFieldNameCol = "DisplayFieldCol";
        private const string DisplayFieldValueCol = "Feature";

        private const string GeometryCol = "ShapeCol";

        private KeyValuePair<int, string> _selectedUnitFormat;
        public KeyValuePair<int, string> SelectedUnitFormat
        {
            get => _selectedUnitFormat;
            set
            {
                _selectedUnitFormat = value;
                this.NotifyPropertyChanged();
                int linearUnitFC = _selectedUnitFormat.Key;
                this.SelectedLinearUnit = LinearUnit.CreateLinearUnit(linearUnitFC);
            }
        }

        private ObservableCollection<KeyValuePair<int, string>> _linearUnitList;
        public ObservableCollection<KeyValuePair<int, string>> LinearUnitList
        {
            get => _linearUnitList;
            set
            {
                _linearUnitList = value;
                this.NotifyPropertyChanged();
            }
        }

        private IList _hiddenColumns;

        public IList HiddenColumns
        {
            get { return _hiddenColumns; }
            set
            {
                _hiddenColumns = value;
                this.NotifyPropertyChanged();
            }
        }



        private DataTable _resultTable;
        //public DataTable ResultTableDS
        //{
        //    get
        //    {
        //        return this._resultTable;
        //    }
        //    set
        //    {
        //        SetProperty(ref _resultTable, value, () => ResultTableDS);
        //        this.NotifyPropertyChanged("ResultTable");
        //    }
        //}
        public DataView ResultTable
        {
            get
            {
                return this._resultTable.DefaultView;
            }

        }

        private IList _selectedFeature = new ArrayList();

        public IList SelectedFeature
        {
            get { return _selectedFeature; }
            set
            {
                _selectedFeature = value;
                this.NotifyPropertyChanged();
                this.ZoomAndUpdateTotal();
            }
        }
        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "Ellipsoidal Area";
        public string Heading
        {
            get { return _heading; }
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }
        private bool dockEnabled;
        public bool DockEnabled
        {
            get { return dockEnabled; }
            set
            {
                SetProperty(ref dockEnabled, value, () => DockEnabled);
            }
        }
        private string _msg = "";
        public string Message
        {
            get { return _msg; }
            set
            {
                SetProperty(ref _msg, value, () => Message);
            }
        }
        private string _TotalSelectedFeatureArea = "";
        public string TotalSelectedFeatureArea
        {
            get { return _TotalSelectedFeatureArea; }
            set
            {
                SetProperty(ref _TotalSelectedFeatureArea, value, () => TotalSelectedFeatureArea);
            }
        }

        private double? _geodesicArea;
        public double? GeoDesicArea
        {
            get { return _geodesicArea; }
            set
            {
                _geodesicArea = value;
                this.NotifyPropertyChanged();
                this.GeodesicAreaInfo = "Graphic Area: " + StringFormatter.FormatDecimal(_geodesicArea, 2) + " " + this.SelectedUnitFormat.Value.ToString();
            }
        }

        private double? _ShapePreservingArea;
        public double? ShapePreservingArea
        {
            get { return _ShapePreservingArea; }
            set
            {
                SetProperty(ref _ShapePreservingArea, value, () => ShapePreservingArea);
                this.ShapePreserveAreaInfo = "Shape preserving area:" + _ShapePreservingArea + string.Empty + this.SelectedUnitFormat.Value.ToString();
            }
        }


        private string _GeoDesicAreaInfo = "";
        public string GeodesicAreaInfo
        {
            get { return _GeoDesicAreaInfo; }
            set
            {
                SetProperty(ref _GeoDesicAreaInfo, value, () => GeodesicAreaInfo);
            }
        }
        private string _GeodesicTotalAreaInfo = "";

        public string GeodesicTotalAreaInfo
        {
            get { return _GeodesicTotalAreaInfo; }
            set
            {
                SetProperty(ref _GeodesicTotalAreaInfo, value, () => GeodesicTotalAreaInfo);
            }
        }
        private string _ShapePreserveAreaInfo = "";
        public string ShapePreserveAreaInfo
        {
            get { return _ShapePreserveAreaInfo; }
            set
            {
                SetProperty(ref _ShapePreserveAreaInfo, value, () => ShapePreserveAreaInfo);
            }
        }

        public ICommand CmdUnitChanged { get; set; }
        private MapViewUtility MapViewUtil { get; set; }
        private ICommand _esriLayoutSelect;
        private ICommand _cmdGetAreaFromFeature;

        public ICommand CmdGetAreaFromFeature
        {
            get
            {
                return _cmdGetAreaFromFeature ?? (_cmdGetAreaFromFeature = new RelayCommand(() =>
                {
                    this.btnGetAreaFromFeature();
                }));
            }
        }

        private ICommand _cmdDrawAndGetArea;
        public ICommand CmdDrawAndGetArea
        {
            get
            {
                return _cmdDrawAndGetArea ?? (_cmdDrawAndGetArea = new RelayCommand(() =>
                {
                    this.btnDrawAndGetArea();
                }));
            }
        }


        public Dictionary<FeatureLayer, List<long>> CurrentSelection { get; private set; }
        public LinearUnit SelectedLinearUnit { get; private set; }


        private async void btnDrawAndGetArea()
        {
            await FrameworkApplication.SetCurrentToolAsync("EllipsoidalArea_ProMapTools");//mappped with id from config.daml
            ((ProMapTools)FrameworkApplication.ActiveTool).OnDrawComplete += DockpaneEllipsoidalAreaViewModel_OnDrawComplete;
        }
        private async void DockpaneEllipsoidalAreaViewModel_OnDrawComplete(Geometry geometry)
        {
            //await this.RemoveAllGraphics();
            this.CheckAndCreateGraphicLayer();
            await this.AddGraphics(geometry);
            this.CalculateAndSetAreaInfo(geometry);
            this.CalculateTotalAreaInfo();
            await FrameworkApplication.SetCurrentToolAsync(DAML.Tool.esri_mapping_exploreTool);
            
            await FrameworkApplication.SetCurrentToolAsync(DAML.Tool.esri_layouts_selectByRectangleTool);
            
        }

        public void HandleUnitChanged()
        {
            if (this.GeoDesicArea.HasValue)
            {
                if (this.SelectedLinearUnit.FactoryCode == LinearUnit.Kilometers.FactoryCode)
                {
                    //previously meter , so meter to kilometer
                    this.GeoDesicArea = this.GeoDesicArea / (1000 * 1000);
                    this.ShapePreservingArea = this.ShapePreservingArea / (1000 * 1000);
                }
                else
                {
                    this.GeoDesicArea = this.GeoDesicArea * (1000 * 1000);
                    this.ShapePreservingArea = this.ShapePreservingArea * (1000 * 1000);
                }
            }
            this.ProcessBatchAreaCalculation();//Process the table
        }
        /// <summary>
        /// Remove previous drawn graphics
        /// </summary>
        /// <returns></returns>


        /// <summary>
        /// Add graphic symbol
        /// </summary>
        /// <param name="geometry"></param>
        private async Task AddGraphics(Geometry geometry)
        {
            var polygonSymbol = new CIMPolygonSymbol();
            var polygonGraphic = new CIMPolygonGraphic();
            await QueuedTask.Run(() =>
            {
                polygonSymbol = SymbolFactory.Instance.ConstructPolygonSymbol();
                polygonGraphic.Polygon = (Polygon)geometry;
                polygonGraphic.Symbol = polygonSymbol.MakeSymbolReference();

                this._PolygonGraphicsLayer.AddElement(polygonGraphic);
                polygonGraphic.PropertyChanged += PolygonGraphic_PropertyChanged;
                //IDisposable tmpGraphic = MapView.Active.AddOverlay(polygonGraphic);
                //this.CurrentGraphicList.Add(tmpGraphic);
            });
        }

        private void PolygonGraphic_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.CalculateTotalAreaInfo();
        }

        /// <summary>
        /// Get area from feature
        /// </summary>
        private void btnGetAreaFromFeature()
        {
            this.DockEnabled = false;
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                this.DockEnabled = false;
            });
        }

        private void ProcessBatchAreaCalculation()
        {
            foreach (DataRow dr in this._resultTable.Rows)
            {
                Geometry featureGeometry = (Geometry)dr[GeometryCol];
                dr[AreaCol] = this.GeometryUtil.CalculateGeoDesicArea(featureGeometry, this.SelectedLinearUnit);

            }
            this._resultTable.TableName = "ResultTable" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
        private async void CalculateTotalAreaInfo()
        {
            this.GeodesicTotalAreaInfo = "";
            if (this._PolygonGraphicsLayer != null)
            {
                double totalArea = 0.0;
                IReadOnlyList<GraphicElement> graphicElementList = this._PolygonGraphicsLayer.GetElementsAsFlattenedList();
                await QueuedTask.Run(() =>
                {
                    foreach (var graphicEle in graphicElementList)
                    {
                        var tmpGraphic = graphicEle.GetGraphic();
                        if (tmpGraphic is CIMPolygonGraphic polygonGraphic)
                        {
                            double tmpArea = this.GeometryUtil.CalculateGeoDesicArea(polygonGraphic.Polygon, this.SelectedLinearUnit);
                            totalArea += tmpArea;
                        }
                    }
                });
                this.GeodesicTotalAreaInfo = "Total Area: " + StringFormatter.FormatDecimal(totalArea, 2) + " " + this.SelectedUnitFormat.Value.ToString();
            }
        }
        private void CalculateAndSetAreaInfo(Geometry CurrentGeometry)
        {
            //https://www.esri.com/arcgis-blog/products/js-api-arcgis/uncategorized/geometryengine-part-2-measurement/?rmedium=redirect&rsource=blogs.esri.com/esri/arcgis/2015/09/16/geometryengine-part-2-measurement
            //https://community.esri.com/thread/179549

            this.GeoDesicArea = this.GeometryUtil.CalculateGeoDesicArea(CurrentGeometry, this.SelectedLinearUnit);
            this.ShapePreservingArea = this.GeometryUtil.CalculateShapePreservingArea(CurrentGeometry, this.SelectedLinearUnit);


        }
    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class EllipsoidalArea_ShowButton : Button
    {
        protected override void OnClick()
        {
            EllipsoidalAreaViewModel.Show();
        }
    }
}
