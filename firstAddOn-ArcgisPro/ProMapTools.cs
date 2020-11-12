using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace firstAddOn_ArcgisPro
{
    internal class ProMapTools : MapTool
    {

        public delegate void DrawComplete(Geometry geometry);
        public event DrawComplete OnDrawComplete;

        public ProMapTools()
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
