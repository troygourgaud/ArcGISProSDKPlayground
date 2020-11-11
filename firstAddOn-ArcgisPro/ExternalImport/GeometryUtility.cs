using ArcGIS.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ArcGIS.Core.CIM;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using ArcGIS.Desktop.Internal.DesktopService;
using CrosscutUtility;

namespace WoodsideTool.ProDesktop.Controller
{
    public class GeometryUtility
    {
        public bool HasError { get; set; }
        public string ErrorDescription { get; set; }

        public static bool isValidLatLon(double lat, double lon)
        {
            if (lat > 90 || lat < -90 || lon > 180 || lon < lon - 180)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public ObservableCollection<KeyValuePair<int, string>> GetLinearUnits()
        {
            //LinearUnit.Meters.ToString() in au Metre
            return new ObservableCollection<KeyValuePair<int, string>>() {
                new KeyValuePair<int, string>(LinearUnit.Kilometers.FactoryCode, "Kilometres") ,
                new KeyValuePair<int, string>(LinearUnit.Meters.FactoryCode, "Metres") ,
                new KeyValuePair<int, string>(LinearUnit.NauticalMiles.FactoryCode, "Nautical miles") };

        }
        public List<MapPoint> ProcessGeometryToPoints(Geometry geometry)
        {
            List<MapPoint> MapPointList = new List<MapPoint>();
            HasError = false;//Defined positive first

            if (geometry == null)
            {
                HasError = true;
                Trace.WriteLine("Geometry Is Null.");
            }
            else
            {
                Trace.WriteLine("geometry.GeometryType = " + geometry.GeometryType);

                if (geometry.IsEmpty)

                {
                    HasError = true;
                    Trace.WriteLine("Geometry Is Empty.");
                }

                else
                {

                    switch (geometry.GeometryType)
                    {

                        case GeometryType.Point:
                            MapPoint point = geometry as MapPoint;
                            MapPointList.Add(point);
                            return MapPointList;

                        //case GeometryType.Ray:

                        //    IRay ray = geometry as IRay;

                        //    Trace.WriteLine("ray.Origin = " + PointToString(ray.Origin));
                        //    Trace.WriteLine("ray.Vector.XComponent = " + ray.Vector.XComponent);
                        //    Trace.WriteLine("ray.Vector.YComponent = " + ray.Vector.YComponent);
                        //    Trace.WriteLine("ray.Vector.ZComponent = " + ray.Vector.ZComponent);
                        //    Trace.WriteLine("ray.Vector.Magnitude = " + ray.Vector.Magnitude);
                        //    break;

                        case GeometryType.Polyline:

                            Polyline line = geometry as Polyline;
                            MapPointList = line.Points.ToList<MapPoint>();

                            break;

                        case GeometryType.Envelope:

                            Envelope envelope = geometry as Envelope;

                            MapPoint ltPoint = MapPointBuilder.CreateMapPoint(envelope.XMin, envelope.YMax, envelope.ZMin, geometry.SpatialReference);
                            MapPoint rtPoint = MapPointBuilder.CreateMapPoint(envelope.XMax, envelope.YMax, envelope.ZMin, geometry.SpatialReference);
                            MapPoint rbPoint = MapPointBuilder.CreateMapPoint(envelope.XMax, envelope.YMin, envelope.ZMin, geometry.SpatialReference);
                            MapPoint lbPoint = MapPointBuilder.CreateMapPoint(envelope.XMin, envelope.YMin, envelope.ZMin, geometry.SpatialReference);

                            MapPointList = new List<MapPoint>() { ltPoint, rtPoint, rbPoint, lbPoint };
                            return MapPointList;

                        case GeometryType.Polygon:
                            Polygon polygon = geometry as Polygon;
                            MapPointList = polygon.Points.ToList<MapPoint>();

                            return MapPointList;

                        case GeometryType.Multipatch:
                            Multipatch multipatch = geometry as Multipatch;
                            MapPointList = multipatch.Points.ToList();
                            return MapPointList;


                        default:
                            Trace.WriteLine($"Geometry Type haven't been translated yet::{geometry.GeometryType.ToString()}");
                            HasError = true;
                            return MapPointList;
                    }
                }
            }
            return MapPointList;
        }


        public string Point2DToString(MapPoint point)
        {
            return Point2DValueToString(point.X, point.Y);
        }

        public string Point2DValueToString(double x, double y)
        {
            return $"{x}, {y}";
        }
        public double CalculateGeoDesicArea(Geometry CurrentGeometry, LinearUnit OutUnitType)
        {
            //https://www.esri.com/arcgis-blog/products/js-api-arcgis/uncategorized/geometryengine-part-2-measurement/?rmedium=redirect&rsource=blogs.esri.com/esri/arcgis/2015/09/16/geometryengine-part-2-measurement
            //https://community.esri.com/thread/179549

            double geodesicArea = GeometryEngine.Instance.GeodesicArea(CurrentGeometry);



            // double shapeArea = GeometryEngine.Instance.GeodesicDistance(CurrentGeometry);
            if (OutUnitType.FactoryCode == LinearUnit.Kilometers.FactoryCode)
            {
                geodesicArea = geodesicArea / (1000 * 1000);
            }
            else if (OutUnitType.FactoryCode == LinearUnit.NauticalMiles.FactoryCode)
            {
                geodesicArea = geodesicArea / (1852 * 1852);
            }
            return geodesicArea;


        }

        public double CalculateShapePreservingArea(Geometry CurrentGeometry, LinearUnit OutUnitType)
        {
            //https://www.esri.com/arcgis-blog/products/js-api-arcgis/uncategorized/geometryengine-part-2-measurement/?rmedium=redirect&rsource=blogs.esri.com/esri/arcgis/2015/09/16/geometryengine-part-2-measurement
            //https://community.esri.com/thread/179549



            double shapeArea = GeometryEngine.Instance.ShapePreservingArea(CurrentGeometry);


            // double shapeArea = GeometryEngine.Instance.GeodesicDistance(CurrentGeometry);
            if (OutUnitType.FactoryCode == LinearUnit.Kilometers.FactoryCode)
            {
                shapeArea = shapeArea / (1000 * 1000);
            }
            else if (OutUnitType.FactoryCode == LinearUnit.NauticalMiles.FactoryCode)
            {
                shapeArea = shapeArea / (1852 * 1852);
            }
            return shapeArea;

        }
        public double CalculateDistance(Geometry CurrentGeometry, LinearUnit OutUnitType)
        {
            //https://www.esri.com/arcgis-blog/products/js-api-arcgis/uncategorized/geometryengine-part-2-measurement/?rmedium=redirect&rsource=blogs.esri.com/esri/arcgis/2015/09/16/geometryengine-part-2-measurement
            //https://community.esri.com/thread/179549
            Polyline currentPolyline = (Polyline)CurrentGeometry;

            double totalDistance = 0;
            for (int i = 1; i < currentPolyline.Points.Count; i++)
            {
                MapPoint start = currentPolyline.Points[i - 1];
                MapPoint end = currentPolyline.Points[i];
                double twoPointDistance = GeometryEngine.Instance.GeodesicDistance(start, end);
                totalDistance += twoPointDistance;
            }

            // double shapeArea = GeometryEngine.Instance.GeodesicDistance(CurrentGeometry);
            if (OutUnitType.FactoryCode == LinearUnit.Kilometers.FactoryCode)
            {
                totalDistance = totalDistance / 1000;
            }
            else if (OutUnitType.FactoryCode == LinearUnit.NauticalMiles.FactoryCode)
            {
                totalDistance = totalDistance / 1852;
            }
            return totalDistance;
            //this.ShapePreservingDistance = "Shape Preserving:" + Environment.NewLine + shapeArea + string.Empty + " square meter";
        }

        public string DirectionRadian2DMS(double dblDirRad, int intRoundValue)
        {
            string result = null;

            try
            {
                double pi = Math.PI;

                double resultDbl = dblDirRad * (180 / pi);

                int Deg = (int)Math.Truncate(resultDbl);
                double mDBL = Math.Abs(resultDbl - Math.Truncate(resultDbl)) * 60.0;
                int Mins = (int)Math.Truncate(mDBL);
                double Sec = (mDBL - Mins) * 60.0;

                result = $"{Deg.ToString("D2")}° {Mins.ToString("D2")}' {StringFormatter.FormatDecimal(Sec, 3)}\"";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
                //return null;
            }

            return result;
        }
    }
}
