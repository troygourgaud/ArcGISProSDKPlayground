using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Core.CIM;

namespace ProWoodsideTools
{
    internal class EllipsoidalAreaMapTool : MapTool
    {
        
        public delegate void DrawComplete(Geometry geometry);
        public event DrawComplete OnDrawComplete;
       
        public EllipsoidalAreaMapTool()
        {
            
            IsSketchTool = true;
            SketchType = SketchGeometryType.Polygon;
            SketchOutputMode = SketchOutputMode.Map;
        }

        protected override Task OnToolActivateAsync(bool active)
        {
            return base.OnToolActivateAsync(active);
        }

        protected override async Task<bool> OnSketchCompleteAsync(Geometry geometry)
        {
            OnDrawComplete(geometry);                      
            return await base.OnSketchCompleteAsync(geometry);
        }

        /// <summary>
        /// Check and remove overlay
        /// </summary>
        /// <param name="hasMapViewChanged"></param>
        /// <returns></returns>
        protected override async Task OnToolDeactivateAsync(bool hasMapViewChanged)
        {           
            await base.OnToolDeactivateAsync(hasMapViewChanged);
        }

     
    }
}
